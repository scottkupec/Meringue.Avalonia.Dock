// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using System.Collections.Specialized;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.VisualTree;
using Meringue.Avalonia.Dock.ViewModels;

namespace Meringue.Avalonia.Dock.Controls
{
    /// <summary>
    /// A specialized <see cref="TabControl"/> for use with docking.
    /// </summary>
    public class DockTabPanel : TabControl
    {
        /// <summary>
        /// Defines the style property for the <see cref="HasTabs"/> member.
        /// </summary>
        public static readonly StyledProperty<Boolean> HasTabsProperty =
            AvaloniaProperty.Register<DockTabPanel, Boolean>(
                nameof(HasTabs));

        /// <summary>
        /// Defines the style property for the <see cref="ShouldShowTabStrip"/> member.
        /// </summary>
        public static readonly StyledProperty<Boolean> ShouldShowTabStripProperty =
            AvaloniaProperty.Register<DockTabPanel, Boolean>(
                nameof(ShouldShowTabStrip));

        /// <summary>
        /// Gets the overlay used when dropping new tabs to the panel.
        /// </summary>
        private DockDropAdorner? dropAdorner;

        /// <summary>
        /// Initializes a new instance of the <see cref="DockTabPanel"/> class.
        /// </summary>
        public DockTabPanel()
        {
            this.SubscribeToItemsCollection();

            // Set initial value (in case items exist from XAML or template initialization)
            this.UpdateTabStripVisibility();

            // In case the entire ItemsSource is replaced.
            _ = this.GetObservable(ItemsSourceProperty)
                .Subscribe(_ =>
                {
                    this.SubscribeToItemsCollection();
                    this.UpdateTabStripVisibility();
                });
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control currently has any tabs.
        /// </summary>
        public Boolean HasTabs
        {
            get => this.GetValue(HasTabsProperty);
            set => this.SetValue(HasTabsProperty, value);
        }

        /// <summary>
        /// Gets or sets the <see cref="DockToolViewModel"/> for the currently active tab.
        /// </summary>
        public DockToolViewModel? Selected { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the corresponding <see cref="TabStrip"/> should
        /// be displayed.
        /// </summary>
        public Boolean ShouldShowTabStrip
        {
            get => this.GetValue(ShouldShowTabStripProperty);
            set => this.SetValue(ShouldShowTabStripProperty, value);
        }

        /// <summary>Gets or sets the cached value for the control's root.</summary>
        private DockHostRootViewModel? CachedRoot { get; set; }

        /// <summary>
        /// Gets or sets the collection of items currently subscribed to.
        /// </summary>
        private INotifyCollectionChanged? CurrentItemsCollection { get; set; }

        /// <inheritdoc/>
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            this.CachedRoot ??= DockContext.FindRootNode(this);
        }

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            this.AddHandler(DragDrop.DragEnterEvent, this.OnDragEnter);
            this.AddHandler(DragDrop.DragLeaveEvent, this.OnDragLeave);
            this.AddHandler(DragDrop.DragOverEvent, this.OnDragOver);
            this.AddHandler(DragDrop.DropEvent, this.OnTabDropped);
            this.AddHandler(SelectionChangedEvent, this.OnTabSelectionChanged, RoutingStrategies.Bubble);
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            if (this.DataContext is DockTabNodeViewModel vm && vm.Selected is not null)
            {
                this.SelectedItem = vm.Selected;
            }
        }

        /// <summary>Find the parent of the tab.</summary>
        /// <param name="root">The root of the full tree.</param>
        /// <param name="tab">The tab to find the parent for.</param>
        /// <returns>The parent of the tab.</returns>
        // CONSIDER: Move to DockNodeViewModel or make an extension method.
        private static DockTabNodeViewModel? FindCurrentParentNode(DockNodeViewModel root, DockToolViewModel tab)
        {
            if (root is DockTabNodeViewModel tabNode)
            {
                if (tabNode.Tabs.Contains(tab))
                {
                    return tabNode;
                }
            }
            else if (root is DockSplitNodeViewModel splitNode)
            {
                foreach (DockNodeViewModel child in splitNode.Children)
                {
                    DockTabNodeViewModel? found = FindCurrentParentNode(child, tab);
                    if (found != null)
                    {
                        return found;
                    }
                }
            }

            return null;
        }

