﻿namespace UtilLux.Core.Settings;

public sealed class GlobalSettings
{
    public string SettingsFilePath { get; set; } = String.Empty;
    public string StateFilePath { get; set; } = String.Empty;
    public string ServicePath { get; set; } = String.Empty;
    public string InitialSetupFilePath { get; set; } = String.Empty;
}
