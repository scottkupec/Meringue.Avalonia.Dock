// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using System.IO;

namespace Meringue.Avalonia.Dock.Layout
{
    /// <summary>
    /// Defines the interface a class must implement to be used as a layout manager with the docking system.
    /// </summary>
    public interface IDockLayoutManager
    {
        /// <summary>
        /// Loads the layout from the specified <paramref name="filename"/>..
        /// </summary>
        /// <param name="filename">The name of the file to use for loading the layout.</param>
        /// <returns>The loaded <see cref="DockLayout"/>.</returns>
        DockLayout Load(String filename);

        /// <summary>
        /// Loads the layout from the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="input">The <see cref="Stream"/> to use for loading the layout.</param>
        /// <returns>The loaded <see cref="DockLayout"/>.</returns>
        DockLayout Load(Stream input);

        /// <summary>
        /// Saves the layout to the specified <paramref name="filename"/>.
        /// </summary>
        /// <param name="layout">The <see cref="DockLayout"/> to save.</param>
        /// <param name="filename">The name of the file to use for saving the layout.</param>
        void Save(DockLayout layout, String filename);

        /// <summary>
        /// Saves the layout to the specified <see cref="Stream"/>.
        /// </summary>
        /// <param name="layout">The <see cref="DockLayout"/> to save.</param>
        /// <param name="output">The <see cref="Stream"/> to use for saving the layout.</param>
        void Save(DockLayout layout, Stream output);
    }
}
