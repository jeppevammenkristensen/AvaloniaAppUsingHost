using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaAppUsingHost.Infrastructure.LongRunning;

public abstract class BaseProgressReportingTask(IMessenger messenger)
{
    public abstract Task ExecuteTask(CancellationToken? token);


    /// <summary>
    ///     Reports progress to the messenger. The progress should be between 0 and 100 otherwise an
    ///     exception will be thrown
    /// </summary>
    /// <param name="progress"></param>
    /// <param name="message"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    protected void ReportProgress(double progress)
    {
        if (progress is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(progress), "Progress must be between 0 and 100.");
        messenger.Send(new ProgressDataMessage(progress));
    }

    protected void ReportStatus(string message)
    {
        messenger.Send(new StatusDataMessage(message));
    }
}