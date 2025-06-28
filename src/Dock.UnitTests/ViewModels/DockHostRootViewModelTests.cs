// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Meringue.Avalonia.Dock.ViewModels.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal sealed class DockHostRootViewModelTests
    {
        [Test]
        public void Constructor_HooksInitialToolsAndAddsUnpinned()
        {
            DockToolViewModel tool1 = NewTool("tool1", pinned: false);
            DockTabNodeViewModel tab = new();
            tab.Tabs.Add(tool1);
            DockHostRootViewModel root = new(tab);

            Assert.That(
                root.UnpinnedTabs,
                Contains.Item(tool1),
                $"{nameof(DockHostRootViewModel.UnpinnedTabs)} should be correct.");

            Assert.That(
                root.ShouldShowUnpinnedTabs,
                Is.True,
                $"{nameof(DockHostRootViewModel.UnpinnedTabs)} should be correct.");
        }

        [Test]
        public void PinnedTool_IsNotInUnpinnedTabs()
        {
            DockToolViewModel tool = NewTool("tool", pinned: true);
            DockTabNodeViewModel tab = new();
            tab.Tabs.Add(tool);
            DockHostRootViewModel root = new(tab);

            Assert.That(
                root.UnpinnedTabs,
                Is.Empty,
                $"{nameof(DockHostRootViewModel.UnpinnedTabs)} should be correct.");
        }

        [Test]
        public void UnpinnedTool_IsAddedToUnpinnedTabs()
        {
            DockToolViewModel tool = NewTool("tool", pinned: false);
            DockTabNodeViewModel tab = new();
            tab.Tabs.Add(tool);
            DockHostRootViewModel root = new(tab);

            Assert.That(
                root.UnpinnedTabs,
                Contains.Item(tool),
                $"{nameof(DockHostRootViewModel.UnpinnedTabs)} should be correct.");
        }

        [Test]
        public void TogglingIsPinned_UpdatesUnpinnedTabs()
        {
            DockToolViewModel tool = NewTool("tool", pinned: true);
            DockTabNodeViewModel tab = new();
            tab.Tabs.Add(tool);
            DockHostRootViewModel root = new(tab);

            Assert.That(
                root.UnpinnedTabs,
                Is.Empty,
                $"{nameof(DockHostRootViewModel.UnpinnedTabs)} should be empty.");

            tool.IsPinned = false;
            Assert.That(
                root.UnpinnedTabs,
                Contains.Item(tool),
                $"{nameof(DockHostRootViewModel.UnpinnedTabs)} should contain a tool that changes to unpinned.");

            tool.IsPinned = true;
            Assert.That(
                root.UnpinnedTabs,
                Is.Empty,
                $"{nameof(DockHostRootViewModel.UnpinnedTabs)} should not contain a tool that changes to pinned.");
        }

        [Test]
        public void AddingNewTool_HooksPinnedState()
        {
            DockToolViewModel tool = NewTool("tool", pinned: false);
            DockTabNodeViewModel tab = new();
            DockHostRootViewModel root = new(tab);

            tab.Tabs.Add(tool);
            Assert.That(
                root.UnpinnedTabs,
                Contains.Item(tool),
                $"{nameof(DockHostRootViewModel.UnpinnedTabs)} should  contain a tool that starts unpinned.");
        }

        [Test]
        public void RemovingTool_UnhooksAndRemoves()
        {
            DockToolViewModel tool = NewTool("tool", pinned: false);
            DockTabNodeViewModel tab = new();
            tab.Tabs.Add(tool);
            DockHostRootViewModel root = new(tab);

            Assert.That(
                root.UnpinnedTabs,
                Contains.Item(tool),
                $"{nameof(DockHostRootViewModel.UnpinnedTabs)} should  contain a tool that starts unpinned.");

            tab.Tabs.Remove(tool);
            Assert.That(
                root.UnpinnedTabs,
                Does.Not.Contain(tool),
                $"{nameof(DockHostRootViewModel.UnpinnedTabs)} should  not contain a tool gets removed.");
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
            await Task.Delay(5).ConfigureAwait(false);

            tool.IsPinned = false;
            await Task.Delay(5).ConfigureAwait(false);

            // Should not re-trigger an event.
            tool.IsPinned = false;
            await Task.Delay(5).ConfigureAwait(false);

            Assert.That(
                changes,
                Is.EqualTo(2),
                $"{nameof(DockHostRootViewModel.ShouldShowUnpinnedTabs)} should raise the correct number of events.");
        }

        private static DockToolViewModel NewTool(String id = "tool", Boolean pinned = true) =>
            new() { Id = id, Header = id, IsPinned = pinned };
    }
}
