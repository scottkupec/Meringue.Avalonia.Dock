// Copyright (C) Meringue Project Team. All rights reserved.

using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Meringue.Avalonia.Dock.ViewModels;

namespace HelloCombo.ViewModels
{
    /// <summary>
    /// The main view model that controls which view is currently displayed in the application.
    /// </summary>
    public partial class MainWindowViewModel : ObservableObject
    {
        /// <summary>
        /// Gets or sets the <see cref="DockInsertPolicy"/> to be used.
        /// </summary>
        [ObservableProperty]
        private DockInsertPolicy insertPolicy = DockInsertPolicy.CreateLast;

        /// <summary>Initializes a new instance of the <see cref="MainWindowViewModel"/> class.</summary>
        public MainWindowViewModel()
        {
            this.HostRoot = BuildHostRoot();
            this.LayoutRoot = BuildLayoutRoot();
        }

        /// <summary>Gets the thing.</summary>
        public DockHostRootViewModel HostRoot { get; }

        /// <summary>Gets the thing.</summary>
        public DockLayoutRootViewModel LayoutRoot { get; }

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

        /// <summary>Build a DockLayoutRootViewModel.</summary>.
        /// <returns>The thing built.</returns>
        private static DockLayoutRootViewModel BuildLayoutRoot()
        {
            DockLayoutRootViewModel layout = new();
            DockSplitNodeViewModel split = new();
            layout.HostRoot = new DockHostRootViewModel(split);
            _ = layout.CreateOrUpdateTool("1", "Hello Panel", new TextBlock { Text = "Hello" });
            _ = layout.CreateOrUpdateTool("2", "Layout Panel", new TextBlock { Text = "Layout" });

            return layout;
        }

        /// <summary>Load the saved layout.</summary>
        [RelayCommand]
        private void LoadLayout()
        {
            _ = this.LayoutRoot.LoadLayout("layout.json");
        }

        /// <summary>Save the layout.</summary>
        [RelayCommand]
        private void SaveLayout()
        {
            this.LayoutRoot.SaveLayout("layout.json");
        }
    }
}
