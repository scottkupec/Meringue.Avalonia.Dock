// Copyright (C) Scott Kupec. All rights reserved.

using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace HelloCustomControl.ViewModels
{
    /// <summary>
    /// A basic view model for testing DockTool content.
    /// </summary>
    public partial class SimpleDockToolViewModel : ObservableObject
    {
        /// <summary>Provides the control's text.</summary>
        [ObservableProperty]
        private String text = "Hello, dock!";
    }
}
