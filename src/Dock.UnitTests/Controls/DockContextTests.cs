// Copyright (C) Scott Kupec. All rights reserved.

using Avalonia.Controls;
using Meringue.Avalonia.Dock.Controls;
using Meringue.Avalonia.Dock.ViewModels;
using NUnit.Framework;

namespace Meringue.Avalonia.Dock.UnitTests.Controls
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class DockContextTests
    {
        [Test]
        public void FindRootNode_ReturnsNull_IfNoRootAnywhere()
        {
            // Arrange
            Border control = new();

            // Act
            DockHostRootViewModel? result = DockContext.FindRootNode(control);

            // Assert
            Assert.That(result, Is.Null, "FindRootNode should return null if no root is found in the control or its parents.");
        }

        [Test]
        public void FindRootNode_ReturnsRootFromLogicalParent()
        {
            // Arrange
            DockHostRootViewModel root = new(new DockSplitNodeViewModel());
            StackPanel parent = new();
            TextBlock child = new();

            // Attach child logically to parent
            ((ISetLogicalParent)child).SetParent(parent);
            DockContext.SetRootNode(parent, root);

            // Act
            DockHostRootViewModel? found = DockContext.FindRootNode(child);

            // Assert
            Assert.That(found, Is.SameAs(root), "FindRootNode should walk up the logical tree to find the attached root.");
        }

        [Test]
        public void FindRootNode_ReturnsRootFromSameControl()
        {
            // Arrange
            DockHostRootViewModel root = new(new DockSplitNodeViewModel());
            Border control = new();
            DockContext.SetRootNode(control, root);

            // Act
            DockHostRootViewModel? found = DockContext.FindRootNode(control);

            // Assert
            Assert.That(found, Is.SameAs(root), "FindRootNode should return the root node attached to the same control.");
        }

        [Test]
        public void GetRootNode_ReturnsNull_WhenNotSet()
        {
            // Arrange
            TextBlock control = new();

            // Act
            DockHostRootViewModel? result = DockContext.GetRootNode(control);

            // Assert
            Assert.That(result, Is.Null, "GetRootNode should return null if no root has been set.");
        }

        [Test]
        public void SetRootNode_DoesNotThrow_WhenElementIsNull()
        {
            // Act & Assert
            Assert.That(
                () => DockContext.SetRootNode(null!, new DockHostRootViewModel(new DockSplitNodeViewModel())),
                Throws.Nothing,
                "SetRootNode should not throw when passed a null control.");
        }

        [Test]
        public void SetRootNode_SetsAttachedPropertyCorrectly()
        {
            // Arrange
            Panel control = new();
            DockHostRootViewModel rootVm = new(new DockSplitNodeViewModel());

            // Act
            DockContext.SetRootNode(control, rootVm);

            // Assert
            DockHostRootViewModel? result = DockContext.GetRootNode(control);
            Assert.That(result, Is.SameAs(rootVm), "SetRootNode should store the view model in the attached property.");
        }
    }
}
