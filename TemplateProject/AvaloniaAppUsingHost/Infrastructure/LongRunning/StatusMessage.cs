namespace AvaloniaAppUsingHost.Infrastructure.LongRunning;

/// <summary>
/// Represents text and presentation style for a status update.
/// </summary>
public record StatusMessage(string Value, StatusType StatusType);