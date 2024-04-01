using ReactiveUI;
using Splat;
using System.Reactive;
using UtilLux.App.Core.Models;
using UtilLux.Core;
using UtilLux.Core.Services.Startup;
using UtilLux.Core.Settings;

namespace UtilLux.App.Core.ViewModels;

public class MainViewModel : ReactiveObject
{
    private readonly AppSettings appSettings;

    public MainViewModel(
        AppSettings appSettings,
        IStartupService? startupService = null)
    {
        this.appSettings = appSettings;

        startupService ??= ServiceUtil.GetRequiredService<IStartupService>();

        this.MainContentViewModel = new(new PreferenceModel(this.appSettings, startupService.IsStartupConfigured()));

        this.ServiceViewModel = new ServiceViewModel();

        this.OpenExternally = ReactiveCommand.Create(() => {
            this.Log().Debug("OpenExternally command invoked");
        });
        this.OpenAboutTab = ReactiveCommand.Create(() => { });

        this.MainContentViewModel.SavePreferences
            .Discard()
            .InvokeCommand(this.ServiceViewModel.ReloadSettings);

        this.Log().Debug("MainViewModel created, invoking OpenAboutTab command");

        this.OpenAboutTab.InvokeCommand(this.MainContentViewModel.OpenAboutTab);

        this.Log().Debug("OpenAboutTab command invoked");
    }

    public MainContentViewModel MainContentViewModel { get; }
    public ServiceViewModel ServiceViewModel { get; }

    public ReactiveCommand<Unit, Unit> OpenExternally { get; }
    public ReactiveCommand<Unit, Unit> OpenAboutTab { get; }


}
