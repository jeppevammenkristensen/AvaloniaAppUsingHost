using System.ComponentModel;
using System.Threading.Tasks;

namespace AvaloniaAppUsingHost.Infrastructure;

/// <summary>
/// Defines the lifecycle and tab metadata required for a screen page.
/// </summary>
public interface IScreenPage : INotifyPropertyChanged
{
    /// <summary>
    /// Gets the title displayed in the tab view.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Gets or sets whether the current screen can close.
    /// </summary>
    bool CanClose { get; set; }

    /// <summary>
    /// Performs work after the screen has been activated.
    /// </summary>
    Task OnActivatedAsync();

    /// <summary>
    /// Performs work before the screen is closed.
    /// </summary>
    Task CloseAsync();
}