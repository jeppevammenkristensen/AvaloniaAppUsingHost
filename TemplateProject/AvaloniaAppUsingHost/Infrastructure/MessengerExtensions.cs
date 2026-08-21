using AvaloniaAppUsingHost.Infrastructure.LongRunning;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaAppUsingHost.Infrastructure;

/// <summary>
/// Provides helpers for publishing application status messages.
/// </summary>
public static class MessengerExtensions
{
    extension(IMessenger messenger)
    {
        /// <summary>
        /// Publishes an informational status message.
        /// </summary>
        public void SendInformation(string message)
        {
            messenger.Send(new StatusValueDataMessage(new StatusMessage(message, StatusType.Info)));
        }

        /// <summary>
        /// Publishes an error status message.
        /// </summary>
        public void SendError(string message)
        {
            messenger.Send(new StatusValueDataMessage(new StatusMessage(message, StatusType.Error)));
        }

        /// <summary>
        /// Publishes a success status message.
        /// </summary>
        public void SendSuccess(string message)
        {
            messenger.Send(new StatusValueDataMessage(new StatusMessage(message, StatusType.Success)));
        }
    }
}