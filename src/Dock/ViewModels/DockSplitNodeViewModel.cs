// Copyright (C) Scott Kupec. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia.Layout;
using CommunityToolkit.Mvvm.ComponentModel;
using Meringue.Avalonia.Dock.Controls;

namespace Meringue.Avalonia.Dock.ViewModels
{
    /// <summary>
    /// Defines the view model for use with <see cref="DockSplitPanel"/>.
    /// </summary>
    public partial class DockSplitNodeViewModel : DockNodeViewModel
    {
        /// <summary>
        /// Gets the child controls that are to be presented.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<DockNodeViewModel> children = [];

        /// <summary>
        /// Defines the <see cref="Orientation"/> used when multiple children are present.
        /// </summary>
        /// <remarks>
        /// The orientation is the reverse of the splitter direction. Horizontal orientation
        /// uses vertical splitters and veritical orientation uses horizontal splitters.
        /// </remarks>
        [ObservableProperty]
        private Orientation orientation;

        /// <summary>
        /// Gets the size currently used for each child in <see cref="children"/>.
        /// </summary>
        /// <remarks>
        /// The sum of all sizes cannot exceed 1.0, but may be less than 1.0 since we don't
        /// have to track tools that are all allocated equally.
        /// </remarks>
        [ObservableProperty]
        private ObservableCollection<Double> sizes = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="DockSplitNodeViewModel"/> class.
        /// </summary>
        public DockSplitNodeViewModel()
        {
            this.children.CollectionChanged += this.OnChildrenCollectionChanged;

            // Optional: subscribe to existing items if any
            foreach (DockNodeViewModel child in this.children)
            {
                child.PropertyChanged += this.OnChildPropertyChanged;
            }
        }

        /// <summary>
        /// Called when the <see cref="children"/> collection has changes.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventArgs">The <see cref="NotifyCollectionChangedEventArgs"/> for the event.</param>
        private void OnChildrenCollectionChanged(Object? sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            if (eventArgs.NewItems != null)
            {
                foreach (DockNodeViewModel newChild in eventArgs.NewItems)
                {
                    newChild.PropertyChanged += this.OnChildPropertyChanged;
                }
            }

            if (eventArgs.OldItems != null)
            {
                foreach (DockNodeViewModel oldChild in eventArgs.OldItems)
                {
                    oldChild.PropertyChanged -= this.OnChildPropertyChanged;
                }
            }
        }

        /// <summary>
        /// Called when a member of the <see cref="children"/> collection has changes.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventArgs">The <see cref="NotifyCollectionChangedEventArgs"/> for the event.</param>
        private void OnChildPropertyChanged(Object? sender, PropertyChangedEventArgs eventArgs)
        {
            // Handle the property change here
            DockNodeViewModel? changedChild = sender as DockNodeViewModel;
            System.Diagnostics.Debug.WriteLine($"Child changed: {changedChild}, Property: {eventArgs.PropertyName}");
        }
    }
}
