﻿using Serilog.Events;

namespace UtilLux.Core.Settings;

public sealed class LoggingSettings
{
    public string LogFilePath { get; set; } = String.Empty;
    public LogEventLevel MinimumLevel { get; set; }
    public int MaxFileSize { get; set; }
    public int MaxRetainedFiles { get; set; }
    public string OutputTemplate { get; set; } = String.Empty;
}
