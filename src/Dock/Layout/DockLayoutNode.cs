// Copyright (C) Scott Kupec. All rights reserved.

using System;
using System.Text.Json.Serialization;

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
}
