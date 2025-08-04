// Copyright (C) Scott Kupec. All rights reserved.

using System;
using System.IO;
using System.Text;
using Meringue.Avalonia.Dock.Layout;
using NUnit.Framework;

namespace Meringue.Avalonia.Dock.ViewModels.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal sealed class DockLayoutRootViewModelTests
    {
        [Test]
        public void ApplyLayout_Retains_ToolContext()
        {
            String expectedContext = "CurrentContext";
            String toolId = "merge-test";

            // Build a layout root to apply the saved layout to.
            DockLayoutRootViewModel activeViewModel = new();
            _ = activeViewModel.CreateOrUpdateTool(toolId, "Header", "OriginalContext");

            // Save the layout.
            using MemoryStream stream = new();
            activeViewModel.SaveLayout(stream);
            stream.Position = 0;

            // Change the tools context.
            DockToolViewModel? tool = activeViewModel.HostRoot.HostRoot.FindTool(toolId);
            tool!.Context = expectedContext;

            // Apply the saved layout.
            Assert.That(
                activeViewModel.LoadLayout(stream),
                Is.True,
                "This test requires the saved layout to successfully be applied.");

            // Get the tool again.
            DockToolViewModel? finalTool = activeViewModel.HostRoot.HostRoot.FindTool(toolId);

            Assert.That(
                finalTool!.Context,
                Is.EqualTo(expectedContext),
                $"The {nameof(DockToolViewModel.Context)} of a tool, after applying a layout, should not change.");
        }

        [Test]
        public void Constructor_InitializesHostRootAndManager()
        {
            DockLayoutRootViewModel viewModel = new();

            Assert.That(
                viewModel.HostRoot,
                Is.Not.Null,
                $"{nameof(DockLayoutRootViewModel.HostRoot)} should be initialized correctly.");

            Assert.That(
                viewModel.LayoutManager,
                Is.Not.Null,
                $"{nameof(DockLayoutRootViewModel.LayoutManager)} should be initialized correctly.");

            Assert.That(
                viewModel.LayoutManager,
                Is.InstanceOf<JsonLayoutManager>(),
                $"{nameof(DockLayoutRootViewModel.LayoutManager)} should default to {nameof(JsonLayoutManager)}.");
        }

        [Test]
        public void CreateOrUpdateTool_CreatesNewTool_WhenNotExists()
        {
            DockLayoutRootViewModel viewModel = new();

            DockToolViewModel? tool = viewModel.CreateOrUpdateTool("tool1", "Header", new Object());

            Assert.That(
                tool,
                Is.Not.Null,
                $"{nameof(DockLayoutRootViewModel.CreateOrUpdateTool)} should insert a valid tool.");

            Assert.That(
                "tool1",
                Is.EqualTo(tool!.Id),
                $"{nameof(DockLayoutRootViewModel.CreateOrUpdateTool)} should use the provided is.");

            Assert.That(
                "Header",
                Is.EqualTo(tool.Header),
                $"{nameof(DockLayoutRootViewModel.CreateOrUpdateTool)} should use the provided header.");
        }

        [Test]
        public void CreateOrUpdateTool_Throws_OnInvalidInsertPolicy()
        {
            DockLayoutRootViewModel viewModel = new()
            {
                InsertPolicy = DockInsertPolicy.Error,
            };

            Assert.Throws<ArgumentOutOfRangeException>(
                () => viewModel.CreateOrUpdateTool("tool1", "Header", new Object(), "MissingParent"),
                $"{nameof(DockLayoutRootViewModel.CreateOrUpdateTool)} should throw if the parent is missing and the policy is {DockInsertPolicy.Error}.");
        }

        [Test]
        public void CreateOrUpdateTool_Throws_OnInvalidNodeType()
        {
            DockLayoutRootViewModel viewModel = new();

            // Inject invalid node type
            viewModel.HostRoot.HostRoot = new InvalidNode();

            Assert.Throws<InvalidOperationException>(
                () => viewModel.CreateOrUpdateTool("bad", "Header", new Object()),
                $"{nameof(viewModel.HostRoot.HostRoot)} should be required to be a {nameof(DockTabNodeViewModel)} or {nameof(DockSplitNodeViewModel)}.");
        }

        [Test]
        public void CreateOrUpdateTool_UpdatesExistingTool()
        {
            DockLayoutRootViewModel viewModel = new();
            Object originalContext = new();
            Object updatedContext = new();

            DockToolViewModel? tool = viewModel.CreateOrUpdateTool("tool1", "Header", originalContext);
            DockToolViewModel? updated = viewModel.CreateOrUpdateTool("tool1", "Updated", updatedContext);

            Assert.That(
                tool,
                Is.Not.Null,
                "This test requires a valid initial tool to run.");

            Assert.That(
                updated,
                Is.Not.Null,
                "This test requires a valid updated tool to run.");

            Assert.That(
                ReferenceEquals(tool, updated),
                Is.True,
                $"{nameof(DockToolViewModel)} should be the same object for updates.");

            Assert.That(
                "Updated",
                Is.EqualTo(updated!.Header),
                $"{nameof(DockToolViewModel.Header)} should be the updated value.");

            Assert.That(
                updatedContext,
                Is.EqualTo(updated.Context),
                $"{nameof(DockToolViewModel.Context)} should be the updated value.");
        }

        [Test]
        public void LoadLayout_FromFile_AppliesLayout()
        {
            DockLayoutRootViewModel viewModel = new();
            DockLayout layout = DockLayoutConverter.BuildLayout(viewModel.HostRoot);

            String tempFile = Path.GetTempFileName();

            try
            {
                viewModel.LayoutManager.Save(layout, tempFile);
                Boolean result = viewModel.LoadLayout(tempFile);
                Assert.That(
                    result,
                    Is.True,
                    "Loading a valid layout should report success.");
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Test]
        public void LoadLayout_FromStream_AppliesLayout()
        {
            DockLayoutRootViewModel viewModel = new();
            DockLayout layout = DockLayoutConverter.BuildLayout(viewModel.HostRoot);

            using MemoryStream stream = new();
            viewModel.LayoutManager.Save(layout, stream);
            stream.Position = 0;

            Boolean result = viewModel.LoadLayout(stream);

            Assert.That(
                result,
                Is.True,
                "Loading a valid layout should report success.");
        }

        [Test]
        public void SaveLayout_ToFile_WritesFile()
        {
            DockLayoutRootViewModel viewModel = new();

            String tempFile = Path.GetTempFileName();

            try
            {
                viewModel.SaveLayout(tempFile);

                String content = File.ReadAllText(tempFile);
                // CONSIDER: More through validation instead of just sanity testing the result.
                Assert.That(
                    content,
                    Does.Contain("rootNode"),
                    "The serialized should contain a root node.");
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [Test]
        public void SaveLayout_ToStream_WritesJson()
        {
            DockLayoutRootViewModel viewModel = new();

            using MemoryStream stream = new();
            viewModel.SaveLayout(stream);

            String json = Encoding.UTF8.GetString(stream.ToArray());

            // CONSIDER: More through validation instead of just sanity testing the result.
            Assert.That(
                json,
                Does.Contain("rootNode"),
                "The serialized should contain a root node.");
        }

        private sealed class InvalidNode : DockNodeViewModel
        {
        }
    }
}
