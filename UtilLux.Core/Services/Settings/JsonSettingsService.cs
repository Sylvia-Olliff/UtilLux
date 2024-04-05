using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reactive.Subjects;
using System.Reactive;
using UtilLux.Core.Settings;
using System.Reactive.Linq;
using System.Text.Json;
using UtilLux.Core.Json;
using UtilLux.Core.Exceptions;
using UtilLux.Core.Settings.Sleep;
using UtilLux.Core.Keyboard;
using SharpHook.Native;
using System.Reflection;
using System.IO.Abstractions;

namespace UtilLux.Core.Services.Settings;

internal sealed class JsonSettingsService(
    IOptions<GlobalSettings> globalSettings,
    IFileSystem fileSystem,
    ILogger<JsonSettingsService> logger) : IAppSettingsService
{
    private readonly IFileInfo file = fileSystem.FileInfo.New(
        Environment.ExpandEnvironmentVariables(globalSettings.Value.SettingsFilePath));

    private readonly Subject<Unit> settingsInvalidated = new();

    private AppSettings? appSettings;

    public IObservable<Unit> SettingsInvalidated =>
        this.settingsInvalidated.AsObservable();

    public async Task<AppSettings> GetAppSettings(bool strict = false)
    {
        if (this.appSettings != null)
            return this.appSettings;

        if (this.file.Exists)
        {
            using var stream = new BufferedStream(this.file.OpenRead());
            this.appSettings = await JsonSerializer.DeserializeAsync(stream,
                CommandSettingsJsonContext.Default.AppSettings);
        }
        else if (strict)
        {
            throw new SettingsNotFoundException("Settings file not found!");
        }
        else
        {
            logger.LogInformation("App settings not found - creating default settings");
            await this.SaveAppSettings(this.CreateDefaultAppSettings());
        }

        if (this.appSettings is null)
        {
            throw new SettingsException("Could not read the app settings");
        }

        var appVersion = this.GetAppVersion();

        if (this.appSettings.AppVersion is null || appSettings.AppVersion > appVersion)
        {
            var version = this.appSettings.AppVersion;
            this.appSettings = null;
            throw new IncompatibleAppVersionException(version);
        }

        if (this.appSettings.AppVersion < appVersion)
        {
            await this.MigrateSettingsToLatestVersion();
        }

        return this.appSettings;
    }

    private async Task MigrateSettingsToLatestVersion()
    {
        if (this.appSettings is null)
        {
            return;
        }

        var defaultSettings = this.CreateDefaultAppSettings();

        logger.LogInformation(
            "Migrating settings from version {SourceVersion} to {TargetVersion}",
            this.appSettings.AppVersion,
            defaultSettings.AppVersion);

        var newSettings = this.appSettings with
        {
            AppVersion = defaultSettings.AppVersion,
            SleepSettings = defaultSettings.SleepSettings,
        };

        await this.SaveAppSettings(newSettings);
    }

    public async Task SaveAppSettings(AppSettings appSettings)
    {
        logger.LogDebug("Saving the app settings");

        this.file.Directory?.Create();

        using var stream = new BufferedStream(this.file.OpenWrite());
        await JsonSerializer.SerializeAsync(stream, appSettings, CommandSettingsJsonContext.Default.AppSettings);

        this.appSettings = appSettings;
    }

    public void InvalidateAppSettings()
    {
        logger.LogDebug("Invalidating the app settings");

        this.appSettings = null;
        this.settingsInvalidated.OnNext(Unit.Default);
    }

    private AppSettings CreateDefaultAppSettings() =>
        new()
        {
            AppVersion = this.GetAppVersion(),
            SleepSettings = new SleepSettings()
            {
                KeyCombo = new KeyCombo(
                    [ModifierMask.LeftCtrl],
                    KeyCode.VcF1),
                MaxKeyCombo = new KeyCombo(
                    [ModifierMask.LeftCtrl, ModifierMask.LeftShift],
                    KeyCode.VcF1),

                MinData = new Commands.Sleep.SleepData(5),
                MaxData = new Commands.Sleep.SleepData(15)
            }
        };

    private Version GetAppVersion() =>
        Assembly.GetExecutingAssembly()?.GetName().Version ?? new Version(0, 0);
}
