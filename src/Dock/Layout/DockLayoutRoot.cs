// Copyright (C) Meringue Project Team. All rights reserved.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Meringue.Avalonia.Dock.Controls;
using Meringue.Avalonia.Dock.ViewModels;

namespace Meringue.Avalonia.Dock.Layout
{
    /// <summary>
    /// A dock panel host.
    /// </summary>
    public class DockLayoutRoot : ContentControl
    {
        /// <summary>
        /// Defines the style property for the <see cref="DockHostRoot"/> member.
        /// </summary>
        public static readonly StyledProperty<DockHostRootViewModel> HostRootProperty =
            AvaloniaProperty.Register<DockLayoutRoot, DockHostRootViewModel>(
                nameof(HostRoot));

        /// <summary>
        /// Defines the style property for the <see cref="DockHostRoot"/> member.
        /// </summary>
        public static readonly StyledProperty<DockInsertPolicy> InsertPolicyProperty =
            AvaloniaProperty.Register<DockLayoutRoot, DockInsertPolicy>(
                nameof(InsertPolicy),
                defaultValue: DockInsertPolicy.CreateLast);

        /// <summary>
        /// Defines the style property for the <see cref="DockHostRoot"/> member.
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
            // Provide default instances only if not set explicitly.
            this.HostRoot ??= new DockHostRootViewModel(new DockSplitNodeViewModel());
            this.LayoutManager ??= new DefaultDockLayoutManager(); // Replace with your actual manager implementation
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
        /// Gets or sets a value indicating whether the corresponding <see cref="TabStrip"/> should
        /// be displayed.
        /// </summary>
        internal DockHostRootViewModel HostRoot
        {
            get => this.GetValue(HostRootProperty);
            set => this.SetValue(HostRootProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the corresponding <see cref="TabStrip"/> should
        /// be displayed.
        /// </summary>
        internal IDockLayoutManager LayoutManager
        {
            get => this.GetValue(LayoutManagerProperty);
            set => this.SetValue(LayoutManagerProperty, value);
        }
    }
}