        /// <summary>Find the nearest parent <see cref="DockSplitNodeViewModel"/> of the <paramref name="child"/>.</summary>
        /// <param name="current">The root of the full tree.</param>
        /// <param name="child">The <see cref="DockNodeViewModel"/> to find the parent for.</param>
        /// <returns>The nearest <see cref="DockSplitNodeViewModel"/> parent, if any..</returns>
        private static DockSplitNodeViewModel? FindParentSplitNode(DockNodeViewModel current, DockNodeViewModel child)
        {
            if (current is DockSplitNodeViewModel splitNode)
            {
                foreach (DockNodeViewModel node in splitNode.Children)
                {
                    if (node == child)
                    {
                        return splitNode;
                    }

                    DockSplitNodeViewModel? result = FindParentSplitNode(node, child);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            else if (current is DockTabNodeViewModel)
            {
                // Tab nodes have no children to recurse into
                return null;
            }

            return null;
        }

        /// <summary>
        /// Removes empty/unneeded panels from the root.
        /// </summary>
        /// <param name="root">The root being cleanup up.</param>
        /// <param name="node">The node to start at for removal.</param>
        // TODO: This is awfully long for early returns to be used. Refactor?
        private static void RemoveEmptyPanels(DockNodeViewModel root, DockNodeViewModel? node)
        {
            while (node is not null && node != root)
            {
                if (node is DockTabNodeViewModel tabNode && tabNode.Tabs.Count == 0)
                {
                    DockSplitNodeViewModel? parent = FindParentSplitNode(root, tabNode);
                    if (parent is null)
                    {
                        return;
                    }

                    _ = parent.Children.Remove(tabNode);
                    node = parent;
                }
                else if (node is DockSplitNodeViewModel splitNode && splitNode.Children.Count == 1)
                {
                    // Promote the only child of this split node into the parent
                    DockNodeViewModel childToPromote = splitNode.Children[0];

                    DockSplitNodeViewModel? parent = FindParentSplitNode(root, splitNode);
                    if (parent is null)
                    {
                        return;
                    }

                    Int32 index = parent.Children.IndexOf(splitNode);
                    if (index >= 0)
                    {
                        parent.Children[index] = childToPromote;
                        node = parent;
                    }
                    else
                    {
                        return;
                    }
                }
                else if (node is DockSplitNodeViewModel emptySplit && emptySplit.Children.Count == 0)
                {
                    DockSplitNodeViewModel? parent = FindParentSplitNode(root, emptySplit);
                    if (parent is null)
                    {
                        return;
                    }

                    _ = parent.Children.Remove(emptySplit);
                    node = parent;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Called when starting to drag into a control.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventArgs">The <see cref="DragEventArgs"/> for the event.</param>
        private void OnDragEnter(Object? sender, DragEventArgs eventArgs)
        {
            if (!eventArgs.Data.Contains("DockTool"))
            {
                return;
            }

            if (this.dropAdorner is null && this.GetVisualParent() is Panel root)
            {
                this.dropAdorner = new DockDropAdorner();
                root.Children.Add(this.dropAdorner);
            }

            eventArgs.Handled = true;
        }

        /// <summary>
        /// Called when no longer dragging a tab over the current control.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventArgs">The <see cref="DragEventArgs"/> for the event.</param>
        private void OnDragLeave(Object? sender, DragEventArgs eventArgs)
        {
            this.RemoveDropAdorner();
        }

        /// <summary>
        /// Called when dragging a tab over the current control.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventArgs">The <see cref="DragEventArgs"/> for the event.</param>
        private void OnDragOver(Object? sender, DragEventArgs eventArgs)
        {
            if (!eventArgs.Data.Contains("DockTool") || this.dropAdorner is null)
            {
                return;
            }

            Point pointerPosition = eventArgs.GetPosition(this.dropAdorner);
            this.dropAdorner.UpdateTarget(this, pointerPosition);

            eventArgs.DragEffects = DragDropEffects.Move;
            eventArgs.Handled = true;
        }

        /// <summary>
        /// Called when the item collection has changes.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventArgs">The <see cref="NotifyCollectionChangedEventArgs"/> for the event.</param>
        private void OnItemsCollectionChanged(Object? sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            this.UpdateTabStripVisibility();
        }

        /// <summary>
        /// Called when dropping a tab onto the current control.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventArgs">The <see cref="DragEventArgs"/> for the event.</param>
        private void OnTabDropped(Object? sender, DragEventArgs eventArgs)
        {
            System.Diagnostics.Debug.WriteLine($"Location is {this.dropAdorner?.HoveredZone}");

            if (this.dropAdorner is null)
            {
                System.Diagnostics.Debug.WriteLine($"Adorner is null.");
            }

            // Extract the dragged DockToolViewModel
            if (eventArgs.Data.Get("DockTool") is not DockToolViewModel draggedTab)
            {
                this.RemoveDropAdorner();
                return;
            }

            // Attempt to find the target DockTabNodeViewModel from the visual tree
            Control? visual = (eventArgs.Source as Visual)?
                .GetVisualAncestors()
                .OfType<Control>()
                .FirstOrDefault(c => c.DataContext is DockTabNodeViewModel);

            if (visual?.DataContext is not DockTabNodeViewModel targetNode)
            {
                this.RemoveDropAdorner();
                return;
            }

            // Resolve the DockHostRootViewModel to access root node
            DockHostRootViewModel? root = DockContext.FindRootNode(this);
            if (root?.HostRoot is not DockNodeViewModel hostRoot)
            {
                this.RemoveDropAdorner();
                return;
            }

            // Find source node where the tab originally resides
            DockTabNodeViewModel? sourceNode = FindCurrentParentNode(hostRoot, draggedTab);
            if (sourceNode is null || sourceNode == targetNode)
            {
                if (this.dropAdorner?.HoveredZone is DropZoneLocation.Center or DropZoneLocation.None)
                {
                    this.RemoveDropAdorner();
                    return;
                }
            }

            if (sourceNode is not null && sourceNode.Tabs.Remove(draggedTab))
            {
                if (this.dropAdorner?.HoveredZone is DropZoneLocation.Center or DropZoneLocation.None)
                {
                    targetNode.Tabs.Add(draggedTab);

                    // If this is a tab container control, update selection
                    if (this is SelectingItemsControl selecting)
                    {
                        selecting.SelectedItem = draggedTab;
                    }
                }
                else
                {
                    DockSplitNodeViewModel? parentSplit = FindParentSplitNode(hostRoot, targetNode);

                    System.Diagnostics.Debug.Assert(parentSplit is not null, "Bad JuJu - null split parent!");

                    Orientation neededOrientation = this.dropAdorner?.HoveredZone is DropZoneLocation.Left or DropZoneLocation.Right
                        ? Orientation.Horizontal
                        : Orientation.Vertical;

                    DropZoneLocation dropLocation = this.dropAdorner?.HoveredZone ?? DropZoneLocation.Center;
                    Int32 targetIndex = parentSplit!.Children.IndexOf(targetNode);

                    if (parentSplit.Orientation == neededOrientation)
                    {
                        // Add a new DockTabPanel to the current parent to the left or right of the current DockTabPanel then
                        // move draggedTab to that DockTabPanel.
                        // Create a new DockTabNode for the dropped tab
                        DockTabNodeViewModel newTabNode = new();
                        newTabNode.Tabs.Add(draggedTab);

                        Int32 insertIndex = dropLocation is DropZoneLocation.Left or DropZoneLocation.Top
                            ? targetIndex
                            : targetIndex + 1;

                        parentSplit.Children.Insert(insertIndex, newTabNode);
                    }
                    else
                    {
                        // Create a new DockTabNodeViewModel for the dropped tab
                        DockTabNodeViewModel newTabNode = new();
                        newTabNode.Tabs.Add(draggedTab);

                        // Create a new inner split with the required orientation
                        DockSplitNodeViewModel newInnerSplit = new()
                        {
                            Orientation = neededOrientation,
                        };

                        if (dropLocation is DropZoneLocation.Left or DropZoneLocation.Top)
                        {
                            newInnerSplit.Children.Add(newTabNode);
                            newInnerSplit.Children.Add(targetNode);
                        }
                        else
                        {
                            newInnerSplit.Children.Add(targetNode);
                            newInnerSplit.Children.Add(newTabNode);
                        }

                        // Replace targetNode in parentSplit with newInnerSplit
                        Int32 replaceIndex = parentSplit.Children.IndexOf(targetNode);
                        if (replaceIndex >= 0)
                        {
                            parentSplit.Children[replaceIndex] = newInnerSplit;
                        }
                    }
                }

                RemoveEmptyPanels(hostRoot, sourceNode);
            }

            this.RemoveDropAdorner();
        }

        /// <summary>
        /// Called when currently selected tab changes.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="eventArgs">The <see cref="DragEventArgs"/> for the event.</param>
        private void OnTabSelectionChanged(Object? sender, SelectionChangedEventArgs eventArgs)
        {
            if (this.DataContext is DockTabNodeViewModel viewModel && this.SelectedItem is DockToolViewModel selectedTab)
            {
                viewModel.Selected = selectedTab;
            }
        }

        /// <summary>
        /// Removes any adorner.
        /// </summary>
        private void RemoveDropAdorner()
        {
            if (this.dropAdorner is { Parent: Panel parent })
            {
                _ = parent.Children.Remove(this.dropAdorner);
            }

            this.dropAdorner = null;
        }

        /// <summary>
        /// Maintains the event subscription for the items collection.
        /// </summary>
        private void SubscribeToItemsCollection()
        {
            if (this.CurrentItemsCollection != null)
            {
                this.CurrentItemsCollection.CollectionChanged -= this.OnItemsCollectionChanged;
            }

            this.CurrentItemsCollection = this.Items;

            if (this.CurrentItemsCollection != null)
            {
                this.CurrentItemsCollection.CollectionChanged += this.OnItemsCollectionChanged;
            }
        }

        /// <summary>
        /// Keeps <see cref="DockTabPanel.ShouldShowTabStrip"/> current as items are added or removed
        /// from the collection.
        /// </summary>
        private void UpdateTabStripVisibility()
        {
            this.ShouldShowTabStrip = this.Items.Count >= 2;
            this.HasTabs = this.Items.Count != 0;
        }
    }
}
