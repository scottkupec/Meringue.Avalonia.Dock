// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Metadata;
using CommunityToolkit.Mvvm.Input;
using Meringue.Avalonia.Dock.ViewModels;

namespace Meringue.Avalonia.Dock.Controls
{
    /// <summary>
    /// A custom tab item with a draggable/pinnable title bar.
    /// </summary>
    public partial class DockTool : TemplatedControl
    {
        /// <summary>
        /// Defines the style property for the <see cref="Content"/> member.
        /// </summary>
        public static readonly StyledProperty<Object?> ContentProperty =
            AvaloniaProperty.Register<DockTool, Object?>(nameof(Content));

        /// <summary>
        /// Defines the style property for the <see cref="Header"/> member.
        /// </summary>
        public static readonly StyledProperty<String> HeaderProperty =
            AvaloniaProperty.Register<DockTool, String>(nameof(Header));

        /// <summary>
        /// Defines the style property for the <see cref="Title"/> member.
        /// </summary>
        public static readonly StyledProperty<String> TitleProperty =
            AvaloniaProperty.Register<DockTool, String>(nameof(Title));

        /// <summary>
        /// Defines the style property for the <see cref="IsClosed"/> member.
        /// </summary>
        public static readonly StyledProperty<Boolean> IsClosedProperty =
            AvaloniaProperty.Register<DockTool, Boolean>(nameof(IsClosed));

        /// <summary>
        /// Defines the style property for the <see cref="IsPinned"/> member.
        /// </summary>
        public static readonly StyledProperty<Boolean> IsPinnedProperty =
            AvaloniaProperty.Register<DockTool, Boolean>(nameof(IsPinned));

        /// <summary>
        /// Initializes a new instance of the <see cref="DockTool"/> class.
        /// </summary>
        public DockTool()
        {
            this.Loaded += (_, _) =>
            {
                this.InvalidateVisual();
            };

            this.ToggleClosedCommand = new RelayCommand(() => this.IsClosed = !this.IsClosed);
            this.TogglePinCommand = new RelayCommand(() => this.IsPinned = !this.IsPinned);
        }

        /// <summary>
        /// Gets or sets the tool content.
        /// </summary>
        [Content]
        public Object? Content
        {
            get => this.GetValue(ContentProperty);
            set => this.SetValue(ContentProperty, value);
        }

        /// <summary>
        /// Gets or sets the header/title for the tool.
        /// </summary>
        public String Header
        {
            get => this.GetValue(HeaderProperty);
            set => this.SetValue(HeaderProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tool is currently closed.
        /// </summary>
        public Boolean IsClosed
        {
            get => this.GetValue(IsClosedProperty);
            set => this.SetValue(IsClosedProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the tool is currently pinned.
        /// </summary>
        public Boolean IsPinned
        {
            get => this.GetValue(IsPinnedProperty);
            set => this.SetValue(IsPinnedProperty, value);
        }

        /// <summary>
        /// Gets or sets the header/title for the tool.
        /// </summary>
        public String Title
        {
            get => this.GetValue(TitleProperty);
            set => this.SetValue(TitleProperty, value);
        }

        /// <summary>
        /// Gets the command to execute when <see cref="IsClosed"/> is toggled.
        /// </summary>
        public ICommand ToggleClosedCommand { get; }

        /// <summary>
        /// Gets the command to execute when <see cref="IsPinned"/> is toggled.
        /// </summary>
        public ICommand TogglePinCommand { get; }

        /// <inheritdoc/>
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            if (e is not null && e.NameScope.Find("PART_UnpinnedStub") is Border unpinnedStub)
            {
                unpinnedStub.PointerPressed += this.OnTitleBarPointerPressedAsync;
            }

            if (e is not null && e.NameScope.Find<Border>("PART_TitleBar") is { } titleBar)
            {
                titleBar.PointerPressed += this.OnTitleBarPointerPressedAsync;
            }
        }

        /// <inheritdoc/>
        protected override void OnPointerEntered(PointerEventArgs e)
        {
            base.OnPointerEntered(e);

            if (this.DataContext is DockToolViewModel viewModel)
            {
                viewModel.IsHovered = true;
            }
        }

        /// <inheritdoc/>
        protected override void OnPointerExited(PointerEventArgs e)
        {
            base.OnPointerExited(e);

            if (this.DataContext is DockToolViewModel viewModel)
            {
                viewModel.IsHovered = false;
            }
        }

        /// <summary>Proceses <see cref="PointerPressedEventArgs"/> events when the pointer is pressed.</summary>
        /// <param name="sender">The sender of the event args.</param>
        /// <param name="eventArgs">The arguments for the event.</param>
        private async void OnTitleBarPointerPressedAsync(Object? sender, PointerPressedEventArgs eventArgs)
        {
            if (eventArgs.GetCurrentPoint(null).Properties.IsLeftButtonPressed &&
                this.DataContext is DockToolViewModel tab)
            {
                DataObject data = new();
                data.Set("DockTool", tab);

                _ = await DragDrop.DoDragDrop(eventArgs, data, DragDropEffects.Move).ConfigureAwait(false);

                eventArgs.Handled = true;
            }
        }
    }
}
