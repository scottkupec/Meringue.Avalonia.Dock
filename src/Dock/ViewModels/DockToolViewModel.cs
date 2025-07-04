// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Meringue.Avalonia.Dock.Controls;

namespace Meringue.Avalonia.Dock.ViewModels
{
    /// <summary>
    /// Defines the view model for use with <see cref="DockTool"/>.
    /// </summary>
    public partial class DockToolViewModel : ObservableObject
    {
        /// <summary>
        /// Gets or sets a value indicating whether the tool is currently closed.
        /// </summary>
        [ObservableProperty]
        private Boolean isClosed;

        /// <summary>
        /// Gets or sets a value indicating whether the tool is currently being hovered over by a pointer.
        /// </summary>
        [ObservableProperty]
        private Boolean isHovered;

        /// <summary>
        /// Gets or sets a value indicating whether the tool is currently pinned.
        /// </summary>
        [ObservableProperty]
        private Boolean isPinned = true;

        /// <summary>
        /// Gets or sets a value indicating whether the tool is currently selected.
        /// </summary>
        [ObservableProperty]
        private Boolean isSelected;

        /// <summary>
        /// Gets the title for the tool.
        /// </summary>
        [ObservableProperty]
        private String title = String.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the tool can be closed.
        /// </summary>
        public Boolean CanClose { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the tool can be pinned.
        /// </summary>
        public Boolean CanPin { get; set; } = true;

        /// <summary>
        /// Gets or sets the content of the tool.
        /// </summary>
        public Object? Context { get; set; }

        /// <summary>
        /// Gets or sets the title of the tool.
        /// </summary>
        public String Header { get; set; } = "Untitled";

        /// <summary>
        /// Gets or sets the id of the tool.
        /// </summary>
        public String Id { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// Gets a value indicating whether the tool is currently visible.
        /// </summary>
        public Boolean IsVisible => !this.IsClosed && (this.IsPinned || this.IsHovered);

        /// <inheritdoc/>
        partial void OnIsClosedChanged(Boolean oldValue, Boolean newValue) => OnPropertyChanged(nameof(this.IsClosed));

        /// <inheritdoc/>
        partial void OnIsPinnedChanged(Boolean oldValue, Boolean newValue) => OnPropertyChanged(nameof(this.IsVisible));

        /// <inheritdoc/>
        partial void OnIsHoveredChanged(Boolean oldValue, Boolean newValue) => OnPropertyChanged(nameof(this.IsVisible));
    }
}
