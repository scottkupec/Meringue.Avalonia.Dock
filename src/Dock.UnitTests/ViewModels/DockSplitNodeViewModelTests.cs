// Copyright (C) Meringue Project Team. All rights reserved.

using Avalonia.Layout;
using Meringue.Avalonia.Dock.ViewModels;
using NUnit.Framework;

namespace Meringue.Avalonia.Dock.Tests.ViewModels
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal sealed class DockSplitNodeViewModelTests
    {
        [Test]
        public void Constructor_InitializesCollectionsAndDefaults()
        {
            DockSplitNodeViewModel vm = new();

            Assert.That(vm.Children, Is.Not.Null);
            Assert.That(vm.Sizes, Is.Not.Null);
            Assert.That(vm.Children.Count, Is.EqualTo(0));
            Assert.That(vm.Sizes.Count, Is.EqualTo(0));
            Assert.That(vm.Orientation, Is.EqualTo(Orientation.Horizontal)); // default enum value
        }

        [Test]
        public void Orientation_CanBeChanged()
        {
            DockSplitNodeViewModel vm = new();

            vm.Orientation = Orientation.Vertical;

            Assert.That(vm.Orientation, Is.EqualTo(Orientation.Vertical));
        }

        [Test]
        public void Children_CanBeMutated()
        {
            DockSplitNodeViewModel vm = new();
            DockTabNodeViewModel child = new();

            vm.Children.Add(child);

            Assert.That(vm.Children, Contains.Item(child));
            Assert.That(vm.Children.Count, Is.EqualTo(1));
        }

        [Test]
        public void Sizes_CanBeMutated()
        {
            DockSplitNodeViewModel vm = new();

            vm.Sizes.Add(0.4);
            vm.Sizes.Add(0.6);

            Assert.That(vm.Sizes, Has.Exactly(2).Items);
            Assert.That(vm.Sizes, Is.EquivalentTo([0.4, 0.6]));
        }

        [Test]
        public void CanMixTabAndSplitNodesInChildren()
        {
            DockSplitNodeViewModel vm = new();
            DockTabNodeViewModel tabNode = new();
            DockSplitNodeViewModel splitNode = new();

            vm.Children.Add(tabNode);
            vm.Children.Add(splitNode);

            Assert.That(vm.Children, Contains.Item(tabNode));
            Assert.That(vm.Children, Contains.Item(splitNode));
            Assert.That(vm.Children.Count, Is.EqualTo(2));
        }
    }
}
