using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AvaloniaAppUsingHost.Infrastructure.LongRunning;

/// <summary>
/// Identifies the presentation style of a status message.
/// </summary>
public enum StatusType
{
    /// <summary>
    /// Indicates informational status.
    /// </summary>
    Info,

    /// <summary>
    /// Indicates an error status.
    /// </summary>
    Error,

    /// <summary>
    /// Indicates successful completion.
    /// </summary>
    Success
}

/// <summary>
/// Represents text and presentation style for a status update.
/// </summary>
public record StatusMessage(string Value, StatusType StatusType);

/// <summary>
/// Communicates a status update through the messenger.
/// </summary>
public class StatusValueDataMessage(StatusMessage value) : ValueChangedMessage<StatusMessage>(value);