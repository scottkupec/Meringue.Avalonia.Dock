// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using System.IO;
using System.Text;
using Meringue.Avalonia.Dock.Layout;
using Meringue.Avalonia.Dock.ViewModels;
using Moq;
using NUnit.Framework;

namespace Meringue.Avalonia.Dock.Tests.ViewModels
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal sealed class DockLayoutRootViewModelTests
    {
        [Test]
        public void ApplyLayout_MergesContextCorrectly()
        {
            DockLayoutRootViewModel viewModel = new();
            _ = viewModel.CreateOrUpdateTool("merge-test", "Header", "OldContext");

            DockLayout layout = DockLayoutConverter.BuildLayout(viewModel.HostRoot);

            // Create a dummy DockLayoutRootViewModel to test if updated
            DockHostRootViewModel updatedRoot = DockLayoutConverter.BuildViewModel(layout);
            DockToolViewModel? tool = updatedRoot.HostRoot.FindTool("merge-test");
            tool!.Context = "NewContext";

            // Patch layout manager to return updatedRoot
            Mock<IDockLayoutManager> mockManager = new();
            mockManager.Setup(x => x.Load(It.IsAny<Stream>())).Returns(layout);
            viewModel.LayoutManager = mockManager.Object;

            using MemoryStream ms = new();
            viewModel.SaveLayout(ms);
            ms.Position = 0;
            _ = viewModel.LoadLayout(ms);

            DockToolViewModel? finalTool = viewModel.HostRoot.HostRoot.FindTool("merge-test");
            Assert.That("NewContext", Is.EqualTo(finalTool!.Context));
        }

        [Test]
        public void Constructor_InitializesHostRootAndManager()
        {
            DockLayoutRootViewModel viewModel = new();

            Assert.That(viewModel.HostRoot, Is.Not.Null);
            Assert.That(viewModel.LayoutManager, Is.Not.Null);
            Assert.That(viewModel.LayoutManager, Is.InstanceOf<JsonLayoutManager>());
        }

        [Test]
        public void CreateOrUpdateTool_CreatesNewTool_WhenNotExists()
        {
            DockLayoutRootViewModel viewModel = new();

            DockToolViewModel? tool = viewModel.CreateOrUpdateTool("tool1", "Header", new Object());

            Assert.That(tool, Is.Not.Null);
            Assert.That("tool1", Is.EqualTo(tool!.Id));
            Assert.That("Header", Is.EqualTo(tool.Header));
        }

        [Test]
        public void CreateOrUpdateTool_Throws_OnInvalidInsertPolicy()
        {
            DockLayoutRootViewModel viewModel = new()
            {
                InsertPolicy = DockInsertPolicy.Error,
            };

            Assert.Throws<ArgumentOutOfRangeException>(
                () => viewModel.CreateOrUpdateTool("tool1", "Header", new Object(), "MissingParent"));
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
                Assert.That(result, Is.True);
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

            using MemoryStream ms = new();
            viewModel.LayoutManager.Save(layout, ms);
            ms.Position = 0;

            Boolean result = viewModel.LoadLayout(ms);

            Assert.That(result, Is.True);
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
                Assert.That(content, Does.Contain("rootNode"));
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
            Assert.That(json, Does.Contain("rootNode"));
        }

        private sealed class InvalidNode : DockNodeViewModel
        {
        }
    }
}
