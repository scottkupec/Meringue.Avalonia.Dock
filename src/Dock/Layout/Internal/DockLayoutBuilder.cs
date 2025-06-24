// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using System.Linq;
using Meringue.Avalonia.Dock.ViewModels;

namespace Meringue.Avalonia.Dock.Layout.Internal
{
    /// <summary>
    /// Converts a live DockHostRootViewModel into a serializable DockLayout.
    /// </summary>
    public static class DockLayoutBuilder
    {
        /// <summary>
        /// Builds a <see cref="DockLayout"/> for the provided <see cref="DockHostRootViewModel"/>.
        /// </summary>
        /// <param name="hostRoot">The <see cref="DockHostRootViewModel"/> to be used.</param>
        /// <returns>The new <see cref="DockLayout"/>.</returns>
        public static DockLayout Build(DockHostRootViewModel hostRoot)
        {
            ArgumentNullException.ThrowIfNull(hostRoot);

            return new DockLayout
            {
                MajorVersion = 1,
                MinorVersion = 0,
                RootNode = BuildNode(hostRoot.HostRoot),
            };
        }

        /// <summary>
        /// Builds a <see cref="DockLayoutNode"/> for the provided <see cref="DockNodeViewModel"/>.
        /// </summary>
        /// <param name="node">The <see cref="DockNodeViewModel"/> to be used.</param>
        /// <returns>The new <see cref="DockLayoutNode"/>.</returns>
        private static DockLayoutNode BuildNode(DockNodeViewModel node)
        {
            return node switch
            {
                DockSplitNodeViewModel split =>
                    new DockLayoutSplit
                    {
                        Id = split.Id,
                        Orientation = split.Orientation,
                        Children = [.. split.Children.Select(BuildNode)],
                        Sizes = [.. split.Sizes],
                    },

                DockTabNodeViewModel tab =>
                    new DockLayoutTab
                    {
                        Id = tab.Id,
                        SelectedId = tab.Selected?.Id,
                        Tools = [.. tab.Tabs.Select(
                            t => new DockLayoutTool
                            {
                                Id = t.Id.ToString(),
                                Header = t.Header,
                                IsPinned = t.IsPinned,
                            })],
                    },

                _ => throw new NotSupportedException($"Unknown node type: {node.GetType().Name}"),
            };
        }
    }
}
