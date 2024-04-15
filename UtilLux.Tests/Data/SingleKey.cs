using FsCheck;
using SharpHook.Native;

namespace UtilLux.Tests.Data;

public sealed record SingleKey(KeyCode KeyCode);

public sealed class ArbitrarySingleKey : Arbitrary<SingleKey>
{
    public override Gen<SingleKey> Generator =>
        Gen.Elements<SingleKey>(
            new(KeyCode.VcA),
            new(KeyCode.VcB),
            new(KeyCode.VcC),
            new(KeyCode.VcD),
            new(KeyCode.VcE),
            new(KeyCode.VcF),
            new(KeyCode.VcG),
            new(KeyCode.VcH),
            new(KeyCode.VcI),
            new(KeyCode.VcJ),
            new(KeyCode.VcK),
            new(KeyCode.VcL),
            new(KeyCode.VcM),
            new(KeyCode.VcN),
            new(KeyCode.VcO),
            new(KeyCode.VcP),
            new(KeyCode.VcQ),
            new(KeyCode.VcR),
            new(KeyCode.VcS),
            new(KeyCode.VcT),
            new(KeyCode.VcU),
            new(KeyCode.VcV),
            new(KeyCode.VcW),
            new(KeyCode.VcX),
            new(KeyCode.VcY),
            new(KeyCode.VcZ),
            new(KeyCode.VcF1),
            new(KeyCode.VcF2),
            new(KeyCode.VcF3),
            new(KeyCode.VcF4),
            new(KeyCode.VcF5),
            new(KeyCode.VcF6),
            new(KeyCode.VcF7),
            new(KeyCode.VcF8),
            new(KeyCode.VcF9),
            new(KeyCode.VcF10),
            new(KeyCode.VcF11),
            new(KeyCode.VcF12));
}

public sealed class ArbitraryKeys : Arbitrary<List<SingleKey>>
{
    public override Gen<List<SingleKey>> Generator =>
        from key1 in Arb.Generate<SingleKey>()
        from key2 in Arb.Generate<SingleKey>()
        from key3 in Arb.Generate<SingleKey>()
        from twoItems in Arb.Generate<bool>()
        where key1 != key2 && key1 != key3 && key2 != key3
        select (twoItems ? new List<SingleKey> { key1, key2 } : [key1, key2, key3]);
}
