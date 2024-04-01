using SharpHook.Native;
using System.Text.Json.Serialization;
using UtilLux.Core.Settings;
using UtilLux.Core.Settings.Sleep;

namespace UtilLux.Core.Json;

[JsonSerializable(typeof(AppSettings))]
[JsonSerializable(typeof(List<string>))]
[JsonSourceGenerationOptions(WriteIndented = true, Converters = [
    typeof(JsonStringEnumConverter<ModifierMask>), 
    typeof(JsonStringEnumConverter<KeyCode>)
    ])]
internal partial class CommandSettingsJsonContext : JsonSerializerContext;
