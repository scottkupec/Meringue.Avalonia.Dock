// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Meringue.Avalonia.Dock.Converters
{
    /// <summary>Converter for enabling dynamic values for "IsVisible=".</summary>
    /// <example>IsVisible="{Binding IsBackButtonVisible, Converter={StaticResource BooleanToVisibilityConverter}}.</example>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <inheritdoc/>
        public Object Convert(Object? value, Type targetType, Object? parameter, CultureInfo culture) =>
            value is Boolean boolValue && boolValue;

        /// <inheritdoc/>
        public Object ConvertBack(Object? value, Type targetType, Object? parameter, CultureInfo culture) =>
            false;
    }
}
