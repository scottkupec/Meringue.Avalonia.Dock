// Copyright (C) Meringue Project Team. All rights reserved.

namespace Meringue.Avalonia.Dock.Controls
{
    /// <summary>
    /// Defines the zones where a tab can be dropped when dragging
    /// to a DockTabPanel.
    /// </summary>
    public enum DropZoneLocation
    {
        /// <summary>
        /// No dock location is currently being hovered over.
        /// </summary>
        None,

        /// <summary>
        /// Add the tab as a new tab to the current DockTabPanel.
        /// </summary>
        Center,

        /// <summary>
        /// Split the current DockTabPanel vertically adding the new tab to the
        /// left of the current content.
        /// </summary>
        Left,

        /// <summary>
        /// Split the current DockTabPanel vertically adding the new tab to the
        /// right of the current content.
        /// </summary>
        Right,

        /// <summary>
        /// Split the current DockTabPanel horizontally adding the new tab above
        /// current content.
        /// </summary>
        Top,

        /// <summary>
        /// Split the current DockTabPanel horizontally adding the new tab below
        /// current content.
        /// </summary>
        Bottom,
    }
}
