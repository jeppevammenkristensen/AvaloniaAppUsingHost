using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Avalonia.Controls;
using AvaloniaAppUsingHost.Infrastructure;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaAppUsingHost.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] public partial ObservableCollection<Screen> Screens { get; set; }
    [ObservableProperty] public partial Screen? Screen { get; set; }
    
    
    [ObservableProperty] public partial ObservableCollection<ViewModelType> ViewModelTypes { get; set; }
    [ObservableProperty] public partial ViewModelType SelectedViewModelType { get; set; }


    private readonly IServiceLocator _locator;


    partial void OnScreenChanged(Screen? oldValue, Screen? newValue)
    {
        if (oldValue is { })
        {
            oldValue.PropertyChanged -= ScreenPropertyChanged;    
        }

        if (newValue is { })
        {
            newValue.PropertyChanged += ScreenPropertyChanged;   
        }
        
    }

    private void ScreenPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Screen.CanClose))
        {
            CloseCommand.NotifyCanExecuteChanged();
        }
    }


    public MainWindowViewModel(IServiceLocator locator)
    {
        _locator = locator;
        Screens = [];
        ViewModelTypes = [new ViewModelType("FirstViewModel", l => l.GetRequiredService<FirstControlViewModel>())];
        SelectedViewModelType = ViewModelTypes[0];
    }

    private bool CanExecuteLaunch()
    {
        return true;
    }

    [RelayCommand(CanExecute = nameof(CanExecuteLaunch))]
    private async Task Launch()
    {
        var screen = SelectedViewModelType.ScreenFunc(_locator);
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
        { 
            Screen = Screens[^1];
        }
        else
        {
            Screen = null;
        }
    }
}

public record ViewModelType(string DisplayName, Func<IServiceLocator,Screen> ScreenFunc)
{
    
}

