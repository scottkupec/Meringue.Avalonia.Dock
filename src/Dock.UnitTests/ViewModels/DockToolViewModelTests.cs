// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using NUnit.Framework;

namespace Meringue.Avalonia.Dock.ViewModels.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal sealed class DockToolViewModelTests
    {
        [Test]
        public void ChangingIsHovered_RaisesIsVisible()
        {
            DockToolViewModel viewModel = new();
            Boolean isVisibleRaised = false;

            viewModel.PropertyChanged += (_, eventArgs) =>
            {
                if (eventArgs.PropertyName == nameof(DockToolViewModel.IsVisible))
                {
                    isVisibleRaised = true;
                }
            };

            viewModel.IsHovered = !viewModel.IsHovered;

            Assert.That(
                isVisibleRaised,
                Is.True,
                $"{nameof(DockToolViewModel.IsVisible)} should be raised when {nameof(viewModel.IsHovered)} changes.");
        }

        [Test]
        public void ChangingIsPinned_RaisesIsVisible()
        {
            DockToolViewModel viewModel = new();
            Boolean isVisibleRaised = false;

            viewModel.PropertyChanged += (_, eventArgs) =>
            {
                if (eventArgs.PropertyName == nameof(DockToolViewModel.IsVisible))
                {
                    isVisibleRaised = true;
                }
            };

            viewModel.IsPinned = !viewModel.IsPinned;

            Assert.That(
                isVisibleRaised,
                Is.True,
                $"{nameof(DockToolViewModel.IsVisible)} should be raised when {nameof(viewModel.IsPinned)} changes.");
        }

        [Test]
        public void Constructor_SetsDefaults()
        {
            DockToolViewModel viewModel = new();

            Assert.That(
                viewModel.Context,
                Is.Null,
                $"The default value for {nameof(DockToolViewModel.Context)} should be correct.");

            Assert.That(
                viewModel.Header,
                Is.EqualTo("Untitled"),
                $"The default value for {nameof(DockToolViewModel.Header)} should be correct.");

            Assert.That(
                Guid.TryParse(viewModel.Id, out _),
                Is.True,
                $"The default value for {nameof(DockToolViewModel.Id)} should be correct.");

            Assert.That(
                viewModel.IsHovered,
                Is.False,
                $"The default value for {nameof(DockToolViewModel.IsHovered)} should be correct.");

            Assert.That(
                viewModel.IsPinned,
                Is.True,
                $"The default value for {nameof(DockToolViewModel.IsPinned)} should be correct.");
        }

        [Test]
        public void Context_Assignment_PreservesValue()
        {
            DockToolViewModel viewModel = new();
            var content = new { Message = "Hello" };

            viewModel.Context = content;

            Assert.That(
                viewModel.Context,
                Is.EqualTo(content),
                $"Assignment of {nameof(viewModel.Context)} should preserve the value set.");
        }

        [Test]
        public void Header_Assignment_PreservesValue()
        {
            DockToolViewModel viewModel = new()
            {
                Header = "MyTool",
            };

            Assert.That(
                viewModel.Header,
                Is.EqualTo("MyTool"),
                $"Assignment of {nameof(viewModel.Header)} should preserve the value set.");
        }

        [Test]
        public void Id_Assignment_PreservesValue()
        {
            DockToolViewModel viewModel = new()
            {
                Id = "custom-tool-id",
            };

            Assert.That(
                viewModel.Id,
                Is.EqualTo("custom-tool-id"),
                $"Assignment of {nameof(viewModel.Id)} should preserve the value set.");
        }

        [Test]
        public void IsVisible_TrueWhenPinnedOrHovered()
        {
            DockToolViewModel viewModel = new();

            viewModel.IsPinned = true;
            viewModel.IsHovered = false;
            Assert.That(
                viewModel.IsVisible,
                Is.True,
                $"{nameof(viewModel.IsVisible)} should be true if {nameof(viewModel.IsPinned)} is set.");

            viewModel.IsPinned = false;
            viewModel.IsHovered = true;
            Assert.That(
                viewModel.IsVisible,
                Is.True,
                $"{nameof(viewModel.IsVisible)} should be true if {nameof(viewModel.IsHovered)} is set.");

            viewModel.IsPinned = false;
            viewModel.IsHovered = false;
            Assert.That(
                viewModel.IsVisible,
                Is.False,
                $"{nameof(viewModel.IsVisible)} should be false if neither {nameof(viewModel.IsHovered)} nor {nameof(viewModel.IsPinned)} is set.");
        }
    }
}
