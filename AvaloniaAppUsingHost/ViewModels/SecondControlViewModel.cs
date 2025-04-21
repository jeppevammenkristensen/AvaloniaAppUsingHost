using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaAppUsingHost.ViewModels;

public sealed partial class SecondControlViewModel : Screen
{
    public SecondControlViewModel()
    {
        DisplayText = $"{Title} {DateTime.Now:T}";
    }
    
    public override string Title => $"Second control";
    [ObservableProperty] public partial string DisplayText { get; set; }
}