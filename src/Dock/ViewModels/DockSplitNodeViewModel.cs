// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using System.Collections.ObjectModel;
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
    }
}
