// Copyright (C) Scott Kupec. All rights reserved.

using Avalonia.Layout;
using NUnit.Framework;

namespace Meringue.Avalonia.Dock.ViewModels.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal sealed class DockSplitNodeViewModelTests
    {
        [Test]
        public void Constructor_InitializesCollectionsAndDefaults()
        {
            DockSplitNodeViewModel viewModel = new();

            Assert.That(
                viewModel.Children,
                Is.Not.Null,
                $"The default for {nameof(DockSplitNodeViewModel.Children)} should be initialized.");

            Assert.That(
                viewModel.Children.Count,
                Is.EqualTo(0),
                $"The default for {nameof(DockSplitNodeViewModel.Children)} should be empty.");

            Assert.That(
                viewModel.Orientation,
                Is.EqualTo(Orientation.Horizontal),
                $"The default for {nameof(DockSplitNodeViewModel.Orientation)} should be correct.");

            Assert.That(
                viewModel.Sizes,
                Is.Not.Null,
                $"The default for {nameof(DockSplitNodeViewModel.Sizes)} should be initialized.");

            Assert.That(
                viewModel.Sizes.Count,
                Is.EqualTo(0),
                $"The default for {nameof(DockSplitNodeViewModel.Sizes)} should be empty.");
        }

        [Test]
        public void Orientation_CanBeChanged()
        {
            DockSplitNodeViewModel viewModel = new();
            viewModel.Orientation = Orientation.Vertical;

            Assert.That(
                viewModel.Orientation,
                Is.EqualTo(Orientation.Vertical),
                $"The updated {nameof(DockSplitNodeViewModel.Orientation)} should be correct.");
        }

        [Test]
        public void Children_CanBeMutated()
        {
            DockSplitNodeViewModel viewModel = new();
            DockTabNodeViewModel child = new();

            viewModel.Children.Add(child);

            Assert.That(
                viewModel.Children,
                Contains.Item(child),
                $"The updated {nameof(DockSplitNodeViewModel.Children)} should be correct.");

            Assert.That(
                viewModel.Children.Count,
                Is.EqualTo(1),
                $"The updated {nameof(DockSplitNodeViewModel.Children)} should report the correct number of children.");
        }

        [Test]
        public void Sizes_CanBeMutated()
        {
            DockSplitNodeViewModel viewModel = new();

            viewModel.Sizes.Add(0.4);
            viewModel.Sizes.Add(0.6);

            Assert.That(
                viewModel.Sizes,
                Has.Exactly(2).Items,
                $"The {nameof(DockSplitNodeViewModel.Sizes)} should have the correct count.");

            Assert.That(
                viewModel.Sizes,
                Is.EquivalentTo([0.4, 0.6]),
                $"The {nameof(DockSplitNodeViewModel.Sizes)} should have the correct values.");
        }

        [Test]
        public void CanMixTabAndSplitNodesInChildren()
        {
            DockSplitNodeViewModel viewModel = new();
            DockTabNodeViewModel tabNode = new();
            DockSplitNodeViewModel splitNode = new();

            viewModel.Children.Add(tabNode);
            viewModel.Children.Add(splitNode);

            Assert.That(
                viewModel.Children,
                Contains.Item(tabNode),
                $"The {nameof(DockTabNodeViewModel)} child should be found.");

            Assert.That(
                viewModel.Children,
                Contains.Item(splitNode),
                $"The {nameof(DockSplitNodeViewModel)} child should be found.");

            Assert.That(
                viewModel.Children.Count,
                Is.EqualTo(2),
                "The correct number of children should be reported.");
        }
    }
}
