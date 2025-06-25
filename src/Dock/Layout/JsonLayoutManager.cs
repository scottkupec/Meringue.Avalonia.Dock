// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Meringue.Avalonia.Dock.Layout
{
    /// <summary>
    /// A default layout manager implementation using System.Text.Json.
    /// </summary>
    public class JsonLayoutManager : IDockLayoutManager
    {
        /// <summary>
        /// Defins the <see cref="JsonSerializerOptions"/> used when loading and saving JSON.
        /// </summary>
        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        /// <inheritdoc/>
        public DockLayout Load(String filename)
        {
            using FileStream stream = File.OpenRead(filename);
            return this.Load(stream);
        }

        /// <inheritdoc/>
        public DockLayout Load(Stream input)
        {
            ArgumentNullException.ThrowIfNull(input);

            DockLayout? layout = JsonSerializer.Deserialize<DockLayout>(input, Options);
            return layout ?? throw new InvalidDataException("Could not deserialize DockLayout.");
        }

        /// <inheritdoc/>
        public void Save(DockLayout layout, String filename)
        {
            ArgumentNullException.ThrowIfNull(layout);

            using FileStream stream = File.Create(filename);
            this.Save(layout, stream);
        }

        /// <inheritdoc/>
        public void Save(DockLayout layout, Stream output)
        {
            ArgumentNullException.ThrowIfNull(layout);
            ArgumentNullException.ThrowIfNull(output);

            JsonSerializer.Serialize(output, layout, Options);
        }
    }
}
