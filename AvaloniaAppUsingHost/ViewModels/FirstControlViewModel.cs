using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace AvaloniaAppUsingHost.ViewModels;

public partial class FirstControlViewModel(ILogger<FirstControlViewModel> logger) : Screen
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