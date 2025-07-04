// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using System.Linq;

namespace Meringue.Avalonia.Dock.ViewModels
{
    /// <summary>
    /// Extension methods for searching within a Dock layout tree.
    /// </summary>
    public static class DockNodeViewModelExtensions
    {
        /// <summary>
        /// Recursively searches a <see cref="DockNodeViewModel"/> tree for a <see cref="DockToolViewModel"/> with the given ID.
        /// </summary>
        /// <param name="rootNode">The root node to search.</param>
        /// <param name="id">The ID of the tool to find.</param>
        /// <returns>The matching <see cref="DockToolViewModel"/>, or null if not found.</returns>
        public static DockToolViewModel? FindTool(this DockNodeViewModel rootNode, String id)
        {
            TargetFrameworkHelper.ThrowIfArgumentNull(rootNode);

            if (rootNode is DockTabNodeViewModel tabNode)
            {
                return tabNode.Tabs.FirstOrDefault(tool => tool.Id == id);
            }
            else if (rootNode is DockSplitNodeViewModel splitNode)
            {
                foreach (DockNodeViewModel child in splitNode.Children)
                {
                    DockToolViewModel? found = child.FindTool(id);
                    if (found != null)
                    {
                        return found;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Recursively searches a <see cref="DockNodeViewModel"/> tree for a <see cref="DockNodeViewModel"/> with the given ID.
        /// </summary>
        /// <param name="rootNode">The root node to search.</param>
        /// <param name="id">The ID of the node to find.</param>
        /// <returns>The matching <see cref="DockNodeViewModel"/>, or null if not found.</returns>
        public static DockNodeViewModel? FindNode(this DockNodeViewModel rootNode, String id)
        {
            TargetFrameworkHelper.ThrowIfArgumentNull(rootNode);

            if (rootNode.Id == id)
            {
                return rootNode;
            }

            if (rootNode is DockSplitNodeViewModel splitNode)
            {
                foreach (DockNodeViewModel child in splitNode.Children)
                {
                    DockNodeViewModel? found = child.FindNode(id);
                    if (found != null)
                    {
                        return found;
                    }
                }
            }

            return null;
        }
    }
}
