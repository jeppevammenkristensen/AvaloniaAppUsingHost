using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AvaloniaAppUsingHost.Infrastructure.LongRunning;

public class ProgressDataMessage(double value) : ValueChangedMessage<double>(value);