using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AvaloniaAppUsingHost.Infrastructure.LongRunning;

public class StatusDataMessage(string value) : ValueChangedMessage<string>(value);