using System.Reactive;
using UtilLux.Core.Settings;

namespace UtilLux.Core.Services.Settings;

public interface IAppSettingsService
{
    IObservable<Unit> SettingsInvalidated { get; }

    Task<AppSettings> GetAppSettings(bool strict = false);
    Task SaveAppSettings(AppSettings appSettings);

    void InvalidateAppSettings();
}
