// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using System.Collections.Generic;

namespace Meringue.Avalonia.Dock.Layout
{
    /// <summary>
    /// Represents a tab group in the layout.
    /// </summary>
    public sealed class DockLayoutTab : DockLayoutNode
    {
        /// <summary>
        /// Gets or sets the list of tools (by ID) in the tab group, in order.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Settable to simplify (de)serialization code.")]
        public List<DockLayoutTool> Tools { get; set; } = [];

        /// <summary>
        /// Gets or sets the ID of the currently selected tab, if any.
        /// </summary>
        public String? SelectedId { get; set; }
    }
}
