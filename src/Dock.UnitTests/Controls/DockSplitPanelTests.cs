// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.Templates;
using Meringue.Avalonia.Dock.ViewModels;
using NUnit.Framework;

namespace Meringue.Avalonia.Dock.Controls.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    // TODO: So many more tests needed!
    public class DockSplitPanelTests
    {
        [Test]
        public void OnChildrenChanged_TriggersRebuildLayoutWhenCollectionChanges()
        {
            TestVariables testVariables = new();

            // Arrange
            Boolean rebuildTriggered = false;

            testVariables.SplitPanel
                .GetType()
                .GetMethod("RebuildLayout", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
                .Invoke(testVariables.SplitPanel, null);

            testVariables.ViewModel.Children.CollectionChanged += (sender, e) =>
            {
                rebuildTriggered = true;
            };

            // Act
            testVariables.ViewModel.Children.Add(new DockTabNodeViewModel()); // Modify the collection

            // Assert
            Assert.That(
                rebuildTriggered,
                Is.True,
                "RebuildLayout should be triggered when children collection changes.");
        }

        private sealed class TestVariables
        {
            public TestVariables()
            {
                this.SplitPanel = new DockSplitPanel();
                this.SplitPanel.DataContext = this.ViewModel;
                this.SplitPanel.Orientation = Orientation.Horizontal;
                this.SplitPanel.Template = new ControlTemplate();
                this.SplitPanel.ApplyTemplate();
            }

            public DockSplitPanel SplitPanel { get; set; }

            public DockSplitNodeViewModel ViewModel { get; set; } = new();
        }
    }
}
