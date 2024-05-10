using UtilLux.Core.Settings;
using UtilLux.Core.Settings.OpenFolder;
using UtilLux.Core.Settings.Sleep;

namespace UtilLux.App.Core.Models;

public sealed class PreferenceModel(AppSettings appSettings, bool startup)
{
    public SleepSettings SleepSettings { get; set; } = appSettings.SleepSettings;

    public OpenFolderSettings OpenFolderSettings { get; set; } = appSettings.OpenFolderSettings;

    public bool Startup { get; set; } = startup;
}
