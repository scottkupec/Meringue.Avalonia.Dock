// Copyright (C) Meringue Project Team. All rights reserved.

using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Meringue.Avalonia.Dock.ViewModels;

namespace HelloDock.ViewModels
{
    /// <summary>
    /// The main view model that controls which view is currently displayed in the application.
    /// </summary>
    public partial class MainWindowViewModel : ObservableObject
    {
        /// <summary>Initializes a new instance of the <see cref="MainWindowViewModel"/> class.</summary>
        public MainWindowViewModel()
        {
            this.DockRoot = BuildHostRoot();
        }

        /// <summary>Gets the thing.</summary>
        public DockHostRootViewModel DockRoot { get; }

        /// <summary>Build a DockHostRootViewModel.</summary>.
        /// <returns>The thing built.</returns>
        private static DockHostRootViewModel BuildHostRoot()
        {
            DockSplitNodeViewModel splitPanel = new() { Orientation = global::Avalonia.Layout.Orientation.Horizontal };
            DockTabNodeViewModel helloTabPanel = new();

            helloTabPanel.Tabs.Add(
                new DockToolViewModel()
                {
                    Header = "Hello Tab",
                    Context = new TextBlock { Text = "Hello", Margin = new Thickness(8) },
                });

            DockTabNodeViewModel dockTabPanel = new();

            dockTabPanel.Tabs.Add(
                new DockToolViewModel()
                {
                    Header = "Dock Tab",
                    Context = new TextBlock { Text = "Dock", Margin = new Thickness(8) },
                });

            splitPanel.Children.Add(helloTabPanel);
            splitPanel.Children.Add(dockTabPanel);

            return new DockHostRootViewModel(splitPanel);
        }
    }
}
