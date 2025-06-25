// Copyright (C) Meringue Project Team. All rights reserved.

namespace Meringue.Avalonia.Dock.ViewModels
{
    /// <summary>
    /// Defines the options for how non-existing tools should be inserted into a layout.
    /// </summary>
    public enum DockInsertPolicy
    {
        /// <summary>
        /// If parent is not found, the new tool is added at the beginning of the root container.
        /// </summary>
        CreateFirst,

        /// <summary>
        /// If parent is not found, the new tool is added at the end of the root container.
        /// </summary>
        CreateLast,

        /// <summary>
        /// If parent is not found, throw an exception and do not attach.
        /// </summary>
        Error,
    }
}
