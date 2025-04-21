using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaAppUsingHost.ViewModels;

public abstract partial class Screen : ViewModelBase
{
    public abstract string Title { get; }

    public virtual Task OnActivatedAsync()
    {
        return Task.CompletedTask;
    }

    [ObservableProperty] public partial bool CanClose { get; set; } = true;

    public virtual Task CloseAsync()
    {
        return Task.CompletedTask;
    }
}