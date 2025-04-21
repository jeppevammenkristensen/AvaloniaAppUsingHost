using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using AvaloniaAppUsingHost.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaAppUsingHost.Infrastructure;

public static class ViewLocatorHelpers
{
    public static IServiceCollection AddView<TViewModel, TView>(this IServiceCollection collection) where TViewModel : ViewModelBase where TView : Control, new()
    {
        collection.AddSingleton(new ViewLocator.ViewLocatorDescriptor(typeof(TViewModel), () => new TView()));
        return collection;
    }
}

public class ViewLocator : IDataTemplate
{
    private readonly Dictionary<Type, Func<Control>> _dic;

    public ViewLocator(IEnumerable<ViewLocatorDescriptor> descriptors)
    {
        _dic = descriptors.ToDictionary(x => x.ViewModelType, x => x.Factory);
    }

    public Control? Build(object? param) => _dic[param!.GetType()]();

    public bool Match(object? data) => data is {} && _dic.ContainsKey(data.GetType());
    public record ViewLocatorDescriptor(Type ViewModelType, Func<Control> Factory);
}