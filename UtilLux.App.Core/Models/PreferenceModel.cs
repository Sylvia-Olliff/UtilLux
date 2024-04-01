using UtilLux.Core.Settings;
using UtilLux.Core.Settings.Sleep;

namespace UtilLux.App.Core.Models;

public sealed class PreferenceModel(AppSettings appSettings, bool startup)
{
    public SleepSettings SleepSettings { get; set; } = appSettings.SleepSettings;

    public bool Startup { get; set; } = startup;
}
