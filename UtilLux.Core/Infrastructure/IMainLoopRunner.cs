namespace UtilLux.Core.Infrastructure;

public interface IMainLoopRunner
{
    public void RunMainLoop(CancellationToken token);
}
