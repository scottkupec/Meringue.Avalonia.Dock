// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "SCS0005:Weak random number generator", Justification = "Randomness used for demo UI updates only")]
        private static DockHostRootViewModel BuildHostRoot()
        {
            DockSplitNodeViewModel splitPanel = new() { Orientation = global::Avalonia.Layout.Orientation.Horizontal };
            DockTabNodeViewModel helloTabPanel = new();
            Random rng = new();

            DockToolViewModel helloTool = new()
            {
                Header = "Hello Tab",
                Context = new TextBlock { Text = $"Hello #{rng.Next(1000)}", Margin = new Thickness(8) },
            };

            helloTabPanel.Tabs.Add(helloTool);

            DockTabNodeViewModel dockTabPanel = new();

            TextBlock dockTextBlock = new() { Text = $"Dock #{rng.Next(1000)}", Margin = new Thickness(8) };
            DockToolViewModel dockTool = new()
            {
                Header = "Dock Tab",
                Context = dockTextBlock,
            };

            dockTabPanel.Tabs.Add(dockTool);

            splitPanel.Children.Add(helloTabPanel);
            splitPanel.Children.Add(dockTabPanel);

            // Timer to update the context every second
            DispatcherTimer timer = new()
            {
                Interval = TimeSpan.FromMilliseconds(100),
            };

            timer.Tick += (_, _) =>
            {
                dockTextBlock.Text = $"Dock #{rng.Next(1000)}";
                helloTool.Context = new TextBlock { Text = $"Hello #{rng.Next(1000)}", Margin = new Thickness(8) };
            };

            timer.Start();

            DockSplitNodeViewModel splitPanel2 = new() { Orientation = global::Avalonia.Layout.Orientation.Horizontal };
            splitPanel2.Children.Add(splitPanel);

            DockTabNodeViewModel dockTabPanel2 = new();
            DockToolViewModel helloTool2 = new()
            {
                Header = "Hello Tab",
                Context = new TextBlock { Text = $"Hello #{rng.Next(1000)}", Margin = new Thickness(8) },
            };

            dockTabPanel2.Tabs.Add(helloTool2);
            splitPanel2.Children.Add(dockTabPanel2);
            return new DockHostRootViewModel(splitPanel2);
        }

        /// <summary>Build a DockLayoutRootViewModel.</summary>.
        /// <returns>The thing built.</returns>
        private static DockLayoutRootViewModel BuildLayoutRoot()
        {
            DockLayoutRootViewModel layout = new();
            _ = layout.CreateOrUpdateTool("1", "Hello Panel", new TextBlock { Text = "Hello" });
            _ = layout.CreateOrUpdateTool("3", "Layout Panel", new TextBlock { Text = "Layout" });

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
