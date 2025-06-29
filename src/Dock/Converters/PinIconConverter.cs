// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Meringue.Avalonia.Dock.Converters
{
    /// <summary>Implements the pin and unpin converter used to display tab pin state..</summary>
    public class PinIconConverter : IValueConverter
    {
        /// <inheritdoc/>
        public Object Convert(Object? value, Type targetType, Object? parameter, CultureInfo culture)
            => value is Boolean pinned && pinned ? "\uE840" : "\uE77A";

        /// <inheritdoc/>
        public Object ConvertBack(Object? value, Type targetType, Object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
