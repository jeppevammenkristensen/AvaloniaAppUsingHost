using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaAppUsingHost.Infrastructure;
using AvaloniaAppUsingHost.Infrastructure.LongRunning;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Configuration;

namespace AvaloniaAppUsingHost.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IRecipient<ProgressDataMessage>, IRecipient<StatusDataMessage>
{
    private readonly IServiceLocator _locator;
    private readonly IConfiguration _configuration;


    public MainWindowViewModel(IServiceLocator locator, IConfiguration configuration)
    {
        _locator = locator;
        _configuration = configuration;
        Screens = [];
        Status = string.Empty;
        Title = _configuration["Title"] ?? "No title defined";
    }


    [ObservableProperty] public partial ObservableCollection<Screen> Screens { get; set; }

    [ObservableProperty] public partial Screen? Screen { get; set; }

    [ObservableProperty] public partial string Status { get; set; }

    /// <summary>
    ///     The progress. Should be between 0 and 100.
    /// </summary>
    [ObservableProperty]
    public partial double CurrentProgress { get; set; }

    [ObservableProperty] public partial bool Loaded { get; set; }
    [ObservableProperty] public partial string Title { get; set; }

    public void Receive(ProgressDataMessage message)
    {
        CurrentProgress = message.Value;
    }

    public void Receive(StatusDataMessage message)
    {
        Status = message.Value;
    }

    [RelayCommand]
    private async Task OnStartup(CancellationToken token)
    {
        OnActivated(); // hooks up implemented IRecipient

        CurrentProgress = 0;
        var longRunningTask = new DummyTask(Messenger);
        await longRunningTask.ExecuteTask(token);
        Loaded = true;
        await LaunchFirstCommand.ExecuteAsync(null);
    }


    private bool CanExecuteLaunchFirst()
    {
        return true;
    }

    [RelayCommand(CanExecute = nameof(CanExecuteLaunchFirst))]
    private async Task LaunchFirst()
    {
        var screen = _locator.GetRequiredService<FirstControlViewModel>();
        await Launch(screen);
    }

    partial void OnScreenChanged(Screen? oldValue, Screen? newValue)
    {
        if (oldValue is not null) oldValue.PropertyChanged -= ScreenPropertyChanged;

        if (newValue is not null) newValue.PropertyChanged += ScreenPropertyChanged;
    }

    private void ScreenPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Screen.CanClose)) CloseCommand.NotifyCanExecuteChanged();
    }

    private async Task Launch(Screen screen)
    {
        Screens.Add(screen);
        await screen.OnActivatedAsync();
        Screen = screen;
    }

    private bool CanExecuteClose(Screen? screen)
    {
        if (screen is null) return false;

        return screen.CanClose;
    }

    [RelayCommand(CanExecute = nameof(CanExecuteClose))]
    private async Task Close(Screen screen)
    {
        await screen.CloseAsync();
        Screens.Remove(screen);
        if (Screens.Count > 0)
            Screen = Screens[^1];
        else
            Screen = null;
    }
}