// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Meringue.Avalonia.Dock.ViewModels;

namespace Meringue.Avalonia.Dock.Controls
{
    /// <summary>
    /// A dock panel host.
    /// </summary>
    public class DockHostRoot : ContentControl
    {
        /// <summary>
        /// Defines the style property for the <see cref="ShouldShowUnpinnedTabs"/> member.
        /// </summary>
        public static readonly StyledProperty<Boolean> ShouldShowUnpinnedTabsProperty =
            AvaloniaProperty.Register<DockHostRoot, Boolean>(
                nameof(ShouldShowUnpinnedTabs));

        /// <summary>
        /// Defines the style property for the <see cref="RootNode"/> member.
        /// </summary>
        public static readonly StyledProperty<DockHostRootViewModel?> RootNodeProperty =
            AvaloniaProperty.Register<DockHostRoot, DockHostRootViewModel?>(nameof(RootNode));

        /// <summary>
        /// Defines the style property for the <see cref="UnpinnedTabs"/> member.
        /// </summary>
        public static readonly StyledProperty<IList<DockToolViewModel>> UnpinnedTabsProperty =
            AvaloniaProperty.Register<DockHostRoot, IList<DockToolViewModel>>(nameof(UnpinnedTabs));

        /// <summary>
        /// Initializes a new instance of the <see cref="DockHostRoot"/> class.
        /// </summary>
        public DockHostRoot()
        {
            _ = this.Bind(ContentProperty, this.GetObservable(RootNodeProperty));
        }

        /// <summary>
        /// Gets or sets a value indicating whether the corresponding <see cref="TabStrip"/> should
        /// be displayed.
        /// </summary>
        public Boolean ShouldShowUnpinnedTabs
        {
            get => this.GetValue(ShouldShowUnpinnedTabsProperty);
            set => this.SetValue(ShouldShowUnpinnedTabsProperty, value);
        }

        /// <summary>
        /// Gets or sets the root <see cref="DockNodeViewModel"/>.
        /// </summary>
        public DockHostRootViewModel? RootNode
        {
            get => this.GetValue(RootNodeProperty);
            set => this.SetValue(RootNodeProperty, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="DockToolViewModel"/> that are not currently pinned.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Bound property")]
        public IList<DockToolViewModel> UnpinnedTabs
        {
            get => this.GetValue(UnpinnedTabsProperty);
            set => this.SetValue(UnpinnedTabsProperty, value);
        }

        /// <summary>
        /// Gets or sets the stub for managing <see cref="UnpinnedTabs"/>.
        /// </summary>
        private DockToolStub? UnpinnedTabsStub { get; set; }

        /// <inheritdoc/>
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            if (e is not null)
            {
                if (e.NameScope.Find("PART_UnpinnedStub") is DockToolStub stub)
                {
                    this.UnpinnedTabsStub = stub;
                    this.UnpinnedTabsStub.Tabs = this.UnpinnedTabs;
                }
            }

            if (this.DataContext is DockHostRootViewModel vm)
            {
                DockContext.SetRootNode(this, vm);
            }
        }

        /// <inheritdoc/>
        protected override void OnDataContextChanged(EventArgs e)
        {
            base.OnDataContextChanged(e);

            if (this.DataContext is DockHostRootViewModel viewModel)
            {
                // TODO: De-duplicate with OnRootNodeChanged.
                DockContext.SetRootNode(this, viewModel);

                _ = this.Bind(
                    ShouldShowUnpinnedTabsProperty,
                    new Binding(nameof(viewModel.ShouldShowUnpinnedTabs)) { Source = viewModel });

                _ = this.Bind(
                    UnpinnedTabsProperty,
                    new Binding(nameof(viewModel.UnpinnedTabs)) { Source = viewModel });

                if (this.UnpinnedTabsStub is not null)
                {
                    this.UnpinnedTabsStub.Tabs = this.UnpinnedTabs;
                }
            }
        }
    }
}
