// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Meringue.Avalonia.Dock.ViewModels;
using NUnit.Framework;

namespace Meringue.Avalonia.Dock.Controls.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class DockTabPanelTests
    {
        [Test]
        public void UpdateTabStripVisibility_SetsExpectedFlags()
        {
            DockTabPanel panel = new();
            panel.DataContext = new DockTabNodeViewModel();

            panel.Items.Add(new TabItem());
            panel.Items.Add(new TabItem());

            typeof(DockTabPanel)
                .GetMethod("UpdateTabStripVisibility", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .Invoke(panel, null);

            Assert.That(panel.HasTabs, Is.True);
            Assert.That(panel.ShouldShowTabStrip, Is.True);
        }

        [Test]
        public void OnTabSelectionChanged_UpdatesViewModelSelected()
        {
            DockTabPanel panel = new();
            DockTabNodeViewModel tabNode = new();
            panel.DataContext = tabNode;

            DockToolViewModel tab = new();
            tabNode.Tabs.Add(tab);

            panel.SelectedItem = tab;

            SelectionChangedEventArgs args = new(DockTabPanel.SelectionChangedEvent, new List<Object>(), new List<Object>());
            typeof(DockTabPanel)
                .GetMethod("OnTabSelectionChanged", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .Invoke(panel, [panel, args]);

            Assert.That(tabNode.Selected, Is.EqualTo(tab));
        }

        [Test]
        public void FindCurrentParentNode_ReturnsExpectedNode()
        {
            DockSplitNodeViewModel split = new();
            DockTabNodeViewModel node = new();
            DockToolViewModel tool = new();

            DockTabPanel panel = new();
            panel.DataContext = new DockTabNodeViewModel();

            node.Tabs.Add(tool);
            split.Children.Add(node);

            System.Reflection.MethodInfo? method = typeof(DockTabPanel)
                .GetMethod("FindCurrentParentNode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            Object? result = method!.Invoke(null, [split, tool]);

            Assert.That(result, Is.EqualTo(node));
        }

        [Test]
        public void FindParentSplitNode_ReturnsExpectedParent()
        {
            DockSplitNodeViewModel root = new();
            DockSplitNodeViewModel split = new();
            DockTabNodeViewModel tabNode = new();
            DockTabPanel panel = new();
            panel.DataContext = new DockTabNodeViewModel();

            split.Children.Add(tabNode);
            root.Children.Add(split);

            System.Reflection.MethodInfo? method = typeof(DockTabPanel)
                .GetMethod("FindParentSplitNode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            Object? result = method!.Invoke(null, [root, tabNode]);

            Assert.That(result, Is.EqualTo(split));
        }

        [Test]
        public void RemoveEmptyPanels_RemovesEmptyTabNode()
        {
            DockSplitNodeViewModel split = new();
            DockTabNodeViewModel tabNode = new(); // no tabs
            DockTabPanel panel = new();
            panel.DataContext = new DockTabNodeViewModel();

            split.Children.Add(tabNode);

            System.Reflection.MethodInfo? method = typeof(DockTabPanel)
                .GetMethod("RemoveEmptyPanels", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            method!.Invoke(null, [split, tabNode]);

            Assert.That(split.Children.Contains(tabNode), Is.False);
        }
    }
}
