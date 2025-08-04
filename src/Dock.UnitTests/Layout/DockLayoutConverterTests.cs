// Copyright (C) Scott Kupec. All rights reserved.

using System;
using System.Linq;
using System.Reflection;
using Avalonia.Layout;
using Meringue.Avalonia.Dock.ViewModels;
using NUnit.Framework;

namespace Meringue.Avalonia.Dock.Layout.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal sealed class DockLayoutConverterTests
    {
        [Test]
        public void BuildLayout_And_BuildViewModel_CorrectlyMapTabNode()
        {
            DockTabNodeViewModel viewModel = new()
            {
                Id = "tab-node",
                Tabs =
                {
                    new DockToolViewModel { Id = "t1", Header = "Header 1", IsPinned = true },
                    new DockToolViewModel { Id = "t2", Header = "Header 2", IsPinned = false },
                },
            };

            DockLayoutTab? layoutNode = GetPrivateBuildLayoutNode(viewModel) as DockLayoutTab;

            Assert.That(
                layoutNode,
                Is.Not.Null,
                $"A valid {nameof(DockLayoutTab)} should be returned.");

            Assert.That(
                layoutNode!.Tools.Count,
                Is.EqualTo(viewModel.Tabs.Count),
                $"The returned {nameof(DockLayoutTab)} should have the correct number of tabs.");

            Assert.That(
                layoutNode.Tools[0].Header,
                Is.EqualTo(viewModel.Tabs[0].Header),
                $"The returned {nameof(DockToolViewModel)} should have the expected {nameof(DockToolViewModel.Header)}.");

            // CONSIDER: More complete validation instead of just sanity checking.
        }

        [Test]
        public void BuildLayout_Throws_OnNullInput()
        {
            Assert.Throws<ArgumentNullException>(
                () => DockLayoutConverter.BuildLayout(null!),
                $"It should not be possible to build a {nameof(DockLayout)} from a null {nameof(DockHostRootViewModel)}.");
        }

        [Test]
        public void BuildLayoutNode_Throws_OnUnsupportedType()
        {
            DummyNode unknownNode = new();
            Exception? thrownException = Assert.Throws<TargetInvocationException>(() => GetPrivateBuildLayoutNode(unknownNode));

            Assert.That(
                thrownException!.InnerException,
                Is.TypeOf<NotSupportedException>(),
                $"The inner exception should be of type {nameof(NotSupportedException)}.");
        }

        [Test]
        public void BuildViewModel_Throws_OnNullLayout()
        {
            Assert.Throws<ArgumentNullException>(
                () => DockLayoutConverter.BuildViewModel(null!),
                $"It should not be possible to build a {nameof(DockHostRootViewModel)} from a null {nameof(DockLayout)}.");
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

            Assert.Throws<ArgumentNullException>(
                () => DockLayoutConverter.BuildViewModel(layout),
                $"It should not be possible to build a {nameof(DockHostRootViewModel)} from a {nameof(DockLayout)} with a null {nameof(DockLayout.RootNode)}.");
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

            Assert.Throws<NotSupportedException>(
                () => DockLayoutConverter.BuildViewModel(layout),
                $"It should not be possible to build a {nameof(DockHostRootViewModel)} from a {nameof(DockLayout)} with a unsupported version.");
        }

        [Test]
        public void BuildViewModelNode_Throws_OnInvalidType()
        {
            DummyLayoutNode invalidNode = new();

            Exception? thrownException = Assert.Throws<TargetInvocationException>(
                () =>
                {
                    MethodInfo method = typeof(DockLayoutConverter)
                        .GetMethod("BuildViewModelNode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!;

                    _ = method.Invoke(null, [invalidNode]);
                });

            Assert.That(
                thrownException!.InnerException,
                Is.TypeOf<ArgumentException>(),
                $"The inner exception should be of type {nameof(ArgumentException)}.");
        }

        [Test]
        public void SplitNode_RoundTrip_MatchesOriginal()
        {
            DockHostRootViewModel source = new(new DockSplitNodeViewModel
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

            DockLayout layout = DockLayoutConverter.BuildLayout(source);

            DockSplitNodeViewModel? rebuilt = DockLayoutConverter.BuildViewModel(layout).HostRoot as DockSplitNodeViewModel;
            DockSplitNodeViewModel? original = source.HostRoot as DockSplitNodeViewModel;

            Assert.That(
                rebuilt,
                Is.Not.Null,
                $"The built {nameof(DockHostRootViewModel)} should have a valid {nameof(DockSplitNodeViewModel)} as the {nameof(DockHostRootViewModel.HostRoot)}.");

            Assert.That(
                rebuilt!.Children.Count,
                Is.EqualTo(original!.Children.Count),
                $"The rebuild {nameof(DockHostRootViewModel)} should have the correct value of {nameof(DockSplitNodeViewModel.Children)}");

            for (Int32 i = 0; i < rebuilt!.Children.Count; i++)
            {
                DockTabNodeViewModel? originalTab = original.Children[i] as DockTabNodeViewModel;
                DockTabNodeViewModel? rebuiltTab = rebuilt.Children[i] as DockTabNodeViewModel;

                // CONSIDER: More complete validation instead of just sanity checking.
                Assert.That(
                    rebuiltTab!.Tabs.First().Header,
                    Is.EqualTo(originalTab!.Tabs.First().Header),
                    $"The {nameof(DockToolViewModel.Header)} for the rebuilt {nameof(DockTabNodeViewModel)} should be the same as for the origintal {nameof(DockTabNodeViewModel)}.");
            }
        }

        private static DockLayoutNode GetPrivateBuildLayoutNode(DockNodeViewModel node)
        {
            MethodInfo method = typeof(DockLayoutConverter)
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
