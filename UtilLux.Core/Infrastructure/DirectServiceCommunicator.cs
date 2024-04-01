using Microsoft.Extensions.Options;
using System.Diagnostics;
using UtilLux.Core.Settings;

namespace UtilLux.Core.Infrastructure;

public class DirectServiceCommunicator(
    INamedPipeService namedPipeService,
    IOptions<GlobalSettings> globalSettings) : IServiceCommunicator
{
    public virtual bool IsServiceRunning() => 
        Process.GetProcessesByName(nameof(UtilLux)).Length > 0;

    public virtual void StartService() =>
        Process.Start(globalSettings.Value.ServicePath);

    public virtual void StopService(bool kill)
    {
        if (kill)
        {
            Process.GetProcessesByName(nameof(UtilLux)).ToList().ForEach(p => p.Kill());
        }
        else
        {
            namedPipeService.Write(nameof(UtilLux), ExternalCommand.Stop);
        }
    }

    public virtual void ReloadService() =>
        namedPipeService.Write(nameof(UtilLux), ExternalCommand.ReloadSettings);
}
