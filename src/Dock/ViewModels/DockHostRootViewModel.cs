// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Meringue.Avalonia.Dock.Controls;

namespace Meringue.Avalonia.Dock.ViewModels
{
    /// <summary>
    /// View model for <see cref="DockHostRoot"/>.
    /// </summary>
    public partial class DockHostRootViewModel : ObservableObject
    {
        /// <summary>Hold tool pin handlers.</summary>
        private readonly Dictionary<DockToolViewModel, PropertyChangedEventHandler> pinnedHandlers = [];

        /// <summary>
        /// Gets or sets the top-level <see cref="DockNodeViewModel"/> in the dock controls tree.
        /// </summary>
        [ObservableProperty]
        private DockNodeViewModel hostRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="DockHostRootViewModel"/> class.
        /// </summary>
        /// <param name="rootNode">The root <see cref="DockNodeViewModel"/> for the dock control tree.</param>
        public DockHostRootViewModel(DockNodeViewModel rootNode)
        {
            this.HostRoot = rootNode;
            this.HookPinnedChangeListeners(this.HostRoot);
            this.OnPropertyChanged(nameof(this.ShouldShowUnpinnedTabs));
        }

        /// <summary>
        /// Gets the list of tools that are currently unpinnined and represented by <see cref="DockToolStub"/>s.
        /// </summary>
        public ObservableCollection<DockToolViewModel> UnpinnedTabs { get; } = [];

        /// <summary>
        /// Gets a value indicating whether a <see cref="UnpinnedTabs"/> strip should be displayed.
        /// </summary>
        public Boolean ShouldShowUnpinnedTabs => this.UnpinnedTabs.Count > 0;

        /// <summary>
        /// I need documented.
        /// </summary>
        /// <param name="propertyName">Needs to be documented.</param>
        /// <returns>This needs to be documented.</returns>
        public IObservable<Boolean> GetObservableProperty(String propertyName)
        {
            return this
                .WhenAnyPropertyChanged(propertyName)
                .Select(_ => this.ShouldShowUnpinnedTabs);
        }

        /// <summary>
        /// I need documented.
        /// </summary>
        /// <param name="propertyName">Needs to be documented.</param>
        /// <returns>This needs to be documented.</returns>
        public IObservable<PropertyChangedEventArgs> WhenAnyPropertyChanged(String propertyName)
        {
            return Observable
                .FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    handler => (s, e) => handler(e),
                    h => this.PropertyChanged += h,
                    h => this.PropertyChanged -= h)
                .Where(e => e.PropertyName == propertyName);
        }

        /// <summary>Hooks tabs.</summary>
        /// <param name="node">The node to walk.</param>
        // TODO: Review. Update docs.
        internal void HookPinnedChangeListeners(DockNodeViewModel node)
        {
            if (node is DockTabNodeViewModel tabNode)
            {
                this.HookTabPinnedChangeListeners(tabNode);
            }
            else if (node is DockSplitNodeViewModel split)
            {
                split.Children.CollectionChanged += (sender, e) =>
                {
                    if (e.OldItems != null)
                    {
                        foreach (DockNodeViewModel oldChild in e.OldItems)
                        {
                            this.UnhookPinnedChangeListeners(oldChild);
                        }
                    }

                    if (e.NewItems != null)
                    {
                        foreach (DockNodeViewModel newChild in e.NewItems)
                        {
                            this.HookPinnedChangeListeners(newChild);
                        }
                    }
                };

                foreach (DockNodeViewModel child in split.Children)
                {
                    this.HookPinnedChangeListeners(child);
                }
            }
        }

        /// <summary>
        /// Listen for changes on tab nodes in the HostRoot tree.
        /// </summary>
        /// <param name="tabNode">The tab node to listen for changes on.</param>
        private void HookTabPinnedChangeListeners(DockTabNodeViewModel tabNode)
        {
            tabNode.Tabs.CollectionChanged += (sender, e) =>
            {
                if (e.OldItems != null)
                {
                    foreach (DockToolViewModel oldTool in e.OldItems)
                    {
                        this.UnhookTool(oldTool);
                    }
                }

                if (e.NewItems != null)
                {
                    foreach (DockToolViewModel newTool in e.NewItems)
                    {
                        this.HookTool(newTool);
                    }
                }
            };

            foreach (DockToolViewModel tool in tabNode.Tabs)
            {
                this.HookTool(tool);
            }
        }

        /// <summary>
        /// Listen for pinned state changes on a tool.
        /// </summary>
        /// <param name="tool">The tool to listen for changes on.</param>
        private void HookTool(DockToolViewModel tool)
        {
            void Handler(Object? s, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(DockToolViewModel.IsPinned))
                {
                    this.UpdatePinnedState(tool);
                }
            }

            tool.PropertyChanged += Handler;
            this.pinnedHandlers[tool] = Handler;

            this.UpdatePinnedState(tool);
        }

        /// <summary>
        /// Remove handlers when a node goes away.
        /// </summary>
        /// <param name="node">The node going away.</param>
        private void UnhookPinnedChangeListeners(DockNodeViewModel node)
        {
            if (node is DockTabNodeViewModel tabNode)
            {
                foreach (DockToolViewModel tool in tabNode.Tabs)
                {
                    this.UnhookTool(tool);
                }
            }
            else if (node is DockSplitNodeViewModel split)
            {
                foreach (DockNodeViewModel child in split.Children)
                {
                    this.UnhookPinnedChangeListeners(child);
                }
            }
        }

        /// <summary>
        /// Remove handlers when a tool goes away.
        /// </summary>
        /// <param name="tool">The tool going away.</param>
        private void UnhookTool(DockToolViewModel tool)
        {
            if (this.pinnedHandlers.TryGetValue(tool, out PropertyChangedEventHandler? handler))
            {
                tool.PropertyChanged -= handler;
                _ = this.pinnedHandlers.Remove(tool);
            }

            _ = this.UnpinnedTabs.Remove(tool); // Also remove from unpinned view
        }

        /// <summary>Updates the state of <see cref="UnpinnedTabs"/> based on the current state of the <paramref name="tool"/>.</summary>
        /// <param name="tool">The <see cref="DockToolViewModel"/> to be updated.</param>
        private void UpdatePinnedState(DockToolViewModel tool)
        {
            if (tool.IsPinned)
            {
                if (this.UnpinnedTabs.Contains(tool))
                {
                    _ = this.UnpinnedTabs.Remove(tool);
                    this.OnPropertyChanged(nameof(this.ShouldShowUnpinnedTabs));
                }
            }
            else if (!this.UnpinnedTabs.Contains(tool))
            {
                this.UnpinnedTabs.Add(tool);
                this.OnPropertyChanged(nameof(this.ShouldShowUnpinnedTabs));
            }
        }
    }
}
