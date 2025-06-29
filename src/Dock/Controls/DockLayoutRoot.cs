// Copyright (C) Meringue Project Team. All rights reserved.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Meringue.Avalonia.Dock.Layout;
using Meringue.Avalonia.Dock.ViewModels;

namespace Meringue.Avalonia.Dock.Controls
{
    /// <summary>
    /// A wrapper around <see cref="DockHostRoot"/> that provides layout management to make
    /// saving and restoring layouts easier.  Designed to support loading layouts prior
    /// to creating of the context for tools so the dock is more visually stable even while
    /// waiting for child controls to be connected to remote content.
    /// </summary>
    public class DockLayoutRoot : ContentControl
    {
        /// <summary>
        /// Defines the style property for the <see cref="HostRoot"/> member.
        /// </summary>
        public static readonly StyledProperty<DockHostRootViewModel> HostRootProperty =
            AvaloniaProperty.Register<DockLayoutRoot, DockHostRootViewModel>(
                nameof(HostRoot));

        /// <summary>
        /// Defines the style property for the <see cref="InsertPolicy"/> member.
        /// </summary>
        public static readonly StyledProperty<DockInsertPolicy> InsertPolicyProperty =
            AvaloniaProperty.Register<DockLayoutRoot, DockInsertPolicy>(
                nameof(InsertPolicy),
                defaultValue: DockInsertPolicy.CreateLast);

        /// <summary>
        /// Defines the style property for the <see cref="LayoutManager"/> member.
        /// </summary>
        public static readonly StyledProperty<IDockLayoutManager> LayoutManagerProperty =
            AvaloniaProperty.Register<DockLayoutRoot, IDockLayoutManager>(
                nameof(LayoutManager));

        /// <summary>
        /// Initializes a new instance of the <see cref="DockLayoutRoot"/> class.
        /// </summary>
        public DockLayoutRoot()
        {
            this.DataContextChanged += (_, _) =>
            {
                if (this.DataContext is DockLayoutRootViewModel viewModel)
                {
                    this.HostRoot = viewModel.HostRoot;
                    this.LayoutManager = viewModel.LayoutManager;
                    this.InsertPolicy = viewModel.InsertPolicy;
                }
            };
        }

        /// <summary>
        /// Gets or sets the <see cref="DockHostRootViewModel"/> used for associated the dock control.
        /// </summary>
        internal DockHostRootViewModel HostRoot
        {
            get => this.GetValue(HostRootProperty);
            set => this.SetValue(HostRootProperty, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="DockInsertPolicy"/> to be used.
        /// </summary>
        internal DockInsertPolicy InsertPolicy
        {
            get => this.GetValue(InsertPolicyProperty);
            set => this.SetValue(InsertPolicyProperty, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="IDockLayoutManager"/> to be used for loading and saving
        /// layouts.
        /// </summary>
        internal IDockLayoutManager LayoutManager
        {
            get => this.GetValue(LayoutManagerProperty);
            set => this.SetValue(LayoutManagerProperty, value);
        }
    }
}
