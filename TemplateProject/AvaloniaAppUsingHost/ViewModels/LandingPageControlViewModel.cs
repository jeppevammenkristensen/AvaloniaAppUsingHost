using System;
using System.Threading.Tasks;
using AvaloniaAppUsingHost.Infrastructure;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace AvaloniaAppUsingHost.ViewModels;

/// <summary>
/// An initial landing page. 
/// </summary>
/// <param name="logger"></param>
public partial class LandingPageControlViewModel(ILogger<LandingPageControlViewModel> logger) : ScreenPage
{
    public override string Title => "Landing page";
    [ObservableProperty] public partial bool PreventClose { get; set; }

    partial void OnPreventCloseChanged(bool value)
    {
        CanClose = !value;
    }

    public override Task OnActivatedAsync()
    {
        logger.LogInformation("On Activated");
        return Task.CompletedTask;
    }

    public override Task CloseAsync()
    {
        logger.LogInformation("On Close");
        return Task.CompletedTask;
    }
}