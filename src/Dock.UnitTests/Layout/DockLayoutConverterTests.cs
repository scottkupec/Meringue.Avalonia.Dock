// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using System.Linq;
using Avalonia.Layout;
using Meringue.Avalonia.Dock.Layout;
using Meringue.Avalonia.Dock.ViewModels;
using NUnit.Framework;

namespace Meringue.Avalonia.Dock.Tests.Layout
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal sealed class DockLayoutConverterTests
    {
        [Test]
        public void BuildLayout_Throws_OnNullInput()
        {
            Assert.Throws<ArgumentNullException>(() => DockLayoutConverter.BuildLayout(null!));
        }

        [Test]
        public void BuildViewModel_Throws_OnNullLayout()
        {
            Assert.Throws<ArgumentNullException>(() => DockLayoutConverter.BuildViewModel(null!));
        }

        [Test]
        public void BuildViewModel_Throws_OnNullRootNode()
        {
            DockLayout layout = new()
            {
                MajorVersion = 1,
                MinorVersion = 0,
                RootNode = null!,
            };

            Assert.Throws<ArgumentNullException>(() => DockLayoutConverter.BuildViewModel(layout));
        }

        [Test]
        public void BuildViewModel_Throws_OnUnsupportedVersion()
        {
            DockLayout layout = new()
            {
                MajorVersion = 99,
                MinorVersion = 0,
                RootNode = new DockLayoutSplit(),
            };

            Assert.Throws<NotSupportedException>(() => DockLayoutConverter.BuildViewModel(layout));
        }

        [Test]
        public void SplitNode_RoundTrip_MatchesOriginal()
        {
            DockHostRootViewModel original = new(new DockSplitNodeViewModel
            {
                Id = "root",
                Orientation = Orientation.Horizontal,
                Sizes = [0.5, 0.5],
                Children =
                {
                    new DockTabNodeViewModel
                    {
                        Id = "tab1",
                        Tabs =
                        {
                            new DockToolViewModel { Id = "tool1", Header = "Tool One", IsPinned = true },
                        },
                    },
                    new DockTabNodeViewModel
                    {
                        Id = "tab2",
                        Tabs =
                        {
                            new DockToolViewModel { Id = "tool2", Header = "Tool Two", IsPinned = false },
                        },
                    },
                },
            });

            DockLayout layout = DockLayoutConverter.BuildLayout(original);
            DockHostRootViewModel rebuilt = DockLayoutConverter.BuildViewModel(layout);

            DockSplitNodeViewModel? split = rebuilt.HostRoot as DockSplitNodeViewModel;
            Assert.That(split, Is.Not.Null);
            Assert.That(split!.Children.Count, Is.EqualTo(2));

            DockTabNodeViewModel? tab1 = split.Children[0] as DockTabNodeViewModel;
            DockTabNodeViewModel? tab2 = split.Children[1] as DockTabNodeViewModel;
            Assert.That(tab1!.Tabs.First().Header, Is.EqualTo("Tool One"));
            Assert.That(tab2!.Tabs.First().Header, Is.EqualTo("Tool Two"));
        }

        [Test]
        public void BuildLayout_And_BuildViewModel_CorrectlyMapTabNode()
        {
            DockTabNodeViewModel tab = new()
            {
                Id = "tab-node",
                Tabs =
                {
                    new DockToolViewModel { Id = "t1", Header = "Header 1", IsPinned = true },
                    new DockToolViewModel { Id = "t2", Header = "Header 2", IsPinned = false },
                },
            };

            DockLayoutTab? layoutNode = GetPrivateBuildLayoutNode(tab) as DockLayoutTab;
            Assert.That(layoutNode, Is.Not.Null);
            Assert.That(layoutNode!.Tools.Count, Is.EqualTo(2));
            Assert.That(layoutNode.Tools[0].Header, Is.EqualTo("Header 1"));
        }

        [Test]
        public void BuildLayoutNode_Throws_OnUnsupportedType()
        {
            DummyNode unknownNode = new(); // Inherits DockNodeViewModel
            Assert.Throws<NotSupportedException>(() => GetPrivateBuildLayoutNode(unknownNode));
        }

        [Test]
        public void BuildViewModelNode_Throws_OnInvalidType()
        {
            DummyLayoutNode invalidNode = new(); // Inherits DockLayoutNode
            Assert.Throws<ArgumentException>(() =>
            {
                System.Reflection.MethodInfo method = typeof(DockLayoutConverter)
                    .GetMethod("BuildViewModelNode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!;

                _ = method.Invoke(null, [invalidNode]);
            });
        }

        private static DockLayoutNode GetPrivateBuildLayoutNode(DockNodeViewModel node)
        {
            System.Reflection.MethodInfo method = typeof(DockLayoutConverter)
                .GetMethod("BuildLayoutNode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!;

            return (DockLayoutNode)method.Invoke(null, [node])!;
        }

        private sealed class DummyNode : DockNodeViewModel
        {
        }

        private sealed class DummyLayoutNode : DockLayoutNode
        {
        }
    }
}
