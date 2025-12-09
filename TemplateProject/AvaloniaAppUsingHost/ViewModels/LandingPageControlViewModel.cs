using System.Threading.Tasks;
using AvaloniaAppUsingHost.Infrastructure;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace AvaloniaAppUsingHost.ViewModels;

public partial class LandingPageControlViewModel(ILogger<LandingPageControlViewModel> logger) : Screen
{
    public override string Title => "First Control";
    [ObservableProperty] public partial bool PreventClose { get; set; }

    partial void OnPreventCloseChanged(bool value)
    {
        CanClose = !value;
    }

    public override Task OnActivatedAsync()
    {
        logger.LogInformation("FirstControlViewModel OnActivatedAsync");
        return Task.CompletedTask;
    }

    public override Task CloseAsync()
    {
        logger.LogInformation("FirstControlViewModel Closed");
        return Task.CompletedTask;
    }
}