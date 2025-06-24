// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Meringue.Avalonia.Dock.ViewModels;

namespace Meringue.Avalonia.Dock.Layout.Internal
{
    /// <summary>
    /// Applies a serialized <see cref="DockLayout"/> structure to a <see cref="DockLayoutRootViewModel"/>.
    /// </summary>
    public static class DockLayoutApplier
    {
        /// <summary>
        /// Builds a view model for the provided <paramref name="layout"/>.
        /// </summary>
        /// <param name="layout">The layout data.</param>
        /// <returns>The view model built.</returns>
        public static DockHostRootViewModel BuildViewModel(DockLayout layout)
        {
            ArgumentNullException.ThrowIfNull(layout);
            ArgumentNullException.ThrowIfNull(layout.RootNode);

            // Validate version
            if (layout.MajorVersion != 1)
            {
                throw new NotSupportedException($"Unsupported layout version {layout.MajorVersion}.{layout.MinorVersion}");
            }

            DockNodeViewModel root = BuildNode(layout.RootNode);
            return new DockHostRootViewModel(root);
        }

        /// <summary>
        /// Recurively builds a <see cref="DockNodeViewModel"/> from a <see cref="DockLayoutNode"/>.
        /// </summary>
        /// <param name="layoutNode">The <see cref="DockLayoutNode"/> to be converted to a <see cref="DockNodeViewModel"/>.</param>
        /// <returns>The <see cref="DockNodeViewModel"/> that was built.</returns>
        private static DockNodeViewModel BuildNode(DockLayoutNode layoutNode)
        {
            ArgumentNullException.ThrowIfNull(layoutNode);
            DockNodeViewModel constructedBuildModel;

            if (layoutNode is DockLayoutSplit splitLayoutNode)
            {
                DockSplitNodeViewModel split = new()
                {
                    Id = layoutNode.Id,
                    Orientation = splitLayoutNode.Orientation,
                    // TODO: Convert both to just be List<Double> instead.
                    Sizes = new ObservableCollection<Double>(splitLayoutNode.Sizes ?? []),
                };

                if (splitLayoutNode.Children?.Count > 0)
                {
                    // Split node
                    foreach (DockLayoutNode child in splitLayoutNode.Children)
                    {
                        split.Children.Add(BuildNode(child));
                    }
                }

                constructedBuildModel = split;
            }
            else if (layoutNode is DockLayoutTab tabLayoutNode)
            {
                // Tab node
                DockTabNodeViewModel tab = new()
                {
                    Id = layoutNode.Id,
                };

                foreach (DockLayoutTool tool in tabLayoutNode.Tools ?? Enumerable.Empty<DockLayoutTool>())
                {
                    DockToolViewModel toolVm = new()
                    {
                        Id = tool.Id,
                        Header = tool.Header ?? "Loading...",
                        IsPinned = tool.IsPinned,
                        Context = new TextBlock() { Text = "Loading..." }, // Will be attached via CreateOrUpdateTool later
                    };

                    tab.Tabs.Add(toolVm);

                    // Optionally mark the active tool
                    if (tabLayoutNode.SelectedId != null && tabLayoutNode.SelectedId == tool.Id)
                    {
                        tab.Selected = toolVm;
                    }
                }

                constructedBuildModel = tab;
            }
            else
            {
                throw new ArgumentException("Provided node could not be parsed.", nameof(layoutNode));
            }

            return constructedBuildModel;
        }
    }
}
