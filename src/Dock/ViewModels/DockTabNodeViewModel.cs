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
    /// Defines the view model for use with <see cref="DockTabPanel"/>.
    /// </summary>
    public partial class DockTabNodeViewModel : DockNodeViewModel
    {
        /// <summary>Tracks the pin handlers for current tabs.</summary>
        private readonly Dictionary<DockToolViewModel, PropertyChangedEventHandler> pinnedHandlers = [];

        /// <summary>
        /// Gets the currently selected tab.
        /// </summary>
        [ObservableProperty]
        private DockToolViewModel? selected;

        /// <summary>The list for <see cref="DockTool"/>s to be presented as tabs.</summary>
        [ObservableProperty]
        private ObservableCollection<DockToolViewModel> tabs = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="DockTabNodeViewModel"/> class.
        /// </summary>
        public DockTabNodeViewModel()
        {
            this.tabs.CollectionChanged += this.OnTabsItemsChanged;

            foreach (DockToolViewModel tab in this.tabs)
            {
                this.AttachPinnedHandler(tab);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current instance has any tabs.
        /// </summary>
        public Boolean HasTabs => this.Tabs.Count != 0;

        /// <summary>Gets the <see cref="Tabs"/> that are currently pinned.</summary>
        public IEnumerable<DockToolViewModel> PinnedTabs =>
            this.Tabs.Where(t => t.IsPinned && !t.IsClosed);

        /// <summary>
        /// Gets a value indicating whether a <see cref="TabStrip"/> for switching tabs should be displayed.
        /// </summary>
        public Boolean ShouldShowTabStrip => this.Tabs.Count >= 2;

        /// <summary>
        /// Called when the entire <see cref="Tabs"/> collection is replaced.
        /// </summary>
        /// <param name="oldValue">The tabs previously in the collection.</param>
        /// <param name="newValue">The tabs now in the collection.</param>
        partial void OnTabsChanged(ObservableCollection<DockToolViewModel>? oldValue, ObservableCollection<DockToolViewModel>? newValue)
        {
            if (ReferenceEquals(oldValue, newValue))
            {
                return;
            }

            if (oldValue != null)
            {
                oldValue.CollectionChanged -= this.OnTabsItemsChanged;

                foreach (DockToolViewModel tab in oldValue)
                {
                    this.DetachPinnedHandler(tab);
                }
            }

            if (newValue != null)
            {
                newValue.CollectionChanged += this.OnTabsItemsChanged;

                foreach (DockToolViewModel tab in newValue)
                {
                    this.AttachPinnedHandler(tab);
                }
            }

            this.OnPropertyChanged(nameof(this.HasTabs));
            this.OnPropertyChanged(nameof(this.PinnedTabs));
            this.OnPropertyChanged(nameof(this.ShouldShowTabStrip));
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

        /// <summary>
        /// Called when the <see cref="Selected"/> property changes.
        /// </summary>
        /// <param name="oldValue">The previously selected <see cref="DockToolViewModel"/>.</param>
        /// <param name="newValue">The currently selected <see cref="DockToolViewModel"/>.</param>
        partial void OnSelectedChanged(DockToolViewModel? oldValue, DockToolViewModel? newValue)
        {
            foreach (DockToolViewModel tab in this.Tabs)
            {
                tab.IsSelected = tab == newValue;
            }
        }

        /// <summary>
        /// Called when the <see cref="Tabs"/> collection is added to or removed from.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventArgs">The <see cref="NotifyCollectionChangedEventArgs"/> for the event.</param>
        private void OnTabsItemsChanged(Object? sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            // eventArgs.OldItems are items being removed (not the full old items collection).
            // eventArgs.NewItems are items being added (not the full new items collection).
            // this.Tabs has already been updated to include eventArgs.NewItems and not include eventArgs.OldItems.

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

            Boolean hadTabs = this.Tabs.Count - (eventArgs.NewItems?.Count ?? 0) + (eventArgs.OldItems?.Count ?? 0) > 0;
            if (hadTabs != this.HasTabs)
            {
                this.OnPropertyChanged(nameof(this.HasTabs));
            }

            Boolean hadTabStrip = this.Tabs.Count - (eventArgs.NewItems?.Count ?? 0) + (eventArgs.OldItems?.Count ?? 0) >= 2;
            if (hadTabStrip != this.ShouldShowTabStrip)
            {
                this.OnPropertyChanged(nameof(this.ShouldShowTabStrip));
            }

            IEnumerable<DockToolViewModel> oldPinnedRemoved = eventArgs.OldItems?.OfType<DockToolViewModel>().Where(tool => tool.IsPinned) ?? [];
            Boolean pinnedChanged = oldPinnedRemoved.Any() || (eventArgs.NewItems?.OfType<DockToolViewModel>().Any(tool => tool.IsPinned) ?? false);

            if (pinnedChanged)
            {
                this.OnPropertyChanged(nameof(this.PinnedTabs));
            }
        }
    }
}
