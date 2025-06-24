// Copyright (C) Meringue Project Team. All rights reserved.

using Avalonia.Controls;
using HelloCombo.ViewModels;

namespace HelloCombo.Windows
{
    /// <summary>The top level window for the application.</summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = new MainWindowViewModel();
        }
    }
}
