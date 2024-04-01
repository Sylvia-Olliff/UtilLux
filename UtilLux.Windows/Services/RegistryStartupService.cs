using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using UtilLux.Core.Services.Startup;
using UtilLux.Core.Settings;

namespace UtilLux.Windows.Services;

internal class RegistryStartupService(
    IOptions<GlobalSettings> globalSettings, 
    ILogger<RegistryStartupService> logger) : IStartupService
{
    private const string StartupKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string StartupKeyName = "UtilLux";
    private const string ExecutableExtension = ".exe";


    public void ConfigureStartup(bool startup)
    {
        logger.LogDebug(
            "Configuring to {Action} running the UtilLux service on startup", startup ? "start" : "stop");

        using var key = Registry.CurrentUser.OpenSubKey(StartupKeyPath, true);

        if (startup)
        {
            key?.SetValue(StartupKeyName, GetServicePath());
        }
        else
        {
            key?.DeleteValue(StartupKeyName, false);
        }

        logger.LogDebug("Configuration to {Action} running the UtilLux service on startup completed", startup ? "start" : "stop");
    }

    public bool IsStartupConfigured()
    {
        logger.LogDebug("Checking if UtilLux is configured to start on boot");

        using var key = Registry.CurrentUser.OpenSubKey(StartupKeyPath);
        bool isConfigured = key?.GetValue(StartupKeyName) is not null;

        logger.LogDebug("UtilLux is {condition} configured to start on boot", isConfigured ? "already" : "not");

        return isConfigured;
    }

    private string GetServicePath()
    {
        var path = globalSettings.Value.ServicePath.EndsWith(
            ExecutableExtension, StringComparison.InvariantCultureIgnoreCase)
            ? globalSettings.Value.ServicePath
            : globalSettings.Value.ServicePath + ExecutableExtension;

        return $"\"{Path.GetFullPath(path)}\"";
    }
}
