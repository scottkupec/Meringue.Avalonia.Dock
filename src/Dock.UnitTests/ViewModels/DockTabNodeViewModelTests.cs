// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Meringue.Avalonia.Dock.ViewModels;
using NUnit.Framework;

namespace Meringue.Avalonia.Dock.Tests.ViewModels
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal sealed class DockTabNodeViewModelTests
    {
        [Test]
        public void Constructor_InitializesEmptyTabs()
        {
            DockTabNodeViewModel viewModel = new();

            Assert.That(
                viewModel.Tabs,
                Is.Not.Null,
                $"{nameof(viewModel.Tabs)} should not be null.");

            Assert.That(
                viewModel.Tabs,
                Is.Empty,
                $"{nameof(viewModel.Tabs)} should not empty by default.");

            Assert.That(
                viewModel.HasTabs,
                Is.False,
                $"{nameof(viewModel.HasTabs)} should not false if there are not tabs.");

            Assert.That(
                viewModel.ShouldShowTabStrip,
                Is.False,
                $"{nameof(viewModel.ShouldShowTabStrip)} should not false if there are not tabs.");
        }

        [Test]
        public void PinnedTabs_OnlyReturnsPinned()
        {
            DockTabNodeViewModel viewModel = new();
            DockToolViewModel pinnedTool = new() { Id = "t1", IsPinned = true };

            viewModel.Tabs.Add(pinnedTool);
            viewModel.Tabs.Add(new DockToolViewModel { Id = "t2", IsPinned = false });

            Assert.That(
                viewModel.PinnedTabs.Count,
                Is.EqualTo(1),
                $"{nameof(viewModel.PinnedTabs)} should only count pinned tabs.");

            Assert.That(
                viewModel.PinnedTabs,
                Does.Contain(pinnedTool),
                $"Pinned tools should be present in {nameof(viewModel.PinnedTabs)}.");
        }

        [Test]
        public void Tabs_AddingTab_RaisesExpectedProperties()
        {
            DockTabNodeViewModel viewModel = new();
            DockToolViewModel tool = new() { Id = "tool1", IsPinned = true };

            Boolean pinnedTabsChanged = false;
            viewModel.PropertyChanged += (_, eventArgs) =>
            {
                if (eventArgs.PropertyName == nameof(DockTabNodeViewModel.PinnedTabs))
                {
                    pinnedTabsChanged = true;
                }
            };

            viewModel.Tabs.Add(tool);

            Assert.That(
                viewModel.HasTabs,
                Is.True,
                $"{nameof(viewModel.HasTabs)} should be true if there are tabs.");

            Assert.That(
                viewModel.ShouldShowTabStrip,
                Is.False,
                $"{nameof(viewModel.ShouldShowTabStrip)} should be false if there is less than two tabs.");

            Assert.That(
                viewModel.PinnedTabs.Contains(tool),
                Is.True,
                $"{nameof(viewModel.PinnedTabs)} should contain all pinned tabs.");

            Assert.That(
                pinnedTabsChanged,
                Is.True,
                $"{nameof(DockTabNodeViewModel.PinnedTabs)} should be raised when a pinned tool is added.");
        }

        [Test]
        public void Tabs_AddTwoTabs_ShowsTabStrip()
        {
            DockTabNodeViewModel viewModel = new();

            viewModel.Tabs.Add(new DockToolViewModel { Id = "tool1", IsPinned = true });
            viewModel.Tabs.Add(new DockToolViewModel { Id = "tool2", IsPinned = true });

            Assert.That(
                viewModel.Tabs.Count,
                Is.EqualTo(2),
                $"Adding two tools should result in two {nameof(viewModel.Tabs)} being present.");

            Assert.That(
                viewModel.ShouldShowTabStrip,
                $"{nameof(viewModel.ShouldShowTabStrip)} should be true if there are two or more tabs.");
        }

        [Test]
        public void Tabs_RemoveTab_StopsShowingTabStrip()
        {
            DockTabNodeViewModel viewModel = new();
            DockToolViewModel tab1 = new() { Id = "tool1", IsPinned = true };
            DockToolViewModel tab2 = new() { Id = "tool2", IsPinned = true };

            viewModel.Tabs.Add(tab1);
            viewModel.Tabs.Add(tab2);

            Assert.That(
                viewModel.ShouldShowTabStrip,
                Is.True,
                $"{nameof(viewModel.ShouldShowTabStrip)} should be true if there are two or more tabs.");

            viewModel.Tabs.Remove(tab2);

            Assert.That(viewModel.Tabs.Count, Is.EqualTo(1));
            Assert.That(viewModel.ShouldShowTabStrip, Is.False);

            Assert.That(
                viewModel.Tabs.Count,
                Is.EqualTo(1),
                $"Removing a tool should reduce the number of {nameof(viewModel.Tabs)} being present.");

            Assert.That(
                viewModel.ShouldShowTabStrip,
                Is.False,
                $"{nameof(viewModel.ShouldShowTabStrip)} should be false if there are less than two tabs present.");
        }

        [Test]
        public void Tabs_ReplaceTabs_TriggersPinnedTracking()
        {
            DockTabNodeViewModel viewModel = new();
            ObservableCollection<DockToolViewModel> newTabs =
            [
                new() { Id = "new1", IsPinned = true },
                new() { Id = "new2", IsPinned = false },
            ];

            viewModel.Tabs = newTabs;

            Assert.That(viewModel.HasTabs, Is.True);
            Assert.That(viewModel.PinnedTabs.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Tabs_ReplacingTabs_DetachesHandlers()
        {
            DockTabNodeViewModel viewModel = new();
            DockToolViewModel oldTool = new() { Id = "old", IsPinned = true };
            viewModel.Tabs.Add(oldTool);

            ObservableCollection<DockToolViewModel> newTabs =
            [
                new() { Id = "new1", IsPinned = true },
            ];

            viewModel.Tabs = newTabs;

            oldTool.IsPinned = false;
            Assert.That(viewModel.PinnedTabs.Any(t => t.Id == "old"), Is.False);
        }

        [Test]
        public void Tabs_SetPinnedFalse_RemovesFromPinnedTabs()
        {
            DockTabNodeViewModel viewModel = new();
            DockToolViewModel tool = new() { Id = "tool1", IsPinned = true };
            viewModel.Tabs.Add(tool);

            tool.IsPinned = false;

            Assert.That(viewModel.PinnedTabs.Contains(tool), Is.False);
        }

        [Test]
        public void TabsChanging_RaisesExpectedPropertyChanges()
        {
            DockTabNodeViewModel viewModel = new();
            List<String> changedProps = [];

            viewModel.PropertyChanged += (_, eventArgs) => changedProps.Add(eventArgs.PropertyName!);

            viewModel.Tabs.Add(new DockToolViewModel { Id = "a", IsPinned = true });

            Assert.That(
                changedProps,
                Does.Contain(nameof(viewModel.PinnedTabs)),
                $"{nameof(viewModel.PinnedTabs)} should have changed.");

            Assert.That(
                changedProps,
                Does.Contain(nameof(viewModel.HasTabs)),
                $"{nameof(viewModel.HasTabs)} should have changed.");

            changedProps.Clear();
            viewModel.Tabs.Add(new DockToolViewModel { Id = "b", IsPinned = true });

            Assert.That(
                changedProps,
                Does.Contain(nameof(viewModel.PinnedTabs)),
                $"{nameof(viewModel.PinnedTabs)} should have changed.");

            Assert.That(
                changedProps,
                Does.Not.Contain(nameof(viewModel.HasTabs)),
                $"{nameof(viewModel.HasTabs)} should not have changed.");

            Assert.That(
                changedProps,
                Does.Contain(nameof(viewModel.ShouldShowTabStrip)),
                $"{nameof(viewModel.ShouldShowTabStrip)} should have changed.");
        }
    }
}
