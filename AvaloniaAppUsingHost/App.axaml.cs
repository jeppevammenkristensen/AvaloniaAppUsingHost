using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using AvaloniaAppUsingHost.Infrastructure;
using AvaloniaAppUsingHost.ViewModels;
using AvaloniaAppUsingHost.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AvaloniaAppUsingHost;

public partial class App : Application
{
    private IHost? _host;

    internal IHost GlobalHost => _host ?? throw new InvalidOperationException("Host has not been initialized");
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        try
        {
            _host = CreateHostBuilder().Build();
            
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
                // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                DisableAvaloniaDataAnnotationValidation();
                desktop.MainWindow = new MainWindow
                {
                    DataContext = GlobalHost.Services.GetRequiredService<MainWindowViewModel>()
                };
                desktop.Exit += async (_, _) =>
                {
                    await GlobalHost.StopAsync();
                    GlobalHost.Dispose();
                    _host = null;
                };
            }
            
            DataTemplates.Add(GlobalHost.Services.GetRequiredService<ViewLocator>());

            base.OnFrameworkInitializationCompleted();
            await GlobalHost.StartAsync();
        }
        catch (Exception e)
        {
            if (_host is {})
                GlobalHost.Services.GetRequiredService<ILogger<App>>().LogCritical(e, "Failed to start the application");
            
        }
    }

    private IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder(Environment.GetCommandLineArgs())
            .ConfigureServices((hostContext, services) =>
            {
                services
                    .AddTransient<IServiceLocator, ServiceCollectionServiceLocator>()
                    .AddTransient<ViewLocator>()
                    .AddSingleton<MainWindowViewModel>()
                    .AddTransient<FirstControlViewModel>()
                    .AddView<MainWindowViewModel, MainWindow>() // Note this might not be necessary
                    .AddView<FirstControlViewModel, FirstControl>();
            });
    }
    
    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}