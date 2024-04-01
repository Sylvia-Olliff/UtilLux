namespace UtilLux.Core.Infrastructure;

public interface IServiceCommunicator
{
    bool IsServiceRunning();
    void StartService();
    void StopService(bool kill);
    void ReloadService();
}
