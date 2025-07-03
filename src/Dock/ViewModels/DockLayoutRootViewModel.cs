// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using Meringue.Avalonia.Dock.Controls;
using Meringue.Avalonia.Dock.Layout;

namespace Meringue.Avalonia.Dock.ViewModels
{
    /// <summary>
    /// Defines the view model for use with <see cref="DockLayoutRoot"/>.
    /// </summary>
    public partial class DockLayoutRootViewModel : ObservableObject
    {
        /// <summary>
        /// Gets or sets the <see cref="DockHostRootViewModel"/> being managed.
        /// </summary>
        private DockHostRootViewModel hostRoot;

        /// <summary>
        /// Gets or sets the <see cref="DockInsertPolicy"/> to be used when adding <see cref="DockToolViewModel"/>s that
        /// are not present in the recorded layout.
        /// </summary>
        [ObservableProperty]
        private DockInsertPolicy insertPolicy = DockInsertPolicy.CreateLast;

        /// <summary>
        /// Initializes a new instance of the <see cref="DockLayoutRootViewModel"/> class.
        /// </summary>
        public DockLayoutRootViewModel()
            : this(null!)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DockLayoutRootViewModel"/> class.
        /// </summary>
        /// <param name="panels">The names of the top-level split panels to pre-create.</param>
        public DockLayoutRootViewModel(params String[] panels)
        {
            DockSplitNodeViewModel split = new()
            {
                Orientation = global::Avalonia.Layout.Orientation.Horizontal,
            };

            if (panels is not null)
            {
                foreach (String panelName in panels)
                {
                    split.Children.Add(
                        new DockTabNodeViewModel()
                        {
                            Id = panelName,
                        });
                }
            }

            this.hostRoot = new DockHostRootViewModel(split);

            this.LayoutManager = new JsonLayoutManager();
        }

        /// <summary>
        /// Gets the <see cref="DockHostRootViewModel"/> being managed.
        /// </summary>
        internal DockHostRootViewModel HostRoot
        {
            get => this.hostRoot;

            private set => this.SetProperty(ref this.hostRoot, value);
        }

        /// <summary>
        /// Gets or sets the layout manager responsible for loading and saving layouts.
        /// </summary>
        // TODO: Review accessibilty modifiers.
        protected internal IDockLayoutManager LayoutManager { get; set; }

        /// <summary>
        /// Adds or updates a tool in the layout.
        /// </summary>
        /// <param name="id">The id of the <see cref="DockTool"/> to be updated.</param>
        /// <param name="header">If not null, the header (title) to set on the created or updated <see cref="DockTool"/>.</param>
        /// <param name="context">The context to set on the created or updated <see cref="DockTool"/>.</param>
        /// <param name="defaultParentId">
        /// The optional name of the parent node to use if the <see cref="DockTool"/> needs to be created.
        /// If no node with the given name exists or if the parameter is null, the behavior of the API is defined by
        /// the <see cref="InsertPolicy"/> property.
        /// </param>
        /// <returns>The <see cref="DockTool"/> created or updated.</returns>
        public virtual DockToolViewModel? CreateOrUpdateTool(String id, String? header, Object context, String? defaultParentId = null)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Tool ID cannot be null or whitespace.", nameof(id));
            }

            DockToolViewModel? tool = null;

            if (this.HostRoot.HostRoot is DockNodeViewModel rootNode)
            {
                tool = rootNode.FindTool(id);

                if (tool is null)
                {
                    DockNodeViewModel? targetNode = null;

                    if (!String.IsNullOrEmpty(defaultParentId))
                    {
                        targetNode = rootNode.FindNode(defaultParentId!);
                    }

                    if (targetNode is null)
                    {
                        if (this.InsertPolicy == DockInsertPolicy.Error)
                        {
                            throw new ArgumentOutOfRangeException(nameof(defaultParentId), "Parent could not be found");
                        }

                        targetNode = rootNode;
                    }

                    tool = new DockToolViewModel
                    {
                        Id = id,
                        Header = header ?? String.Empty,
                        Context = context,
                    };

                    if (targetNode is DockTabNodeViewModel tabNode)
                    {
                        tabNode.Tabs.Add(tool);
                    }
                    else if (targetNode is DockSplitNodeViewModel splitNode)
                    {
                        DockTabNodeViewModel newTabNode = new();
                        newTabNode.Tabs.Add(tool);
                        newTabNode.Selected = tool;

                        if (this.InsertPolicy == DockInsertPolicy.CreateFirst)
                        {
                            splitNode.Children.Insert(0, newTabNode);
                        }
                        else
                        {
                            splitNode.Children.Add(newTabNode);
                        }
                    }
                    else
                    {
                        // Unexpected node type — fallback behavior
                        throw new InvalidOperationException($"Unsupported parent node type: {targetNode.GetType().Name}");
                    }
                }
                else
                {
                    if (header is not null)
                    {
                        tool.Header = header;
                    }

                    tool.Context = context;
                }
            }

