// Copyright (C) Meringue Project Team. All rights reserved.

using System.IO;
using System.Text;
using System.Text.Json;
using Meringue.Avalonia.Dock.Layout;
using NUnit.Framework;

namespace Meringue.Avalonia.Dock.Tests.Layout
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal sealed class JsonLayoutManagerTests
    {
        [Test]
        public void Load_Throws_OnNullStream()
        {
            JsonLayoutManager layoutManager = new();
            Assert.That(() => layoutManager.Load((Stream)null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Save_Throws_OnNullLayout()
        {
            JsonLayoutManager layoutManager = new();
            Assert.That(() => layoutManager.Save(null!, new MemoryStream()), Throws.ArgumentNullException);
        }

        [Test]
        public void Save_Throws_OnNullOutputStream()
        {
            JsonLayoutManager layoutManager = new();
            DockLayout layout = new() { RootNode = new DockLayoutTab() };
            Assert.That(() => layoutManager.Save(layout, (Stream)null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Load_Throws_OnInvalidData()
        {
            JsonLayoutManager layoutManager = new();
            using MemoryStream stream = new(Encoding.UTF8.GetBytes("Not valid JSON"));
            Assert.That(() => layoutManager.Load(stream), Throws.TypeOf<JsonException>());
        }

        [Test]
        public void Save_And_Load_RoundTrip_Works()
        {
            JsonLayoutManager layoutManager = new();
            // Arrange
            DockLayout originalLayout = new()
            {
                MajorVersion = 1,
                MinorVersion = 0,
                RootNode = new DockLayoutTab
                {
                    Id = "tab1",
                    Tools =
                    {
                        new DockLayoutTool
                        {
                            Id = "toolA",
                            Header = "Header A",
                            IsPinned = true,
                        },
                    },
                    SelectedId = "toolA",
                },
            };

            using MemoryStream stream = new();

            // Act
            layoutManager.Save(originalLayout, stream);
            stream.Seek(0, SeekOrigin.Begin);
            DockLayout result = layoutManager.Load(stream);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.MajorVersion, Is.EqualTo(1));
            Assert.That(result.MinorVersion, Is.EqualTo(0));
            Assert.That(result.RootNode, Is.TypeOf<DockLayoutTab>());

            DockLayoutTab tab = (DockLayoutTab)result.RootNode!;
            Assert.That(tab.Id, Is.EqualTo("tab1"));
            Assert.That(tab.Tools.Count, Is.EqualTo(1));
            Assert.That(tab.Tools[0].Id, Is.EqualTo("toolA"));
            Assert.That(tab.Tools[0].Header, Is.EqualTo("Header A"));
            Assert.That(tab.Tools[0].IsPinned, Is.True);
            Assert.That(tab.SelectedId, Is.EqualTo("toolA"));
        }
    }
}
