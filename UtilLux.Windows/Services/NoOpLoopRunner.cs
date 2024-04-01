using UtilLux.Core.Infrastructure;

namespace UtilLux.Windows.Services;

internal sealed class NoOpLoopRunner : IMainLoopRunner
{
    public void RunMainLoop(CancellationToken token)
    {
        // Do nothing
    }
}
