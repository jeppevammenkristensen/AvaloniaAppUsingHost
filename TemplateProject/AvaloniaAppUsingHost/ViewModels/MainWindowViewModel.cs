using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using AvaloniaAppUsingHost.Infrastructure;
using AvaloniaAppUsingHost.Infrastructure.LongRunning;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Configuration;

namespace AvaloniaAppUsingHost.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IRecipient<ProgressDataMessage>, IRecipient<StatusValueDataMessage>
{
    private readonly IServiceLocator _locator;


    public MainWindowViewModel(IServiceLocator locator, IConfiguration configuration)
    {
        _locator = locator;
        Screens = [];
        Status = string.Empty;
        Title = configuration["Title"] ?? "No title defined";
    }


    [ObservableProperty] public partial ObservableCollection<ScreenPage> Screens { get; set; }

    [ObservableProperty] public partial ScreenPage? Screen { get; set; }

    [ObservableProperty] public partial string Status { get; set; }

    /// <summary>
    ///     The progress. Should be between 0 and 100.
    /// </summary>
    [ObservableProperty]
    public partial double CurrentProgress { get; set; }

    [ObservableProperty] public partial bool Loaded { get; set; }
    [ObservableProperty] public partial string Title { get; set; }
    [NotifyPropertyChangedFor(nameof(DisplayInfo))] [NotifyPropertyChangedFor(nameof(DisplayInfoError))] [ObservableProperty] public partial StatusType StatusType { get; set; }

    public bool DisplayInfo => StatusType == StatusType.Info;
    public bool DisplayInfoError => StatusType == StatusType.Error;

    public void Receive(ProgressDataMessage message)
    {
        CurrentProgress = message.Value;
    }

    public void Receive(StatusValueDataMessage message)
    {
        Status = message.Value.Value;
        StatusType = message.Value.StatusType;
        
    }

    [RelayCommand]
    private async Task OnStartup(CancellationToken token)
    {
        OnActivated(); // hooks up implemented IRecipient

        CurrentProgress = 0;
        var longRunningTask = new DummyTask(Messenger);
        await longRunningTask.ExecuteTask(token);
        Loaded = true;
        await LaunchPrimaryCommand.ExecuteAsync(null);
    }


    private bool CanExecuteLaunchPrimary()
    {
        return true;
    }

    [RelayCommand(CanExecute = nameof(CanExecuteLaunchPrimary))]
    private async Task LaunchPrimary()
    {
        var screen = _locator.GetRequiredService<LandingPageControlViewModel>();
        await Launch(screen);
    }

    partial void OnScreenChanged(ScreenPage? oldValue, ScreenPage? newValue)
    {
        if (oldValue is not null) oldValue.PropertyChanged -= ScreenPropertyChanged;

        if (newValue is not null) newValue.PropertyChanged += ScreenPropertyChanged;
    }

    private void ScreenPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Screen.CanClose)) CloseCommand.NotifyCanExecuteChanged();
    }

    private async Task Launch(ScreenPage screenPage)
    {
        Screens.Add(screenPage);
        await screenPage.OnActivatedAsync();
        Screen = screenPage;
    }

    private bool CanExecuteClose(ScreenPage? screen)
    {
        if (screen is null) return false;

        return screen.CanClose;
    }

    [RelayCommand(CanExecute = nameof(CanExecuteClose))]
    private async Task Close(ScreenPage screenPage)
    {
        await screenPage.CloseAsync();
        Screens.Remove(screenPage);
        if (Screens.Count > 0)
            Screen = Screens[^1];
        else
            Screen = null;
    }
}