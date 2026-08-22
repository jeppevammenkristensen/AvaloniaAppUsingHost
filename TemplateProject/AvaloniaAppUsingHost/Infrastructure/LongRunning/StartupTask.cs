using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;

namespace AvaloniaAppUsingHost.Infrastructure.LongRunning;

// NOTE: This is a start up task to demonstrate a long-running operation that reports progress and status.
public class StartupTask(IMessenger messenger) : BaseProgressReportingTask(messenger)
{
    public override async Task ExecuteTask(CancellationToken? token)
    {
        ReportStatus("Starting");

        for (var i = 0; i <= 100; i += 10)
        {
            if (i == 80) ReportStatus("Custom reported status");

            ReportProgress(i);
            await Task.Delay(i, token ?? CancellationToken.None);
        }

        ReportStatus("Started");
    }
}