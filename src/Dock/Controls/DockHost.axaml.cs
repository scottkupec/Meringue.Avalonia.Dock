// Copyright (C) Scott Kupec. All rights reserved.

using Avalonia.Controls;

namespace Meringue.Avalonia.Dock.Controls
{
    /// <summary>
    /// Represents either a <see cref="DockTabPanel"/> or a <see cref="DockSplitPanel"/> in the
    /// docking controls tree.
    /// </summary>
    public partial class DockHost : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DockHost"/> class.
        /// </summary>
        public DockHost()
            => this.InitializeComponent();
    }
}
