// Copyright (C) Meringue Project Team. All rights reserved.

using System;

namespace Meringue.Avalonia.Dock.Layout
{
    /// <summary>
    /// Represents a persisted tool entry in a layout.
    /// </summary>
    public sealed class DockLayoutTool
    {
        /// <summary>
        /// Gets or sets the unique ID of the tool.
        /// </summary>
        public String Id { get; set; } = String.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the tool is pinned.
        /// </summary>
        public Boolean CanClose { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the tool is pinned.
        /// </summary>
        public Boolean CanPin { get; set; } = true;

        /// <summary>
        /// Gets or sets the display header for the tool.
        /// </summary>
        public String? Header { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tool is pinned.
        /// </summary>
        public Boolean IsClosed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tool is pinned.
        /// </summary>
        public Boolean IsPinned { get; set; }

        /// <summary>
        /// Gets or sets the title for the tool.
        /// </summary>
        public String? Title { get; set; }
    }
}
