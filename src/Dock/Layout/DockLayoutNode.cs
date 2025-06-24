// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Avalonia.Layout;

namespace Meringue.Avalonia.Dock.Layout
{
    /// <summary>
    /// A base class for nodes in the persisted layout tree.
    /// </summary>
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(DockLayoutSplit), "split")]
    [JsonDerivedType(typeof(DockLayoutTab), "tab")]
    public abstract class DockLayoutNode
    {
        /// <summary>
        /// Gets or sets the unique ID of the node.
        /// </summary>
        public String? Id { get; set; }
    }

    /// <summary>
    /// Represents a split container in the layout.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Small, closely related classes.")]
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

    /// <summary>
    /// Represents a tab group in the layout.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Small, closely related classes.")]
    public sealed class DockLayoutTab : DockLayoutNode
    {
        /// <summary>
        /// Gets or sets the list of tools (by ID) in the tab group, in order.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Settable for serialization.")]
        public List<DockLayoutTool> Tools { get; set; } = [];

        /// <summary>
        /// Gets or sets the ID of the currently selected tab, if any.
        /// </summary>
        public String? SelectedId { get; set; }
    }
}
