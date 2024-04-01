using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UtilLux.Core.Services.Startup;
using UtilLux.Core.Services.Users;
using UtilLux.Core.Settings;

namespace UtilLux.Core.Services.InitialSetup;

public class StartupSetupService(
    IStartupService startupService,
    IUserProvider userProvider,
    IOptions<GlobalSettings> globalSettings,
    ILogger<StartupSetupService> logger) : OneTimeInitialSetupService(userProvider, globalSettings, logger)
{
    protected override void DoInitialSetup(string currentUser)
    {
        logger.LogInformation("Setting UtilLux to start on system startup");
        startupService.ConfigureStartup(startup: true);
    }
}
