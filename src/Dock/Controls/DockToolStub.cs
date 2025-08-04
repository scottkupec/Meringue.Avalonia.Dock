// Copyright (C) Scott Kupec. All rights reserved.

using System;
using System.Collections.Generic;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.Primitives;
using CommunityToolkit.Mvvm.Input;
using Meringue.Avalonia.Dock.ViewModels;
using DockLocation = Avalonia.Controls.Dock;

namespace Meringue.Avalonia.Dock.Controls
{
    /// <summary>
    /// Displays stub headers for unpinned docked tools along a <see cref="DockHostRoot"/> edge.
    /// </summary>
    internal class DockToolStub : TemplatedControl
    {
        /// <summary>
        /// Defines the style property for the <see cref="Tabs"/> member.
        /// </summary>
        public static readonly StyledProperty<IEnumerable<DockToolViewModel>?> TabsProperty =
            AvaloniaProperty.Register<DockToolStub, IEnumerable<DockToolViewModel>?>(nameof(Tabs));

        /// <summary>
        /// Defines the style property for the <see cref="DockEdge"/> member.
        /// </summary>
        public static readonly StyledProperty<DockLocation?> DockEdgeProperty =
            AvaloniaProperty.Register<DockToolStub, DockLocation?>(nameof(DockEdge));

        /// <summary>
        /// Defines the style property for the <see cref="ShouldRotate"/> member.
        /// </summary>
        public static readonly StyledProperty<Boolean?> ShouldRotateProperty =
            AvaloniaProperty.Register<DockToolStub, Boolean?>(nameof(ShouldRotate));

        /// <summary>
        /// Gets or sets the screen edge to dock the tool stubs to.
        /// </summary>
        public DockLocation? DockEdge
        {
            get => this.GetValue(DockEdgeProperty);
            set => this.SetValue(DockEdgeProperty, value);
        }

        /// <summary>
        /// Gets a value indicating whether the rotation setting is enabled based on edge and override.
        /// </summary>
        // TODO: This probably is no longer attached correctly.  Review and restore if needed.  This is
        //       intended to allow callers to override the default of rotating based on the dock location.
        public Boolean EffectiveRotation =>
            this.ShouldRotate ?? this.DockEdge is DockLocation.Left or DockLocation.Right;

        /// <summary>
        /// Gets or sets whether to rotate tab headers. If null, defaults to true for Left/Right edges.
        /// </summary>
        public Boolean? ShouldRotate
        {
            get => this.GetValue(ShouldRotateProperty);
            set => this.SetValue(ShouldRotateProperty, value);
        }

        /// <summary>
        /// Gets or sets the collection of unpinned dock tool view models to display as stubs.
        /// </summary>
        public IEnumerable<DockToolViewModel>? Tabs
        {
            get => this.GetValue(TabsProperty);
            set => this.SetValue(TabsProperty, value);
        }

        /// <summary>
        /// Gets or sets the command executed when the stub becomes selected (e.g. the pane should be displayed).
        /// </summary>
        public ICommand TabSelectedCommand { get; set; } = new RelayCommand<DockToolViewModel>(
            tab =>
            {
                tab!.IsPinned = true;
            });
    }
}
