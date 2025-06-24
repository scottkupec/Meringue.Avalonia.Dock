// Copyright (C) Meringue Project Team. All rights reserved.

using Avalonia;
using Avalonia.Controls;
using Meringue.Avalonia.Dock.ViewModels;

namespace Meringue.Avalonia.Dock.Controls
{
    /// <summary>
    /// Context class for use with the docking controls.
    /// </summary>
    public static class DockContext
    {
        /// <summary>
        /// Defines the name of f the <see cref="AttachedProperty{TValue}"/> used for the root node.
        /// </summary>
        public static readonly AttachedProperty<DockHostRootViewModel?> RootNodeProperty =
            AvaloniaProperty.RegisterAttached<Control, Control, DockHostRootViewModel?>("RootNode");

        /// <summary>
        /// Sets the root <see cref="DockHostRootViewModel"/> for the given <see cref="Control"/>.
        /// </summary>
        /// <param name="element">The <see cref="Control"/> the root is to be set for.</param>
        /// <param name="value">The root to set.</param>
        public static void SetRootNode(Control element, DockHostRootViewModel? value)
        {
            if (element is not null)
            {
                _ = element.SetValue(RootNodeProperty, value);
            }
        }

        /// <summary>
        /// Gets the root <see cref="DockHostRootViewModel"/> for the given <see cref="Control"/>.
        /// </summary>
        /// <param name="element">The <see cref="Control"/> for which the root is being retrieved.</param>
        /// <returns>The <see cref="DockHostRootViewModel"/> found or null if no such root exists.</returns>
        public static DockHostRootViewModel? GetRootNode(Control element)
        {
            return element?.GetValue(RootNodeProperty);
        }

        /// <summary>
        /// Walks up the visual tree to find the nearest DockHostRootViewModel, if not set on this control.
        /// </summary>
        /// <param name="startingFrom">The <see cref="Control"/> to start from.</param>
        /// <returns>The root <see cref="DockHostRootViewModel"/> or null if not such root exists.</returns>
        public static DockHostRootViewModel? FindRootNode(Control? startingFrom)
        {
            while (startingFrom is not null)
            {
                DockHostRootViewModel? root = GetRootNode(startingFrom);

                if (root != null)
                {
                    return root;
                }

                ////StyledElement? parent = startingFrom.Parent;
                ////System.Diagnostics.Debug.WriteLine($"Parent of {startingFrom} is {parent} with data context {parent?.DataContext}.");
                startingFrom = startingFrom.Parent as Control;
            }

            return null;
        }
    }
}
