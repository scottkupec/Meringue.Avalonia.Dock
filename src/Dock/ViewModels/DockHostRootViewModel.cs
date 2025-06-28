// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Meringue.Avalonia.Dock.Controls;

namespace Meringue.Avalonia.Dock.ViewModels
{
    /// <summary>
    /// Defines the view model for use with <see cref="DockHostRoot"/>.
    /// </summary>
    public partial class DockHostRootViewModel : ObservableObject
    {
        /// <summary>
        /// Holds the <see cref="PropertyChangedEventHandler"/> for all <see cref="DockToolViewModel"/> currently hosted.
        /// </summary>
        private readonly Dictionary<DockToolViewModel, PropertyChangedEventHandler> pinnedHandlers = [];

        /// <summary>
        /// Gets or sets the top-level <see cref="DockNodeViewModel"/>.
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
        /// Gets the list of <see cref="DockToolViewModel"/> that are currently unpinned and represented by <see cref="DockToolStub"/>s.
        /// </summary>
        public ObservableCollection<DockToolViewModel> UnpinnedTabs { get; } = [];

        /// <summary>
        /// Gets a value indicating whether the <see cref="UnpinnedTabs"/> strip should be displayed.
        /// </summary>
        public Boolean ShouldShowUnpinnedTabs => this.UnpinnedTabs.Count > 0;

        /// <summary>
        /// Updates <see cref="pinnedHandlers"/> for all controls in the provided <see cref="DockNodeViewModel"/>.
        /// </summary>
        /// <param name="node">The <see cref="DockNodeViewModel"/> to be processed.</param>
        // TODO: Revisit how to work with this in DockLayoutRootViewModel so it isn't internal.
        //       Either make it public because it is needed or allow controls another way to do
        //       this and make this private.
        internal void HookPinnedChangeListeners(DockNodeViewModel node)
        {
            if (node is DockTabNodeViewModel tabNode)
            {
                this.HookTabPinnedChangeListeners(tabNode);
            }
            else if (node is DockSplitNodeViewModel split)
            {
                split.Children.CollectionChanged += (sender, eventArgs) =>
                {
                    if (eventArgs.OldItems != null)
                    {
                        foreach (DockNodeViewModel oldChild in eventArgs.OldItems)
                        {
                            this.UnhookPinnedChangeListeners(oldChild);
                        }
                    }

                    if (eventArgs.NewItems != null)
                    {
                        foreach (DockNodeViewModel newChild in eventArgs.NewItems)
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
            else
            {
                // TODO: Meringue specific exceptions need added.
                throw new ArgumentOutOfRangeException(
                    nameof(node),
                    $"The provided {nameof(DockNodeViewModel)} has an unxpected node type.");
            }
        }

        /// <summary>
        /// Hooks all of the tools in the provided <paramref name="tabNode"/> to listen for changes.
        /// </summary>
        /// <param name="tabNode">The <see cref="DockTabNodeViewModel"/> to process.</param>
        private void HookTabPinnedChangeListeners(DockTabNodeViewModel tabNode)
        {
            tabNode.Tabs.CollectionChanged += (sender, eventArgs) =>
            {
                if (eventArgs.OldItems != null)
                {
                    foreach (DockToolViewModel oldTool in eventArgs.OldItems)
                    {
                        this.UnhookTool(oldTool);
                    }
                }

                if (eventArgs.NewItems != null)
                {
                    foreach (DockToolViewModel newTool in eventArgs.NewItems)
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
        /// Handles hooking a single <see cref="DockToolViewModel"/> so the current <see cref="DockHostRootViewModel"/>
        /// will be notified of changes it needs to update state based on.
        /// </summary>
        /// <param name="tool">The <see cref="DockToolViewModel"/> to proces.</param>
        private void HookTool(DockToolViewModel tool)
        {
            void Handler(Object? sender, PropertyChangedEventArgs eventArgs)
            {
                if (eventArgs.PropertyName == nameof(DockToolViewModel.IsPinned))
                {
                    this.UpdatePinnedState(tool);
                }
            }

            tool.PropertyChanged += Handler;
            this.pinnedHandlers[tool] = Handler;

            this.UpdatePinnedState(tool);
        }

        /// <summary>
        /// Cleans up handles for the provided <see cref="DockNodeViewModel"/>.
        /// </summary>
        /// <param name="node">The <see cref="DockNodeViewModel"/> to process.</param>
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
        /// Handles removing all handlers for a <see cref="DockToolViewModel"/> so the current <see cref="DockHostRootViewModel"/>
        /// will no longer be notified of changes.
        /// </summary>
        /// <param name="tool">The <see cref="DockToolViewModel"/> to process.</param>
        private void UnhookTool(DockToolViewModel tool)
        {
            if (this.pinnedHandlers.TryGetValue(tool, out PropertyChangedEventHandler? handler))
            {
                tool.PropertyChanged -= handler;
                _ = this.pinnedHandlers.Remove(tool);
            }

            _ = this.UnpinnedTabs.Remove(tool); // Also remove from unpinned view
        }

        /// <summary>
        /// Updates the state of <see cref="UnpinnedTabs"/> based on the updated state of the <paramref name="tool"/>.
        /// </summary>
        /// <param name="tool">The <see cref="DockToolViewModel"/> which has a state change to process.</param>
        private void UpdatePinnedState(DockToolViewModel tool)
        {
            if (tool.IsPinned)
            {
                if (this.UnpinnedTabs.Contains(tool))
                {
                    _ = this.UnpinnedTabs.Remove(tool);
                    this.OnPropertyChanged(nameof(this.ShouldShowUnpinnedTabs));
                }

                // the else case may be worthy of a debug assert.
            }
            else if (!this.UnpinnedTabs.Contains(tool))
            {
                this.UnpinnedTabs.Add(tool);
                this.OnPropertyChanged(nameof(this.ShouldShowUnpinnedTabs));
            }

            // the else case may also be worthy of a debug assert.
        }
    }
}
