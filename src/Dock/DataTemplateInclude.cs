// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;

namespace Meringue.Avalonia.Dock
{
    /// <summary>
    /// Handle loading data templates from the assembly's resources.
    /// </summary>
    // Based on the thread https://github.com/AvaloniaUI/Avalonia/discussions/8426
    internal class DataTemplateInclude : IDataTemplate
    {
        /// <summary>
        /// Initializes static members of the <see cref="DataTemplateInclude"/> class.
        /// </summary>
        static DataTemplateInclude()
        {
            if (Application.Current is { } app)
            {
                Uri uri = new("avares://Meringue.Avalonia.Dock/Themes/Templates.axaml");
                DataTemplates templates = (DataTemplates)AvaloniaXamlLoader.Load(uri);
                app.DataTemplates.AddRange(templates);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTemplateInclude"/> class.
        /// </summary>
        /// <param name="baseUri">The base uri of the data templates resource to use.</param>
        public DataTemplateInclude(Uri baseUri)
        {
            this.LazyTemplates = new Lazy<DataTemplates>(
                () =>
                {
                    return this.Source == null
                        ? throw new InvalidOperationException("Source URI must be set before loading.")
                        : (DataTemplates)AvaloniaXamlLoader.Load(this.Source, baseUri);
                },
                isThreadSafe: true);
        }

        /// <summary>
        /// Gets or sets the source of the data templates.
        /// </summary>
        public Uri? Source { get; set; }

        /// <summary>
        /// Gets the lazy loader for the templates.
        /// </summary>
        private Lazy<DataTemplates> LazyTemplates { get; }

        /// <summary>
        /// Just a static method to ensure the static constructor is referenced.
        /// </summary>
        public static void InjectTemplates()
        {
        }

        /// <inheritdoc/>
        public Boolean Match(Object? data)
        {
            if (data is null)
            {
                return false;
            }

            return this.LazyTemplates.Value.Any(template => template.Match(data));
        }

        /// <inheritdoc/>
        public Control? Build(Object? data) =>
            this.LazyTemplates.Value.First(t => t.Match(data)).Build(data);
    }
}
