using UtilLux.Core.Commands.OpenFolder;
using UtilLux.Core.Keyboard;

namespace UtilLux.Core.Settings.OpenFolder;

public sealed record OpenFolderSettings
{
    public const string NAME = "Open Folder";

    public required Dictionary<KeyCombo, OpenFolderData> KeyCombos { get; init; }
}
