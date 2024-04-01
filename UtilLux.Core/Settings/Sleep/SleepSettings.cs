using SharpHook.Native;
using UtilLux.Core.Commands.Sleep;
using UtilLux.Core.Keyboard;

namespace UtilLux.Core.Settings.Sleep;

public sealed record SleepSettings
{
    public const string NAME = "Sleep";

    public required KeyCombo KeyCombo { get; init; }

    public required KeyCombo MaxKeyCombo { get; init; }

    public required SleepData MinData { get; init; }

    public required SleepData MaxData { get; init; }

}
