// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Meringue.Avalonia.Dock.ViewModels
{
    /// <summary>
    /// A common base for view models that can be used with docking.
    /// </summary>
    public abstract class DockNodeViewModel : ObservableObject
    {
        /// <summary>Gets or sets the id of the corresponding node.</summary>
        public String? Id { get; set; }
    }
}
