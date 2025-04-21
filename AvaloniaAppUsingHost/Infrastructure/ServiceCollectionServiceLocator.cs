using System;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaAppUsingHost.Infrastructure;

public class ServiceCollectionServiceLocator(IServiceProvider services) : IServiceLocator
{
    public T? GetService<T>()
    {
        return services.GetService<T>();
    }

    public T GetRequiredService<T>() where T : notnull
    {
        return services.GetRequiredService<T>();
    }
}