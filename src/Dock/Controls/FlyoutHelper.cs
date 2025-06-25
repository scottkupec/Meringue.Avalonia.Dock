// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace Meringue.Avalonia.Dock.Controls
{
    /// <summary>
    /// Diplays the flyout for DockToolStub.
    /// </summary>
    public static class FlyoutHelper
    {
        /// <summary>
        /// Defines the name of f the <see cref="AttachedProperty{TValue}"/> used for displaying the flyout.
        /// </summary>
        public static readonly AttachedProperty<Boolean> ShowFlyoutOnHoverProperty =
            AvaloniaProperty.RegisterAttached<Control, Control, Boolean>("ShowFlyoutOnHover");

        /// <summary>
        /// Initializes static members of the <see cref="FlyoutHelper"/> class.
        /// </summary>
        static FlyoutHelper()
        {
            _ = ShowFlyoutOnHoverProperty.Changed.Subscribe(OnShowFlyoutOnHoverChanged);
        }

        /// <summary>
        /// Enables or disables the flyout being visibile.
        /// </summary>
        /// <param name="element">The element to set the ShowFlyoutOnHoverProperty for.</param>
        /// <param name="value">The value to set.</param>
        public static void SetShowFlyoutOnHover(AvaloniaObject element, Boolean value) =>
            element?.SetValue(ShowFlyoutOnHoverProperty, value);

        /// <summary>
        /// Checks if the flyout should be displayed on hover.
        /// </summary>
        /// <param name="element">The elemnt to get the ShowFlyoutOnHoverProperty for.</param>
        /// <returns>The value of the property.</returns>
        public static Boolean GetShowFlyoutOnHover(AvaloniaObject element) =>
            element?.GetValue(ShowFlyoutOnHoverProperty) ?? false;

        /// <summary>
        /// Show the flyout.
        /// </summary>
        /// <param name="args">The arguments for the event.</param>
        private static void OnShowFlyoutOnHoverChanged(AvaloniaPropertyChangedEventArgs args)
        {
            if (args.Sender is Control control)
            {
                control.PointerEntered -= OnPointerEntered;

                if (args.NewValue is true)
                {
                    control.PointerEntered += OnPointerEntered;
                }
            }
        }

        /// <summary>
        /// Called on hovering over the control.
        /// </summary>
        /// <param name="sender">The sender of the event args.</param>
        /// <param name="eventArgs">The arguments for the event.</param>
        private static void OnPointerEntered(Object? sender, PointerEventArgs eventArgs)
        {
            if (sender is Control control)
            {
                FlyoutBase? flyout = FlyoutBase.GetAttachedFlyout(control);
                flyout?.ShowAt(control);
            }
        }
    }
}
