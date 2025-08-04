// Copyright (C) Scott Kupec. All rights reserved.

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Meringue.Avalonia.Dock.Controls
{
    /// <summary>
    /// An adorner layer for dock drop locations.
    /// </summary>
    // TODO: Investigate AdornerLayer as probably being more appropriate.
    //       https://reference.avaloniaui.net/api/Avalonia.Controls.Primitives/AdornerLayer/
    public class DockDropAdorner : Control
    {
        /// <summary>An empty <see cref="Rect"/> so we don't have to keep creating new ones.</summary>
#pragma warning disable 0649 // Intentional
        private static readonly Rect RectEmpty;
#pragma warning restore 0649

        /// <summary>The rect bounding the control.</summary>
        private Rect targetBounds;

        /// <summary>The current position of the pointer over the control.</summary>
        private Point? pointerPosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="DockDropAdorner"/> class.
        /// </summary>
        public DockDropAdorner()
        {
            this.Focusable = false;
            this.IsHitTestVisible = false;
        }

        /// <summary>Gets or sets the control being adorned.</summary>
        public Control? AdornedElement { get; set; }

        /// <summary>Gets the current drop zone being hovered over.</summary>
        public DropZoneLocation HoveredZone { get; private set; } = DropZoneLocation.None;

        /// <summary>Renders the adorner layer.</summary>
        /// <param name="context">The context over which the layer should be drawn.</param>
        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (context is not null && this.pointerPosition is not null && this.AdornedElement is not null)
            {
                Rect bounds = this.targetBounds;
                Rect highlightRect = this.HoveredZone switch
                {
                    // TODO: Cache the rects.
                    DropZoneLocation.Left => new Rect(bounds.X, bounds.Y, bounds.Width * 0.5, bounds.Height),
                    DropZoneLocation.Right => new Rect(bounds.X + (bounds.Width * 0.5), bounds.Y, bounds.Width * 0.5, bounds.Height),
                    DropZoneLocation.Top => new Rect(bounds.X, bounds.Y, bounds.Width, bounds.Height * 0.5),
                    DropZoneLocation.Bottom => new Rect(bounds.X, bounds.Y + (bounds.Height * 0.5), bounds.Width, bounds.Height * 0.5),
                    DropZoneLocation.Center or DropZoneLocation.None => new Rect(bounds.X + (bounds.Width * 0.25), bounds.Y + (bounds.Height * 0.25), bounds.Width * 0.5, bounds.Height * 0.5),
                    _ => RectEmpty,
                };

                if (highlightRect != RectEmpty)
                {
                    context.DrawRectangle(
                        new SolidColorBrush(Colors.DeepSkyBlue, 0.3),
                        new Pen(Brushes.DeepSkyBlue, 2),
                        highlightRect);
                }
            }
        }

        /// <summary>
        /// Updates the current pointer location relative to the control.
        /// </summary>
        /// <param name="pointerPosition">The new pointer location.</param>
        public void UpdatePointer(Point? pointerPosition)
        {
            if (pointerPosition == this.pointerPosition)
            {
                return;
            }

            this.pointerPosition = pointerPosition;

            DropZoneLocation newHoveredZone = DropZoneLocation.None;

            if (pointerPosition is { } pos)
            {
                Rect bounds = this.targetBounds;
                // TODO: Cache the values.
                Double leftZone = bounds.X + (bounds.Width * 0.25);
                Double rightZone = bounds.X + (bounds.Width * 0.75);
                Double topZone = bounds.Y + (bounds.Height * 0.25);
                Double bottomZone = bounds.Y + (bounds.Height * 0.75);

                Boolean isLeft = pos.X < leftZone;
                Boolean isRight = pos.X > rightZone;
                Boolean isTop = pos.Y < topZone;
                Boolean isBottom = pos.Y > bottomZone;

                if (isLeft)
                {
                    newHoveredZone = DropZoneLocation.Left;
                }
                else if (isRight)
                {
                    newHoveredZone = DropZoneLocation.Right;
                }
                else if (isTop)
                {
                    newHoveredZone = DropZoneLocation.Top;
                }
                else if (isBottom)
                {
                    newHoveredZone = DropZoneLocation.Bottom;
                }
                else
                {
                    newHoveredZone = DropZoneLocation.Center;
                }
            }

            if (newHoveredZone != this.HoveredZone)
            {
                this.HoveredZone = newHoveredZone;
                this.InvalidateVisual();
            }
        }

        /// <summary>
        /// Updates the current target being adorned.
        /// </summary>
        /// <param name="adornedElement">The control being adorned.</param>
        /// <param name="pointerPosition">The new pointer location.</param>
        public void UpdateTarget(Control adornedElement, Point pointerPosition)
        {
            this.AdornedElement = adornedElement ?? throw new ArgumentNullException(nameof(adornedElement));

            Rect adornedBounds = adornedElement.Bounds;
            Point? topLeft = adornedElement.TranslatePoint(new Point(0, 0), this);
            if (topLeft != null)
            {
                this.targetBounds = new Rect(topLeft.Value, adornedBounds.Size);
            }

            this.UpdatePointer(pointerPosition);
        }
    }
}
