// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Meringue.Avalonia.Dock.Converters
{
    /// <summary>Converter for enabling dynamic values for "IsVisible=".</summary>
    public class InverseBooleanConverter : IValueConverter
    {
        /// <inheritdoc/>
        public Object Convert(Object? value, Type targetType, Object? parameter, CultureInfo culture)
            => value is Boolean b ? !b : AvaloniaProperty.UnsetValue;

        /// <inheritdoc/>
        public Object ConvertBack(Object? value, Type targetType, Object? parameter, CultureInfo culture)
            => value is Boolean b ? !b : AvaloniaProperty.UnsetValue;
    }
}
