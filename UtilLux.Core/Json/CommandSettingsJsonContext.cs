using SharpHook.Native;
using System.Text.Json.Serialization;
using UtilLux.Core.Commands.OpenFolder;
using UtilLux.Core.Keyboard;
using UtilLux.Core.Settings;

namespace UtilLux.Core.Json;

[JsonSerializable(typeof(AppSettings))]
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(Dictionary<KeyCombo, OpenFolderData>))]
[JsonSourceGenerationOptions(WriteIndented = true, Converters = [
    typeof(JsonStringEnumConverter<ModifierMask>), 
    typeof(JsonStringEnumConverter<KeyCode>)
    ])]
internal partial class CommandSettingsJsonContext : JsonSerializerContext;
