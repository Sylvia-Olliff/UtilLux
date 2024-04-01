using ReactiveUI;
using System.Reactive;
using UtilLux.App.Core.Models;
using UtilLux.Core.Services.Settings;
using UtilLux.Core.Services.Startup;

using static UtilLux.App.Core.ServiceUtil;

namespace UtilLux.App.Core.ViewModels;

public sealed class MainContentViewModel : ReactiveObject
{
    private readonly IAppSettingsService appSettingsService;
    private readonly IStartupService startupService;

    public MainContentViewModel(
        PreferenceModel preferenceModel,
        IAppSettingsService? appSettingsService = null,
        IStartupService? startupService = null)
    {
        this.appSettingsService = appSettingsService ?? GetRequiredService<IAppSettingsService>();
        this.startupService = startupService ?? GetRequiredService<IStartupService>();

        this.PreferencesViewModel = new(preferenceModel);
        this.AboutViewModel = new();

        this.SavePreferences = ReactiveCommand.CreateFromTask<PreferenceModel>(
            this.SavePreferencesAsync);
        this.OpenAboutTab = ReactiveCommand.Create(() => { });

        this.PreferencesViewModel.Save.InvokeCommand(this.SavePreferences);
    }

    public PreferencesViewModel PreferencesViewModel { get; }
    public AboutViewModel AboutViewModel { get; }

    public ReactiveCommand<PreferenceModel, Unit> SavePreferences { get; }
    public ReactiveCommand<Unit, Unit> OpenAboutTab { get; }

    private async Task SavePreferencesAsync(PreferenceModel preferenceModel)
    {
        var settings = await this.appSettingsService.GetAppSettings();

        var newSettings = settings with
        {
            SleepSettings = preferenceModel.SleepSettings
        };

        await this.appSettingsService.SaveAppSettings(newSettings);

        if (this.startupService.IsStartupConfigured() != preferenceModel.Startup)
        {
            this.startupService.ConfigureStartup(preferenceModel.Startup);
        }
    }
}
