// Copyright (C) Meringue Project Team. All rights reserved.

using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Meringue.Avalonia.Dock.ViewModels;

namespace HelloCustomControl.ViewModels
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
                new DockToolViewModel
                {
                    Id = "simple-tool",
                    Header = "Test Tool",
                    Title = "- This is a tool title",
                    Context = new SimpleDockToolViewModel(),
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

            _ = layout.CreateOrUpdateTool("1", "Tab 1", new SimpleDockToolViewModel());
            _ = layout.CreateOrUpdateTool("2", "Tab 2", new SimpleDockToolViewModel());

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
