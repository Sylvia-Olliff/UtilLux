using FsCheck;

namespace UtilLux.Tests.Data;

public sealed record SingleTrigger(NonModifierKey ExecutionKey, List<SingleModifier> Modifiers);

public sealed class ArbitrarySingleTrigger : Arbitrary<SingleTrigger>
{
    public override Gen<SingleTrigger> Generator =>
        from executionKey in Arb.Generate<NonModifierKey>()
        from modifiers in Arb.Generate<List<SingleModifier>>()
        select new SingleTrigger(executionKey, modifiers);
}
