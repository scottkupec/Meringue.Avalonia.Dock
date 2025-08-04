// Copyright (C) Scott Kupec. All rights reserved.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Meringue.Avalonia.Dock.ViewModels;

namespace Meringue.Avalonia.Dock.Layout
{
    /// <summary>
    /// Provides services to convert between a <see cref="DockLayout"/> and a <see cref="DockHostRootViewModel"/>.
    /// </summary>
    internal static class DockLayoutConverter
    {
        /// <summary>
        /// Builds a <see cref="DockLayout"/> for the provided <see cref="DockHostRootViewModel"/>.
        /// </summary>
        /// <param name="hostRoot">The <see cref="DockHostRootViewModel"/> to be used.</param>
        /// <returns>The new <see cref="DockLayout"/>.</returns>
        public static DockLayout BuildLayout(DockHostRootViewModel hostRoot)
        {
            TargetFrameworkHelper.ThrowIfArgumentNull(hostRoot);

            return new DockLayout
            {
                MajorVersion = 1,
                MinorVersion = 0,
                RootNode = BuildLayoutNode(hostRoot.HostRoot),
            };
        }

        /// <summary>
        /// Builds a view model for the provided <paramref name="layout"/>.
        /// </summary>
        /// <param name="layout">The layout data.</param>
        /// <returns>The view model built.</returns>
        public static DockHostRootViewModel BuildViewModel(DockLayout layout)
        {
            TargetFrameworkHelper.ThrowIfArgumentNull(layout);
            TargetFrameworkHelper.ThrowIfArgumentNull(layout.RootNode);

            // Validate version
            if (layout.MajorVersion != 1)
            {
                throw new NotSupportedException($"Unsupported layout version {layout.MajorVersion}.{layout.MinorVersion}");
            }

            DockNodeViewModel root = BuildViewModelNode(layout.RootNode);
            return new DockHostRootViewModel(root);
        }

        /// <summary>
        /// Builds a <see cref="DockLayoutNode"/> for the provided <see cref="DockNodeViewModel"/>.
        /// </summary>
        /// <param name="node">The <see cref="DockNodeViewModel"/> to be used.</param>
        /// <returns>The new <see cref="DockLayoutNode"/>.</returns>
        private static DockLayoutNode BuildLayoutNode(DockNodeViewModel node)
        {
            return node switch
            {
                DockSplitNodeViewModel split =>
                    new DockLayoutSplit
                    {
                        Id = split.Id,
                        Orientation = split.Orientation,
                        Children = [.. split.Children.Select(BuildLayoutNode)],
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
                                CanClose = t.CanClose,
                                CanPin = t.CanPin,
                                IsClosed = t.IsClosed,
                                IsPinned = t.IsPinned,
                                Title = t.Title,
                            })],
                    },

                _ => throw new NotSupportedException($"Unknown node type: {node.GetType().Name}"),
            };
        }

        /// <summary>
        /// Recurively builds a <see cref="DockNodeViewModel"/> from a <see cref="DockLayoutNode"/>.
        /// </summary>
        /// <param name="layoutNode">The <see cref="DockLayoutNode"/> to be converted to a <see cref="DockNodeViewModel"/>.</param>
        /// <returns>The <see cref="DockNodeViewModel"/> that was built.</returns>
        private static DockNodeViewModel BuildViewModelNode(DockLayoutNode layoutNode)
        {
            TargetFrameworkHelper.ThrowIfArgumentNull(layoutNode);
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
                        split.Children.Add(BuildViewModelNode(child));
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
                        CanClose = tool.CanClose,
                        CanPin = tool.CanPin,
                        IsClosed = tool.IsClosed,
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
