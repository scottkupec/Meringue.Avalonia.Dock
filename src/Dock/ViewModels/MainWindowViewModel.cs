// Copyright (C) Meringue Project Team. All rights reserved.

using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Meringue.Avalonia.Dock.Layout;

namespace Meringue.Avalonia.Dock.ViewModels
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
        ////public DockLayoutRootViewModel LayoutRoot { get; }
        public DockLayoutRootViewModel LayoutRoot { get; }

        /// <summary>Build a DockHostRootViewModel.</summary>.
        /// <returns>The thing built.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "WIP")]
        private static DockHostRootViewModel BuildHostRoot()
        {
            DockSplitNodeViewModel split = new() { Orientation = global::Avalonia.Layout.Orientation.Horizontal };
            DockTabNodeViewModel tab1 = new();

            tab1.Tabs.Add(
                new DockToolViewModel()
                {
                    Header = "Tool 1",
                    Context = new TextBlock { Text = "Tool Content", Margin = new Thickness(8) },
                });

            tab1.Tabs.Add(
                new DockToolViewModel()
                {
                    Header = "Tool 2",
                    Context = new TextBlock { Text = "Tool Content", Margin = new Thickness(8) },
                });

            DockTabNodeViewModel tab2 = new();

            tab2.Tabs.Add(
                new DockToolViewModel()
                {
                    Header = "Tool 3",
                    Context = new TextBlock { Text = "Tool Content", Margin = new Thickness(8) },
                });

            tab2.Tabs.Add(
                new DockToolViewModel()
                {
                    Header = "Tool 4",
                    Context = new TextBlock { Text = "Tool Content", Margin = new Thickness(8) },
                });

            split.Children.Add(tab1);
            split.Children.Add(tab2);

            return new DockHostRootViewModel(split);
        }

        /// <summary>Build a DockLayoutRootViewModel.</summary>.
        /// <returns>The thing built.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0051:Remove unused private members", Justification = "WIP")]
        private static DockLayoutRootViewModel BuildLayoutRoot()
        {
            DockLayoutRootViewModel layout = new();
            DockSplitNodeViewModel split = new();
            layout.HostRoot = new DockHostRootViewModel(split);
            _ = layout.CreateOrUpdateTool("1", "header", new TextBlock { Text = "Hello from Context!" });
            _ = layout.CreateOrUpdateTool("2", "header2", new TextBlock { Text = "Hello from Context, again!" }, "X");

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
