// Copyright (C) Meringue Project Team. All rights reserved.

using System;

namespace Meringue.Avalonia.Dock.Layout
{
    /// <summary>
    /// Represents a serializable layout of the entire docking system.
    /// </summary>
    public sealed class DockLayout
    {
        /// <summary>
        /// Gets or sets the major version of the layout format.
        /// </summary>
        public Int32 MajorVersion { get; set; } = 1;

        /// <summary>
        /// Gets or sets the minor version of the layout format.
        /// </summary>
        public Int32 MinorVersion { get; set; } = 0;

        /// <summary>
        /// Gets or sets the root node of the layout tree.
        /// </summary>
        public DockLayoutNode? RootNode { get; set; }
    }
}
