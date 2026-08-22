using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AvaloniaAppUsingHost.Infrastructure.LongRunning;

/// <summary>
/// Communicates a status update through the messenger.
/// </summary>
public class StatusValueDataMessage(StatusMessage value) : ValueChangedMessage<StatusMessage>(value);