// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Meringue.Avalonia.Dock.ViewModels.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal sealed class DockHostRootViewModelTests
    {
        [Test]
        public void Constructor_HooksInitialToolsAndAddsUnpinned()
        {
            DockToolViewModel tool1 = NewTool("tool1", false);
            DockTabNodeViewModel tab = new();
            tab.Tabs.Add(tool1);
            DockHostRootViewModel root = new(tab);

            Assert.That(root.UnpinnedTabs, Contains.Item(tool1));
            Assert.That(root.ShouldShowUnpinnedTabs, Is.True);
        }

        [Test]
        public void PinnedTool_IsNotInUnpinnedTabs()
        {
            DockToolViewModel tool = NewTool("tool", true);
            DockTabNodeViewModel tab = new();
            tab.Tabs.Add(tool);
            DockHostRootViewModel root = new(tab);

            Assert.That(root.UnpinnedTabs, Is.Empty);
        }

        [Test]
        public void UnpinnedTool_IsAddedToUnpinnedTabs()
        {
            DockToolViewModel tool = NewTool("tool", false);
            DockTabNodeViewModel tab = new();
            tab.Tabs.Add(tool);
            DockHostRootViewModel root = new(tab);

            Assert.That(root.UnpinnedTabs, Contains.Item(tool));
        }

        [Test]
        public void TogglingIsPinned_UpdatesUnpinnedTabs()
        {
            DockToolViewModel tool = NewTool("tool", true);
            DockTabNodeViewModel tab = new();
            tab.Tabs.Add(tool);
            DockHostRootViewModel root = new(tab);

            Assert.That(root.UnpinnedTabs, Is.Empty);

            tool.IsPinned = false;
            Assert.That(root.UnpinnedTabs, Contains.Item(tool));

            tool.IsPinned = true;
            Assert.That(root.UnpinnedTabs, Is.Empty);
        }

        [Test]
        public void AddingNewTool_HooksPinnedState()
        {
            DockToolViewModel tool = NewTool("tool", false);
            DockTabNodeViewModel tab = new();
            DockHostRootViewModel root = new(tab);

            tab.Tabs.Add(tool);
            Assert.That(root.UnpinnedTabs, Contains.Item(tool));
        }

        [Test]
        public void RemovingTool_UnhooksAndRemoves()
        {
            DockToolViewModel tool = NewTool("tool", false);
            DockTabNodeViewModel tab = new();
            tab.Tabs.Add(tool);
            DockHostRootViewModel root = new(tab);

            Assert.That(root.UnpinnedTabs, Contains.Item(tool));

            tab.Tabs.Remove(tool);
            Assert.That(root.UnpinnedTabs, Does.Not.Contain(tool));
        }

        [Test]
        public async Task GetObservableProperty_EmitsOnChange()
        {
            DockToolViewModel tool = NewTool("tool", false);
            DockTabNodeViewModel tab = new();
            tab.Tabs.Add(tool);
            DockHostRootViewModel root = new(tab);

            Int32 changes = 0;
            using IDisposable sub = root
                .GetObservableProperty(nameof(root.ShouldShowUnpinnedTabs))
                .Subscribe(_ => changes++);

            tool.IsPinned = true;
            await Task.Delay(10).ConfigureAwait(false);
            tool.IsPinned = false;
            await Task.Delay(10).ConfigureAwait(false);

            Assert.That(changes, Is.GreaterThanOrEqualTo(2));
        }

        private static DockToolViewModel NewTool(String id = "tool", Boolean pinned = true) =>
            new() { Id = id, Header = id, IsPinned = pinned };
    }
}
