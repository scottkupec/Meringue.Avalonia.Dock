// Copyright (C) Meringue Project Team. All rights reserved.

using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Meringue.Avalonia.Dock.ViewModels;

namespace HelloLayout.ViewModels
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
            this.LayoutRoot = BuildLayoutRoot();
        }

        /// <summary>Gets the thing.</summary>
        public DockLayoutRootViewModel LayoutRoot { get; }

        /// <summary>Build a DockLayoutRootViewModel.</summary>.
        /// <returns>The thing built.</returns>
        private static DockLayoutRootViewModel BuildLayoutRoot()
        {
            DockLayoutRootViewModel layout = new();
            DockSplitNodeViewModel split = new()
            {
                Id = "root",
            };

            DockTabNodeViewModel tab = new()
            {
                Id = "center",
            };

            split.Children.Add(tab);

            layout.HostRoot = new DockHostRootViewModel(split);
            _ = layout.CreateOrUpdateTool("1", "Hello Panel", new TextBlock { Text = "Hello" }, "center");
            _ = layout.CreateOrUpdateTool("2", "Layout Panel", new TextBlock { Text = "Layout" }, "center");
            ////_ = layout.CreateOrUpdateTool("3", "Extra Panel", new TextBlock { Text = "Layout" }, "center");

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
