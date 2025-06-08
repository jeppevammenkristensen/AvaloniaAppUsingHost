using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaAppUsingHost.Infrastructure;

public abstract partial class Screen : ViewModelBase
{
    public abstract string Title { get; }

    [ObservableProperty] public partial bool CanClose { get; set; } = true;

    public virtual Task OnActivatedAsync()
    {
        return Task.CompletedTask;
    }

    public virtual Task CloseAsync()
    {
        return Task.CompletedTask;
    }
}