using System.Collections.Immutable;
using System.Runtime.Serialization;
using UtilLux.Core.Commands;
using UtilLux.Core.Settings.OpenFolder;
using UtilLux.Core.Settings.Sleep;

namespace UtilLux.Core.Settings;

public sealed record AppSettings
{
    public required SleepSettings SleepSettings { get; init; }

    public required OpenFolderSettings OpenFolderSettings { get; init; }

    public required Version AppVersion { get; init; } = null!;
}
