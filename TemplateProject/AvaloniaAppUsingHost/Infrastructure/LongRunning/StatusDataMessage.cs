using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AvaloniaAppUsingHost.Infrastructure.LongRunning;

public enum StatusType { Info, Error }


public record StatusMessage(string Value, StatusType StatusType);

public class StatusValueDataMessage(StatusMessage value) : ValueChangedMessage<StatusMessage>(value);