            return tool;
        }

        /// <summary>
        /// Saves the current layout to a stream.
        /// </summary>
        /// <param name="stream">The name of the stream from which the layout should be loaded.</param>
        /// <returns><c>true</c> on success; otherwise, <c>false</c>.</returns>
        public Boolean LoadLayout(Stream stream)
        {
            DockLayout layout = this.LayoutManager.Load(stream);
            return this.ApplyLayout(layout);
        }

        /// <summary>
        /// Saves the current layout to a stream.
        /// </summary>
        /// <param name="filename">The name of the file from which the layout should be loaded.</param>
        /// <returns><c>true</c> on success; otherwise, <c>false</c>.</returns>
        public Boolean LoadLayout(String filename)
        {
            DockLayout layout = this.LayoutManager.Load(filename);
            return this.ApplyLayout(layout);
        }

        /// <summary>
        /// Saves the current layout to a stream.
        /// </summary>
        /// <param name="filename">The name of the file that will receive the layout.</param>
        public void SaveLayout(String filename)
        {
            DockLayout layout = DockLayoutConverter.BuildLayout(this.HostRoot);
            this.LayoutManager.Save(layout, filename);
        }

        /// <summary>
        /// Saves the current layout to a stream.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> that will receive the layout.</param>
        public void SaveLayout(Stream stream)
        {
            DockLayout layout = DockLayoutConverter.BuildLayout(this.HostRoot);
            this.LayoutManager.Save(layout, stream);
        }

        /// <summary>
        /// Enumerates all of the <see cref="DockTabNodeViewModel"/> in a <see cref="DockNodeViewModel"/>.
        /// </summary>
        /// <param name="hostRoot">The <see cref="DockNodeViewModel"/> to enumerate.</param>
        /// <returns>The enumeration of <see cref="DockTabNodeViewModel"/>s found.</returns>
        private static IEnumerable<DockTabNodeViewModel> EnumerateTabViewModels(DockNodeViewModel hostRoot)
        {
            if (hostRoot is DockSplitNodeViewModel splitView)
            {
                foreach (DockNodeViewModel child in splitView.Children)
                {
                    foreach (DockTabNodeViewModel tabView in EnumerateTabViewModels(child))
                    {
                        yield return tabView;
                    }
                }
            }
            else if (hostRoot is DockTabNodeViewModel tabView)
            {
                yield return tabView;
            }
        }

        /// <summary>
        /// Applies the provided layout to the current instance.
        /// </summary>
        /// <param name="layout">The <see cref="DockLayout"/> tto be applied.</param>
        /// <returns><c>true</c> if the layout was applied; otherwise <c>false</c>.</returns>
        private Boolean ApplyLayout(DockLayout layout)
        {
            // TODO: Review for leaking resources.
            // TODO: Handle different merge options:
            //       - KeepOldContexts
            //       - KeepNewContexts (current, default)
            //       - RemoveOldTools (don't re-add tools from rootGoingAway)
            //       - RemoveNewTools (don't add tools from layout that aren't in rootGoingAway)
            //       - KeepAllTools (current, default)
            DockHostRootViewModel root = DockLayoutConverter.BuildViewModel(layout);
            DockHostRootViewModel rootGoingAway = this.HostRoot;

            this.HostRoot = root;
            this.HostRoot.UnpinnedTabs.Clear();
            this.HostRoot.HookPinnedChangeListeners(root.HostRoot);

            foreach (DockTabNodeViewModel tabView in EnumerateTabViewModels(rootGoingAway.HostRoot))
            {
                foreach (DockToolViewModel tool in tabView.Tabs)
                {
                    DockToolViewModel? alreadyInsertedTool = this.HostRoot.HostRoot.FindTool(tool.Id);

                    if (alreadyInsertedTool is not null)
                    {
                        alreadyInsertedTool.Context = tool.Context;
                    }
                    else
                    {
                        _ = this.CreateOrUpdateTool(tool.Id, tool.Header, tool.Context!, tabView.Id);
                    }
                }
            }

            return true;
        }
    }
}
