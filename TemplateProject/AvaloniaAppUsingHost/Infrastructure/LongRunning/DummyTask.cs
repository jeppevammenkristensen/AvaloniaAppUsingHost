using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaAppUsingHost.Infrastructure.LongRunning;

// NOTE: This is a dummy task to demonstrate a long-running operation that reports progress and status.
public class DummyTask(IMessenger messenger) : BaseProgressReportingTask(messenger)
{
    public override async Task ExecuteTask(CancellationToken? token)
    {
        ReportStatus("Starting engines... (DummyTask)");

        for (var i = 0; i <= 100; i += 10)
        {
            if (i == 80) ReportStatus("Custom reported status");

            ReportProgress(i);
            await Task.Delay(i, token ?? CancellationToken.None);
        }

        ReportStatus("Engines started");
    }
}