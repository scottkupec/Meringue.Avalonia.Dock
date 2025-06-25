// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using System.Collections.Generic;
using Avalonia.Layout;

namespace Meringue.Avalonia.Dock.Layout
{
    /// <summary>
    /// Represents a split container in the layout.
    /// </summary>
    public sealed class DockLayoutSplit : DockLayoutNode
    {
        /// <summary>
        /// Gets or sets the orientation of the split.
        /// </summary>
        public Orientation Orientation { get; set; }

        /// <summary>
        /// Gets or sets the list of child nodes within the split.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Settable for serialization.")]
        public List<DockLayoutNode> Children { get; set; } = [];

        /// <summary>
        /// Gets or sets the list of child nodes within the split.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Settable for serialization.")]
        public List<Double> Sizes { get; set; } = [];
    }
}
