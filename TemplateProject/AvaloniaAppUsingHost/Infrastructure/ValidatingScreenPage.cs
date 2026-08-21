using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaAppUsingHost.Infrastructure;

/// <summary>
/// Provides tab-screen behavior with data-annotation validation support.
/// </summary>
public abstract class ValidatingScreenPage : ObservableValidator, IScreenPage
{
    /// <summary>
    /// Gets the title displayed in the tab view.
    /// </summary>
    public abstract string Title { get; }

    /// <summary>
    /// Gets or sets whether the current screen can close.
    /// </summary>
    public virtual bool CanClose { get; set; } = true;

    /// <summary>
    /// Performs work after the screen has been activated.
    /// </summary>
    public virtual Task OnActivatedAsync() => Task.CompletedTask;

    /// <summary>
    /// Performs work before the screen is closed.
    /// </summary>
    public virtual Task CloseAsync() => Task.CompletedTask;
}