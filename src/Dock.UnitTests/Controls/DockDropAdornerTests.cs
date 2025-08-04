// Copyright (C) Scott Kupec. All rights reserved.

using System;
using Avalonia;
using Avalonia.Controls;
using Meringue.Avalonia.Dock.Controls;
using NUnit.Framework;

namespace Meringue.Avalonia.Dock.UnitTests.Controls
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class DockDropAdornerTests
    {
        [Test]
        public void UpdatePointer_DoesNotRecompute_WhenPositionUnchanged()
        {
            // Arrange
            DockDropAdorner adorner = new();

            typeof(DockDropAdorner)
                .GetField("targetBounds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .SetValue(adorner, new Rect(0, 0, 200, 200));

            adorner.UpdatePointer(new Point(50, 50));
            DropZoneLocation first = adorner.HoveredZone;

            // Act
            adorner.UpdatePointer(new Point(50, 50)); // Same point again
            DropZoneLocation second = adorner.HoveredZone;

            // Assert
            Assert.That(second, Is.EqualTo(first), "HoveredZone should not change for same pointer location.");
        }

        [TestCase(10, 50, DropZoneLocation.Left)]
        [TestCase(190, 50, DropZoneLocation.Right)]
        [TestCase(100, 10, DropZoneLocation.Top)]
        [TestCase(100, 190, DropZoneLocation.Bottom)]
        [TestCase(100, 100, DropZoneLocation.Center)]
        public void UpdatePointer_SetsCorrectHoveredZone(Double x, Double y, DropZoneLocation expectedZone)
        {
            // Arrange
            DockDropAdorner adorner = new();

            // Simulate a 200x200 targetBounds
            typeof(DockDropAdorner)
                .GetField("targetBounds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .SetValue(adorner, new Rect(0, 0, 200, 200));

            // Act
            adorner.UpdatePointer(new Point(x, y));

            // Assert
            Assert.That(adorner.HoveredZone, Is.EqualTo(expectedZone), $"Expected zone: {expectedZone} for point ({x},{y})");
        }

        [Test]
        public void UpdateTarget_SetsAdornedElementAndTargetBounds()
        {
            // Arrange
            DockDropAdorner adorner = new();
            Border target = new(); // Doesn't matter, just a dummy control
            Point pointer = new(50, 25);

            // Inject bounds manually
            typeof(DockDropAdorner)
                .GetField("targetBounds", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
                .SetValue(adorner, new Rect(0, 0, 100, 50));

            // Act
            adorner.UpdateTarget(target, pointer);

            // Assert
            Assert.That(adorner.AdornedElement, Is.SameAs(target), "AdornedElement should be set.");
            Assert.That(adorner.HoveredZone, Is.EqualTo(DropZoneLocation.Center), "HoveredZone should match pointer.");
        }
    }
}
