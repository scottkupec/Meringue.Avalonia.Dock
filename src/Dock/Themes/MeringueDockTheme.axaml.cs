// Copyright (C) Meringue Project Team. All rights reserved.

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
            _ = new DataTemplateInclude(new System.Uri("avares://Meringue.Avalonia.Dock/Themes/Templates.axaml"));
            AvaloniaXamlLoader.Load(this);
        }

        /////// <summary>
        /////// Gets the included templates.
        /////// </summary>
        ////private DataTemplateInclude IncludedTemplates { get; } = new DataTemplateInclude(new System.Uri("avares://Meringue.Avalonia.Dock/Themes/Templates.axaml"));
    }
}
