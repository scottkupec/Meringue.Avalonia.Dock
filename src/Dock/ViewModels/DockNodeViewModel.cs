// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Meringue.Avalonia.Dock.Controls;

namespace Meringue.Avalonia.Dock.ViewModels
{
    /// <summary>
    /// A common base for view models that can participate directly in a <see cref="DockHost"/> control tree.
    /// </summary>
    // We'd call these panels but DockPanel is already an Avalonia control and that would be
    // confusing.
    // "Directly" is an important word in the summary. Other control types may (and will!) be present as
    // children of controls in dock tree.
    public abstract class DockNodeViewModel : ObservableObject
    {
        /// <summary>Gets or sets the id of the corresponding node.</summary>
        public String? Id { get; set; }
    }
}
