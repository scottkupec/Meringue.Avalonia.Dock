// Copyright (C) Scott Kupec. All rights reserved.

using System.IO;
using System.Text;
using System.Text.Json;
using NUnit.Framework;

namespace Meringue.Avalonia.Dock.Layout.UnitTests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal sealed class JsonLayoutManagerTests
    {
        [Test]
        public void Load_Throws_OnInvalidData()
        {
            JsonLayoutManager layoutManager = new();
            using MemoryStream stream = new(Encoding.UTF8.GetBytes("Not valid JSON"));

            Assert.That(
                () => layoutManager.Load(stream),
                Throws.TypeOf<JsonException>(),
                "Loading invalid json should not succeed.");
        }

        [Test]
        public void Load_Throws_OnNullStream()
        {
            JsonLayoutManager layoutManager = new();

            Assert.That(
                () => layoutManager.Load((Stream)null!),
                Throws.ArgumentNullException,
                "Loading null json should not succeed.");
        }

        [Test]
        public void Save_Throws_OnNullLayout()
        {
            JsonLayoutManager layoutManager = new();

            Assert.That(
                () => layoutManager.Save(null!, new MemoryStream()),
                Throws.ArgumentNullException,
                "Saving null json should not succeed.");
        }

        [Test]
        public void Save_Throws_OnNullOutputStream()
        {
            JsonLayoutManager layoutManager = new();
            DockLayout layout = new() { RootNode = new DockLayoutTab() };

            Assert.That(
                () => layoutManager.Save(layout, (Stream)null!),
                Throws.ArgumentNullException,
                "Saving json to a null stream should not succeed.");
        }

        [Test]
        public void Save_And_Load_RoundTrip_Works()
        {
            JsonLayoutManager layoutManager = new();

            DockLayoutTool originalTool = new()
            {
                Id = "toolA",
                Header = "Header A",
                IsPinned = true,
            };

            DockLayoutTab orignalLayoutTab = new()
            {
                Id = "tab1",
                Tools =
                    {
                        originalTool,
                    },
                SelectedId = originalTool.Id,
            };

            DockLayout originalLayout = new()
            {
                MajorVersion = 1,
                MinorVersion = 0,
                RootNode = orignalLayoutTab,
            };

            using MemoryStream stream = new();
            layoutManager.Save(originalLayout, stream);
            stream.Seek(0, SeekOrigin.Begin);
            DockLayout result = layoutManager.Load(stream);

            // Assert
            Assert.That(
                result,
                Is.Not.Null,
                "Loading valid json should succeed.");

            Assert.That(
                result.MajorVersion,
                Is.EqualTo(originalLayout.MajorVersion),
                $"{nameof(DockLayout.MajorVersion)} should be preserved.");

            Assert.That(
                result.MinorVersion,
                Is.EqualTo(originalLayout.MinorVersion),
                $"{nameof(DockLayout.MinorVersion)} should be preserved.");

            Assert.That(
                result.RootNode,
                Is.TypeOf<DockLayoutTab>(),
                $"{nameof(DockLayout.RootNode)} should be of the correct type.");

            DockLayoutTab tab = (DockLayoutTab)result.RootNode!;
            Assert.That(
                tab.Id,
                Is.EqualTo(orignalLayoutTab.Id),
                $"{nameof(DockLayoutTab.Id)} should be preserved.");

            Assert.That(
                tab.SelectedId,
                Is.EqualTo(orignalLayoutTab.SelectedId),
                $"{nameof(DockLayoutTab.SelectedId)} should be preserved.");

            Assert.That(
                tab.Tools.Count,
                Is.EqualTo(orignalLayoutTab.Tools.Count),
                $"The number of tools should be correctly preserved.");

            Assert.That(
                tab.Tools[0].Header,
                Is.EqualTo(originalTool.Header),
                $"The value for {nameof(DockLayoutTool.Header)} should be preserved.");

            Assert.That(
                tab.Tools[0].Id,
                Is.EqualTo(originalTool.Id),
                $"The value for {nameof(DockLayoutTool.Id)} should be preserved.");

            Assert.That(
                tab.Tools[0].IsPinned,
                Is.EqualTo(originalTool.IsPinned),
                $"The value for {nameof(DockLayoutTool.IsPinned)} should be preserved.");
        }
    }
}
