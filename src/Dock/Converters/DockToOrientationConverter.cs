// Copyright (C) Meringue Project Team. All rights reserved.

using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using DockLocation = Avalonia.Controls.Dock;

namespace Meringue.Avalonia.Dock.Converters
{
    /// <summary>Implements the converter for determing orientation of the unpinned tabs area..</summary>
    public class DockToOrientationConverter : IValueConverter
    {
        /// <inheritdoc/>
        public Object? Convert(Object? value, Type targetType, Object? parameter, CultureInfo culture)
        {
            if (value is DockLocation dock)
            {
                return dock is DockLocation.Left or DockLocation.Right ? Orientation.Vertical : Orientation.Horizontal;
            }

            return Orientation.Horizontal;
        }

        /// <inheritdoc/>
        public Object ConvertBack(Object? value, Type targetType, Object? parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
