using Microsoft.Extensions.Logging;
using static UtilLux.Core.Util;

namespace UtilLux.Core.Infrastructure;

internal sealed class SingleInstanceService(
    INamedPipeService namedPipeService,
    ILogger<SingleInstanceService> logger) : ISingleInstanceService
{
    public Mutex TryAcquireMutex(string name)
    {
        var mutex = new Mutex(false, $"Global\\{name}", out bool createdNew);

        if (!createdNew)
        {
            SendArguementAndExit(name);
        }

        bool hasHandle = mutex.WaitOne(5000, false);
        if (!hasHandle)
        {
            const string message = "Timeout waiting for exclusive access on the mutex";
            logger.LogError(message);
            throw new TimeoutException(message);
        }

        logger.LogDebug("Acquired the global mutex");

        return mutex;
    }

    private void SendArguementAndExit(string name)
    {
        logger.LogDebug("Sending argument to existing instance");

        try
        {
            string? command = GetCommand();
            namedPipeService.Write(name, command ?? string.Empty);
            logger.LogDebug("Sent the command to the original instance: {Command}", command);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to send the command to the original instance");
        }
        finally
        {
            Environment.Exit(0);
        }
    }

    private string? GetCommand() => 
        Environment.GetCommandLineArgs().Length <= 1
            ? null
            : StripCommandLineArgument(Environment.GetCommandLineArgs()[1]);
}
