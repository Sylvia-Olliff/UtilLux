using System.Text.Json.Serialization;

namespace UtilLux.App.State;

[JsonSerializable(typeof(AppState))]
[JsonSourceGenerationOptions(WriteIndented = true)]
internal partial class AppStateContext : JsonSerializerContext;
