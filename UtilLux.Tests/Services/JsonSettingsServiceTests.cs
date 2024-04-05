using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using SharpHook.Native;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using System.Text.Json.Nodes;
using UtilLux.Core.Commands.Sleep;
using UtilLux.Core.Exceptions;
using UtilLux.Core.Keyboard;
using UtilLux.Core.Services.Settings;
using UtilLux.Core.Settings;
using UtilLux.Core.Settings.Sleep;
using UtilLux.Tests.Logging;
using Xunit.Abstractions;

namespace UtilLux.Tests.Services;

public sealed class JsonSettingsServiceTests(ITestOutputHelper output)
{
    private const string SettingsFileName = "settings.json";
    private const int DefaultMinSleepTime = 5;
    private const int DefaultMaxSleepTime = 15;

    private static readonly Version Version = Assembly.GetExecutingAssembly()?.GetName()?.Version ?? new Version(0, 0);

    private static readonly string SettingsContent = $$"""
        {
          "SleepSettings": {
            "KeyCombo": {
              "Modifiers": [
                "LeftCtrl"
              ],
              "ExecutionKey": "VcF1"
            },
            "MaxKeyCombo": {
              "Modifiers": [
                "LeftCtrl",
                "LeftShift"
              ],
              "ExecutionKey": "VcF1"
            },
            "MinData": {
              "Minutes": {{DefaultMinSleepTime}}
            },
            "MaxData": {
              "Minutes": {{DefaultMaxSleepTime}}
            }
          },
        "AppVersion": "{{Version}}"
       }
    """;

    private readonly ILogger<JsonSettingsService> logger = XUnitLogger.Create<JsonSettingsService>(output);

