using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AvaloniaAppUsingHost.Infrastructure;

public static class ViewLocatorHelpers
{
    /// <summary>
    ///     Registers a view model with the specified lifetime scope and associates it with a corresponding view.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model to register.</typeparam>
    /// <typeparam name="TView">The type of the view to associate with the view model.</typeparam>
    /// <param name="collection">The IServiceCollection to which the view model and view will be added.</param>
    /// <param name="scope">
    ///     The scope of the view model registration. Use <see cref="ViewModelScope.Transient" /> for transient reuse
    ///     or <see cref="ViewModelScope.Singleton" /> for a single instance in the container.
    /// </param>
    /// <returns>The IServiceCollection instance for method chaining.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the specified <paramref name="scope" /> is invalid.</exception>
    /// <remarks>
    ///     When a view is associated with a viewmodel it will be added as DataTemplate so you can bind
    ///     like this <![CDATA[<ContentControl Grid.Row="1" Content="{Binding Screen}" ></ContentControl>]]>
    ///     In a tab control if you bind a viewmodel to the SeledtedItem property it will be automatically resolved
    /// </remarks>
    internal static IServiceCollection AddViewModelAndRegisterView<TViewModel, TView>(
        this IServiceCollection collection, ViewModelScope scope)
        where TViewModel : ViewModelBase where TView : Control, new()
    {
        switch (scope)
        {
            case ViewModelScope.Transient:
                collection.AddTransient<TViewModel>();
                break;
            case ViewModelScope.Singleton:
                collection.AddSingleton<TViewModel>();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(scope), scope, null);
        }

        return collection.AddView<TViewModel, TView>();
    }

    /// <summary>
    ///     Registers a view and associates it with a specific view model type.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model that the view is associated with.</typeparam>
    /// <typeparam name="TView">The type of the view to register.</typeparam>
    /// <param name="collection">The IServiceCollection to register the view into.</param>
    /// <returns>The IServiceCollection instance for method chaining.</returns>
    /// <remarks>
    ///     When a view is associated with a viewmodel it will be added as DataTemplate so you can bind
    ///     like this <![CDATA[<ContentControl Grid.Row="1" Content="{Binding Screen}" ></ContentControl>]]>
    /// </remarks>
    private static IServiceCollection AddView<TViewModel, TView>(this IServiceCollection collection)
        where TViewModel : ViewModelBase where TView : Control, new()
    {
        collection.AddSingleton(new ViewLocator.ViewLocatorDescriptor(typeof(TViewModel), () => new TView()));
        return collection;
    }
}

internal enum ViewModelScope
{
    Transient,
    Singleton
}

public class ViewLocator : IDataTemplate
{
    private readonly Dictionary<Type, Func<Control>> _dic;
    private readonly ILogger<ViewLocator> _logger;

    public ViewLocator(IEnumerable<ViewLocatorDescriptor> descriptors, ILogger<ViewLocator> logger)
    {
        _logger = logger;
        _dic = descriptors.ToDictionary(x => x.ViewModelType, x => x.Factory);
    }

    public Control Build(object? param)
    {
        return _dic[param!.GetType()]();
    }

    public bool Match(object? data)
    {
        return data is not null && _dic.ContainsKey(data.GetType());
    }

    public record ViewLocatorDescriptor(Type ViewModelType, Func<Control> Factory);
}