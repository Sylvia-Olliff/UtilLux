using UtilLux.Core.Commands;
using UtilLux.Core.Commands.Sleep;
using UtilLux.Core.Exceptions;
using UtilLux.Core;
using UtilLux.Core.Services.Hook;
using UtilLux.Core.Services.Power;
using UtilLux.Core.Services.Settings;
using UtilLux.Core.Settings.Sleep;

namespace UtilLux;

public class Worker(
    IKeyboardHookService keyboardHookService,
    IAppSettingsService settingsService,
    IPowerService powerService,
    IExitService exitService,
    ILogger<Worker> logger
    ) : BackgroundService
{
    private readonly List<CommandHandler> handlers = [];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            logger.LogDebug("Configuring UtilLux service");

            await this.RegisterHotKeysFromSettings();

            settingsService.SettingsInvalidated.SubscribeAsync(this.RefreshHotKeys);

            logger.LogInformation("UtilLux service configured");
            logger.LogInformation("Starting UtilLux service");

            await keyboardHookService.StartHook(stoppingToken);
        }
        catch (SettingsNotFoundException ex)
        {
            logger.LogCritical(ex, "The settings file does not exist - open UtilLux Settings to create it");
            await exitService.Exit(ExitCode.SettingsDoNotExist, stoppingToken);
        }
        catch (IncompatibleAppVersionException ex)
        {
            logger.LogCritical(ex, "The settings file is incompatible with the current version of UtilLux");
            await exitService.Exit(ExitCode.IncompatibleSettingsVersion, stoppingToken);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "An error occurred while running the UtilLux service");
            await exitService.Exit(ExitCode.Error, stoppingToken);
        }
    }

    private async Task RegisterHotKeysFromSettings()
    {
        logger.LogDebug("Registering hotkeys from settings");
        var settings = await settingsService.GetAppSettings(strict: true);
        this.RegisterHotKeys(settings.SleepSettings);
    }

    private void RegisterHotKeys(SleepSettings settings)
    {
        var sleepHandlerMin = new EditScreenSleepCommand(
            new CommandTrigger()
            {
                Trigger = settings.KeyCombo,
                Data = settings.MinData
            }, keyboardHookService, powerService);

        this.handlers.Add(sleepHandlerMin);

        var sleepHandlerMax = new EditScreenSleepCommand(
                       new CommandTrigger()
                       {
                Trigger = settings.MaxKeyCombo,
                Data = settings.MaxData
            }, keyboardHookService, powerService);

        this.handlers.Add(sleepHandlerMax);
    }

    private async Task RefreshHotKeys()
    {
        logger.LogDebug("Refreshing hotkeys");
        this.handlers.Clear();
        keyboardHookService.UnregisterAll();
        await this.RegisterHotKeysFromSettings();
    }
}