    [Fact(DisplayName = "Settings should be read from the settings file")]
    public async Task GetAppSettingsFile()
    {
        // Arrange
        var globalSettings = new GlobalSettings { SettingsFilePath = SettingsFileName };

        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            [SettingsFileName] = new MockFileData(SettingsContent)
        });

        var settingsService = new JsonSettingsService(Options.Create(globalSettings), fileSystem, logger);

        // Act

        var appSettings = await settingsService.GetAppSettings();

        // Assert
        Assert.NotNull(appSettings);
        Assert.NotNull(appSettings.SleepSettings);
        Assert.NotNull(appSettings.SleepSettings.KeyCombo);
        Assert.NotNull(appSettings.SleepSettings.MaxKeyCombo);
        Assert.NotNull(appSettings.SleepSettings.MinData);
        Assert.NotNull(appSettings.SleepSettings.MaxData);

        Assert.Equal(KeyCode.VcF1, appSettings.SleepSettings.KeyCombo.ExecutionKey);
        Assert.Equal(KeyCode.VcF1, appSettings.SleepSettings.MaxKeyCombo.ExecutionKey);

        Assert.IsType<int>(appSettings.SleepSettings.MinData.Minutes);
        Assert.IsType<int>(appSettings.SleepSettings.MaxData.Minutes);
        Assert.Equal(DefaultMinSleepTime, appSettings.SleepSettings.MinData.Minutes);
        Assert.Equal(DefaultMaxSleepTime, appSettings.SleepSettings.MaxData.Minutes);

        Assert.Equal(Version, appSettings.AppVersion);
    }

    [Fact(DisplayName = "An exception should be thrown if the settings file is not found in strict mode")]
    public async Task GetAppSettingsStrict()
    {
        // Arrange
        var globalSettings = new GlobalSettings { SettingsFilePath = SettingsFileName };

        var fileSystem = new MockFileSystem();

        var settingsService = new JsonSettingsService(Options.Create(globalSettings), fileSystem, this.logger);

        // Act + Assert

        await Assert.ThrowsAsync<SettingsNotFoundException>(() => settingsService.GetAppSettings(strict: true));
    }

    [Fact(DisplayName = "An exception should be thrown if the settings version is greater than the version of the app")]
    public async Task GetAppSettingsIncompatibleVersion()
    {
        // Arrange
        var globalSettings = new GlobalSettings { SettingsFilePath = SettingsFileName };

        var greaterVersion = new Version(Version.Major + 1, 0);

        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            [SettingsFileName] = new MockFileData(SettingsContent.Replace(Version.ToString(), greaterVersion.ToString()))
        });

        var settingsService = new JsonSettingsService(Options.Create(globalSettings), fileSystem, this.logger);

        // Act + Assert
        var exception = await Assert.ThrowsAsync<IncompatibleAppVersionException>(
            () => settingsService.GetAppSettings());

        Assert.Equal(greaterVersion, exception.Version);
    }

    [Fact(DisplayName = "An exception should be thrown if the settings Version is null")]
    public async Task GetAppSettingsNullVersion()
    {
        var globalSettings = new GlobalSettings { SettingsFilePath = SettingsFileName };

        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            [SettingsFileName] = new MockFileData(SettingsContent.Replace($"\"{Version}\"", "null"))
        });

        var settingsService = new JsonSettingsService(Options.Create(globalSettings), fileSystem, this.logger);

        // Act + Assert

        var exception = await Assert.ThrowsAsync<IncompatibleAppVersionException>(
            () => settingsService.GetAppSettings());

        Assert.Null(exception.Version);
    }

    [Fact(DisplayName = "Settings should be cached after the first read")]
    public async Task GetAppSettingsCache()
    {
        // Arrange

        var globalSettings = new GlobalSettings { SettingsFilePath = SettingsFileName };

        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            [SettingsFileName] = new MockFileData(SettingsContent)
        });

        var settingsService = new JsonSettingsService(Options.Create(globalSettings), fileSystem, this.logger);

        // Act

        await settingsService.GetAppSettings();
        fileSystem.GetFile(SettingsFileName).TextContents = SettingsContent.Replace("true", "false");

        var actualSettings = await settingsService.GetAppSettings();

        // Assert
        
        Assert.NotNull(actualSettings);
        Assert.NotNull(actualSettings.SleepSettings);
        Assert.NotNull(actualSettings.SleepSettings.KeyCombo);
    }

    [Fact(DisplayName = "Settings should be updated if they have a previous version")]
    public async Task GetAppSettingsMigrateVersion()
    {
        // Arrange

        var globalSettings = new GlobalSettings { SettingsFilePath = SettingsFileName };

        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            [SettingsFileName] = new MockFileData(SettingsContent.Replace(Version.ToString(), new Version(0, 0).ToString()))
        });

        var settingsService = new JsonSettingsService(Options.Create(globalSettings), fileSystem, this.logger);

        // Act

        var settings = await settingsService.GetAppSettings(strict: false);

        // Assert

        Assert.NotNull(settings);
        Assert.NotNull(settings.SleepSettings);
        Assert.NotNull(settings.SleepSettings.KeyCombo);
        Assert.NotNull(settings.SleepSettings.MaxKeyCombo);
        Assert.NotNull(settings.SleepSettings.MinData);
        Assert.NotNull(settings.SleepSettings.MaxData);

        Assert.Equal(
            [ModifierMask.LeftCtrl],
            settings.SleepSettings.KeyCombo.Modifiers);

        Assert.Equal(
            [ModifierMask.LeftCtrl, ModifierMask.LeftShift],
            settings.SleepSettings.MaxKeyCombo.Modifiers);

        Assert.Equal(KeyCode.VcF1, settings.SleepSettings.KeyCombo.ExecutionKey);
        Assert.Equal(KeyCode.VcF1, settings.SleepSettings.MaxKeyCombo.ExecutionKey);

        Assert.IsType<int>(settings.SleepSettings.MinData.Minutes);
        Assert.IsType<int>(settings.SleepSettings.MaxData.Minutes);
        Assert.Equal(DefaultMinSleepTime, settings.SleepSettings.MinData.Minutes);
        Assert.Equal(DefaultMaxSleepTime, settings.SleepSettings.MaxData.Minutes);

        Assert.Equal(Version, settings.AppVersion);
    }

    [Fact(DisplayName = "Invalidating the app settings should remove the cache")]
    public async Task InvalidateAppSettingsCache()
    {
        // Arrange

        var globalSettings = new GlobalSettings { SettingsFilePath = SettingsFileName };

        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
        {
            [SettingsFileName] = new MockFileData(SettingsContent)
        });

        var settingsService = new JsonSettingsService(Options.Create(globalSettings), fileSystem, this.logger);

        // Act

        await settingsService.GetAppSettings();
        fileSystem.GetFile(SettingsFileName).TextContents = SettingsContent.Replace("true", "false");

        settingsService.InvalidateAppSettings();
        var actualSettings = await settingsService.GetAppSettings();

        // Assert

        Assert.Equal(DefaultMaxSleepTime, actualSettings.SleepSettings.MaxData.Minutes);
        Assert.Equal(DefaultMinSleepTime, actualSettings.SleepSettings.MinData.Minutes);
    }

    [Fact(DisplayName = "Saving the app settings should write them")]
    public async Task SaveAppSettings()
    {
        // Arrange

        var globalSettings = new GlobalSettings { SettingsFilePath = SettingsFileName };

        var fileSystem = new MockFileSystem();

        var settingsService = new JsonSettingsService(Options.Create(globalSettings), fileSystem, this.logger);

        var settings = new AppSettings
        {
            AppVersion = Version,
            SleepSettings = new SleepSettings
            {
                KeyCombo = new KeyCombo([ModifierMask.LeftCtrl], KeyCode.VcF1),
                MaxKeyCombo = new KeyCombo([ModifierMask.LeftCtrl, ModifierMask.LeftShift], KeyCode.VcF1),
                MinData = new SleepData(DefaultMinSleepTime),
                MaxData = new SleepData(DefaultMaxSleepTime)
            }
        };

        // Act

        await settingsService.SaveAppSettings(settings);

        // Assert

        var file = fileSystem.GetFile(SettingsFileName);
        Assert.NotNull(file);

        var expectedJson = JsonNode.Parse(SettingsContent);
        var actualJson = JsonNode.Parse(file.TextContents);
        Assert.True(JsonNode.DeepEquals(expectedJson, actualJson));
    }
}
