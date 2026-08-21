using System.Threading.Tasks;
using AvaloniaAppUsingHost.Views;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaAppUsingHost.Infrastructure;

/// <summary>
/// This is a screen displayed in the <see cref="MainWindow"/>
/// </summary>
/// <remarks>To register a coupling between a view model and a View use the extension method
/// <see cref="ViewLocatorHelpers.AddViewModelAndRegisterView"/> in the <see cref="App.RegisterViews"/> part
/// </remarks>
public abstract partial class ScreenPage : ViewModelBase, IScreenPage
{
    /// <summary>
    /// The title displayed in the tab view
    /// </summary>
    public abstract string Title { get; }

    /// <summary>
    /// Signal if the current screen can close
    /// </summary>
    [ObservableProperty]
    public partial bool CanClose { get; set; } = true;

    /// <summary>
    /// Override this to perform an operation after an instance of the given
    /// screen page had been activated
    /// </summary>
    /// <returns></returns>
    public virtual Task OnActivatedAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Perform an operation before the screen is closed
    /// </summary>
    /// <returns></returns>
    public virtual Task CloseAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Submit a status message that will be displayed in the
    /// bottom of the screen
    /// </summary>
    /// <param name="message">The message to display.</param>
    protected void SetStatusMessage(string message)
    {
        Messenger.SendInformation(message);
    }

    /// <summary>
    /// Submit an error message that will be displayed in the
    /// bottom of the screen
    /// </summary>
    /// <param name="message">The message to display.</param>
    protected void SetErrorMessage(string message)
    {
        Messenger.SendError(message);
    }
}