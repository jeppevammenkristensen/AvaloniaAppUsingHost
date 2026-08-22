using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace AvaloniaAppUsingHost.Converters;

/// <summary>
/// Resolves a tree item's icon identifier to an application vector resource.
/// </summary>
public sealed class TreeItemConverter : IValueConverter
{
    /// <summary>
    /// Resolves the supplied icon identifier to its vector resource.
    /// </summary>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string iconId || string.IsNullOrWhiteSpace(iconId))
        {
            return null;
        }

        return Application.Current?.TryFindResource($"{iconId}Icon", Application.Current.ActualThemeVariant,
            out var icon) == true
            ? icon
            : null;
    }

    /// <summary>
    /// Returns a no-op result because icon resources are not converted back to identifiers.
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return BindingOperations.DoNothing;
    }
}
