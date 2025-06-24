// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Meringue.Avalonia.Dock.ViewModels
{
    /// <summary>
    /// View model for an individual docked tool tab.
    /// </summary>
    public partial class DockToolViewModel : ObservableObject
    {
        /// <summary>
        /// Gets or sets a value indicating whether the tools is currently being hovered over.
        /// </summary>
        [ObservableProperty]
        private Boolean isHovered;

        /// <summary>
        /// Gets or sets a value indicating whether the tool is currently pinned.
        /// </summary>
        [ObservableProperty]
        private Boolean isPinned = true;

        /// <summary>
        /// Gets or sets the id of the instance.
        /// </summary>
        public String Id { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// Gets a value indicating whether the tool is currently visible.
        /// </summary>
        public Boolean IsVisible => this.IsPinned || this.IsHovered;

        /// <summary>
        /// Gets or sets the content of the tool tab.
        /// </summary>
        public Object? Context { get; set; }

        /// <summary>
        /// Gets or sets the title of the tool tab.
        /// </summary>
        public String Header { get; set; } = "Untitled";

        /// <inheritdoc/>
        partial void OnIsPinnedChanged(Boolean oldValue, Boolean newValue) => OnPropertyChanged(nameof(this.IsVisible));

        /// <inheritdoc/>
        partial void OnIsHoveredChanged(Boolean oldValue, Boolean newValue) => OnPropertyChanged(nameof(this.IsVisible));
    }
}
