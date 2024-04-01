namespace UtilLux.Core.Infrastructure;

public interface ISingleInstanceService
{
    Mutex TryAcquireMutex(string name);
}
