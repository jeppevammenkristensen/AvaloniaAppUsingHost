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