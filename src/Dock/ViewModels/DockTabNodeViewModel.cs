// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Avalonia.Controls.Primitives;
using CommunityToolkit.Mvvm.ComponentModel;
using Meringue.Avalonia.Dock.Controls;

namespace Meringue.Avalonia.Dock.ViewModels
{
    /// <summary>
    /// View model for <see cref="DockTabPanel"/>.
    /// </summary>
    public partial class DockTabNodeViewModel : DockNodeViewModel
    {
        /// <summary>Tracks pin handlers for current tabs.</summary>
        private readonly Dictionary<DockToolViewModel, PropertyChangedEventHandler> pinnedHandlers = [];

        /// <summary>The list for <see cref="DockTool"/>s to be presented as tabs.</summary>
        [ObservableProperty]
        private ObservableCollection<DockToolViewModel> tabs = [];

        /// <summary>The <see cref="NotifyCollectionChangedEventHandler"/> for the current <see cref="tabs"/>.</summary>
        private NotifyCollectionChangedEventHandler? tabsHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="DockTabNodeViewModel"/> class.
        /// </summary>
        public DockTabNodeViewModel()
        {
            this.tabs.CollectionChanged += this.OnTabsCollectionChanged;

            foreach (DockToolViewModel tab in this.tabs)
            {
                this.AttachPinnedHandler(tab);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current instance has any tabs.
        /// </summary>
        public Boolean HasTabs => this.Tabs.Count != 0;

        /// <summary>Gets the <see cref="Tabs"/> that are pinned (visible).</summary>
        public IEnumerable<DockToolViewModel> PinnedTabs =>
            this.Tabs.Where(t => t.IsPinned);

        /// <summary>
        /// Gets the currently selected tab.
        /// </summary>
        public DockToolViewModel? Selected { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether a <see cref="TabStrip"/> for switching tabs should be displayed.
        /// </summary>
        public Boolean ShouldShowTabStrip => this.Tabs.Count >= 2;

        /// <summary>
        /// Called when the item collection has changes.
        /// </summary>
        /// <param name="value">The <see cref="ObservableCollection{T}"/> that changed.</param>
        partial void OnTabsChanged(ObservableCollection<DockToolViewModel> value)
        {
            if (this.tabsHandler != null && this.tabs != null)
            {
                this.tabs.CollectionChanged -= this.tabsHandler;
            }

            this.tabsHandler = (_, _) =>
            {
                this.OnPropertyChanged(nameof(this.HasTabs));
                this.OnPropertyChanged(nameof(this.ShouldShowTabStrip));
            };

            value.CollectionChanged += this.tabsHandler;

            this.OnPropertyChanged(nameof(this.HasTabs));
            this.OnPropertyChanged(nameof(this.PinnedTabs));
            this.OnPropertyChanged(nameof(this.ShouldShowTabStrip));
        }

        /// <summary>Handles chanes to the tabs collection.</summary>
        /// <param name="oldValue">The tabs previously in the collection.</param>
        /// <param name="newValue">The tabs now in the collection.</param>
        partial void OnTabsChanged(ObservableCollection<DockToolViewModel>? oldValue, ObservableCollection<DockToolViewModel>? newValue)
        {
            if (oldValue != null)
            {
                oldValue.CollectionChanged -= this.OnTabsCollectionChanged;

                foreach (DockToolViewModel tab in oldValue)
                {
                    this.DetachPinnedHandler(tab);
                }
            }

            if (newValue != null)
            {
                newValue.CollectionChanged += this.OnTabsCollectionChanged;

                foreach (DockToolViewModel tab in newValue)
                {
                    this.AttachPinnedHandler(tab);
                }
            }

            this.OnPropertyChanged(nameof(PinnedTabs));
        }

        /// <summary>Called when the entire collection is being replaced.</summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventArgs">The <see cref="NotifyCollectionChangedEventArgs"/> for the event.</param>
        private void OnTabsCollectionChanged(Object? sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            if (eventArgs.OldItems != null)
            {
                foreach (DockToolViewModel tab in eventArgs.OldItems)
                {
                    this.DetachPinnedHandler(tab);
                }
            }

            if (eventArgs.NewItems != null)
            {
                foreach (DockToolViewModel tab in eventArgs.NewItems)
                {
                    this.AttachPinnedHandler(tab);
                }
            }

            this.OnPropertyChanged(nameof(this.PinnedTabs));
        }

        /// <summary>Adds a handler for a <see cref="DockToolViewModel"/> that is being added.</summary>
        /// <param name="tab">The <see cref="DockToolViewModel"/> that is being added.</param>
        private void AttachPinnedHandler(DockToolViewModel tab)
        {
            void Handler(Object? s, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(DockToolViewModel.IsPinned))
                {
                    this.OnPropertyChanged(nameof(this.PinnedTabs));
                }
            }

            ((INotifyPropertyChanged)tab).PropertyChanged += Handler;
            this.pinnedHandlers[tab] = Handler;
        }

        /// <summary>Removes a handler for a <see cref="DockToolViewModel"/> that is going away.</summary>
        /// <param name="tab">The <see cref="DockToolViewModel"/> that is going away.</param>
        private void DetachPinnedHandler(DockToolViewModel tab)
        {
            if (this.pinnedHandlers.TryGetValue(tab, out PropertyChangedEventHandler? handler))
            {
                ((INotifyPropertyChanged)tab).PropertyChanged -= handler;
                _ = this.pinnedHandlers.Remove(tab);
            }
        }
    }
}
