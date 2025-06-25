// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using Avalonia;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Meringue.Avalonia.Dock.Themes
{
    /// <summary>
    /// Theme file for Meringue.Avalonia.Dock.
    /// </summary>
    public class MeringueDockTheme : Styles
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeringueDockTheme"/> class.
        /// </summary>
        public MeringueDockTheme()
        {
            MeringueDockTheme.InjectTemplates(new System.Uri("avares://Meringue.Avalonia.Dock/Themes/DataTemplates.axaml"));
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Injects the data templates into the current application.
        /// </summary>
        /// <param name="templateUri">The uri of the templates to inject.</param>
        public static void InjectTemplates(Uri templateUri)
        {
            if (Application.Current is { } app)
            {
                DataTemplates templates = (DataTemplates)AvaloniaXamlLoader.Load(templateUri);
                app.DataTemplates.AddRange(templates);
            }
        }
    }
}
