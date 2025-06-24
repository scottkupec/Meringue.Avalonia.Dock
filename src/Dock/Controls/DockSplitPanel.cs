// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Meringue.Avalonia.Dock.ViewModels;

namespace Meringue.Avalonia.Dock.Controls
{
    /// <summary>
    /// A split panel with resizable regions for use in dock hosts.
    /// </summary>
    public partial class DockSplitPanel : TemplatedControl
    {
        /// <summary>
        /// Defines the style property for the <see cref="Orientation"/> member.
        /// </summary>
        public static readonly StyledProperty<Orientation> OrientationProperty =
            AvaloniaProperty.Register<DockSplitPanel, Orientation>(
                nameof(Orientation), Orientation.Horizontal);

        /// <summary>
        /// Initializes a new instance of the <see cref="DockSplitPanel"/> class.
        /// </summary>
        public DockSplitPanel()
        {
            this.DataContextChanged += (_, _) => this.OnDataContextChanged();
            this.Width = Double.NaN;
            this.Height = Double.NaN;
        }

        /// <summary>
        /// Gets or sets the orientation for splitting.
        /// </summary>
        public Orientation Orientation
        {
            get => this.GetValue(OrientationProperty);
            set => this.SetValue(OrientationProperty, value);
        }

        /// <summary>Gets or sets the grid being presented.</summary>
        private Grid? Container { get; set; }

        /// <summary>
        /// Gets or sets the assocaited view model for the control.
        /// </summary>
        private DockSplitNodeViewModel? ViewModel { get; set; }

        /// <inheritdoc/>
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            if (e is not null)
            {
                this.Container = e.NameScope.Find<Grid>("PART_Container");
                this.RebuildLayout();
            }
        }

        /// <summary>
        /// Helper method for building an enumeration of <see cref="Control"/>s from an enumeration of <see cref="DockNodeViewModel"/>s.
        /// </summary>
        /// <param name="children">The <see cref="DockNodeViewModel"/>s to use for constructing <see cref="Control"/>s.</param>
        /// <returns>The <see cref="Control"/>s built.</returns>
        private static IEnumerable<Control> BuildChildControls(IEnumerable<DockNodeViewModel> children)
        {
            foreach (DockNodeViewModel child in children)
            {
                yield return new DockHost { DataContext = child };
            }
        }

        /// <summary>
        /// Updates the provided <paramref name="container"/> to be a split grid of the <paramref name="children"/>.
        /// </summary>
        /// <param name="container">The <see cref="Grid"/> to be updated.</param>
        /// <param name="orientation">The <see cref="Orientation"/> to use for splitting.</param>
        /// <param name="children">The <see cref="Control"/>s to add to the container.</param>
        private void BuildGrid(Grid? container, Orientation orientation, IEnumerable<Control>? children)
        {
            if (container == null || children == null)
            {
                return;
            }

            container.Children.Clear();
            container.ColumnDefinitions.Clear();
            container.RowDefinitions.Clear();

            Boolean isHorizontal = orientation == Orientation.Horizontal;
            Int32 index = 0;

            foreach (Control child in children)
            {
                if (index > 0)
                {
                    GridSplitter splitter = new()
                    {
                        //// Background = Brushes.SlateGray,
                        Width = isHorizontal ? 0.5 : Double.NaN,
                        Height = isHorizontal ? Double.NaN : 0.5,
                        MinWidth = isHorizontal ? 0.5 : 0,
                        MaxWidth = isHorizontal ? 0.5 : Double.PositiveInfinity,
                        MinHeight = isHorizontal ? 0 : 0.5,
                        MaxHeight = isHorizontal ? Double.PositiveInfinity : 0.5,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                    };

                    splitter.DragCompleted += (_, _) => this.SaveCurrentSizes();

                    if (isHorizontal)
                    {
                        container.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
                        Grid.SetColumn(splitter, (index * 2) - 1);
                    }
                    else
                    {
                        container.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
                        Grid.SetRow(splitter, (index * 2) - 1);
                    }

                    container.Children.Add(splitter);
                }

                GridLength length = this.ViewModel?.Sizes != null && index < this.ViewModel.Sizes.Count
                    ? new GridLength(this.ViewModel.Sizes[index], GridUnitType.Star)
                    : new GridLength(1.0, GridUnitType.Star);

                if (isHorizontal)
                {
                    container.ColumnDefinitions.Add(new ColumnDefinition(length));
                }
                else
                {
                    container.RowDefinitions.Add(new RowDefinition(length));
                }

                if (child is Control ctrl)
                {
                    if (isHorizontal)
                    {
                        Grid.SetColumn(ctrl, index * 2);
                        Grid.SetRow(ctrl, 0);
                    }
                    else
                    {
                        Grid.SetRow(ctrl, index * 2);
                        Grid.SetColumn(ctrl, 0);
                    }

                    container.Children.Add(ctrl);
                }

                index++;
            }
        }

        /// <summary>
        /// Called when the children collection changes.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventArgs">The <see cref="NotifyCollectionChangedEventArgs"/> for the event.</param>
        private void OnChildrenChanged(Object? sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            // You could make this smarter with partial rebuild, but full rebuild is safer for now
            this.RebuildLayout();
        }

        /// <summary>
        /// Called when data context for the view model changes.
        /// </summary>
        private void OnDataContextChanged()
        {
            this.SaveCurrentSizes();

            if (this.ViewModel != null)
            {
                this.ViewModel.Children.CollectionChanged += this.OnChildrenChanged;
            }

            if (this.DataContext is DockSplitNodeViewModel vm && vm.Children is not null)
            {
                this.ViewModel = vm;
                this.ViewModel.Children.CollectionChanged += this.OnChildrenChanged;
                this.RebuildLayout();
            }
            else
            {
                this.ViewModel = null;
            }
        }

        /// <summary>
        /// Rebuilds the control's visuals.
        /// </summary>
        private void RebuildLayout()
        {
            if (this.ViewModel == null || this.Container == null)
            {
                return;
            }

            IEnumerable<Control> controls = BuildChildControls(this.ViewModel.Children);
            this.BuildGrid(this.Container, this.ViewModel.Orientation, controls);
        }

        /// <summary>Save the current sizes of each column or row.</summary>
        private void SaveCurrentSizes()
        {
            if (this.Container is null || this.ViewModel is null)
            {
                return;
            }

            List<Double> sizes = [];

            if (this.Orientation == Orientation.Horizontal)
            {
                // Filter out Auto columns (splitters) and get only the star columns
                foreach (ColumnDefinition column in this.Container.ColumnDefinitions)
                {
                    if (column.Width.IsStar)
                    {
                        sizes.Add(column.Width.Value);
                    }
                }
            }
            else
            {
                foreach (RowDefinition row in this.Container.RowDefinitions)
                {
                    if (row.Height.IsStar)
                    {
                        sizes.Add(row.Height.Value);
                    }
                }
            }

            // Normalize sizes to proportions
            Double total = sizes.Sum();
            if (total > 0)
            {
                this.ViewModel.Sizes = new ObservableCollection<Double>(sizes.Select(s => s / total));
            }
        }
    }
}
