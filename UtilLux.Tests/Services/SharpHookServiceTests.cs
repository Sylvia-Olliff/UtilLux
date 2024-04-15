using FsCheck;
using FsCheck.Xunit;
using Microsoft.Extensions.Logging;
using Microsoft.Reactive.Testing;
using SharpHook.Native;
using SharpHook.Reactive;
using SharpHook.Testing;
using System.Diagnostics.CodeAnalysis;
using UtilLux.Core.Commands;
using UtilLux.Core.Keyboard;
using UtilLux.Core.Services.Hook;
using UtilLux.Tests.Data;
using UtilLux.Tests.Logging;
using Xunit.Abstractions;
using Random = System.Random;

namespace UtilLux.Tests.Services;

public sealed class SharpHookServiceTests(ITestOutputHelper output)
{
    private static readonly TimeSpan SmallDelay = TimeSpan.FromMilliseconds(32);

    private readonly ILogger<SharpHookService> logger = XUnitLogger.Create<SharpHookService>(output);

    public static Arbitrary<SingleModifier> ArbitrarySingleModifier =>
        new ArbitrarySingleModifier();

    public static Arbitrary<List<SingleModifier>> ArbitraryMultiModifier =>
        new ArbitraryModifiers();

    public static Arbitrary<List<SingleKey>> ArbitraryKeys =>
        new ArbitraryKeys();

    public static Arbitrary<SingleTrigger> ArbitrarySingleTrigger =>
        new ArbitrarySingleTrigger();

    public static Arbitrary<NonModifierKey> ArbitraryNonModifierKey =>
        new ArbitraryNonModifierKey();

    public static Arbitrary<WaitTime> ArbitraryWaitTime =>
        new ArbitraryWaitTime();

    [Property(
        DisplayName = "Keys pressed once should work as a hotkey",
        Arbitrary = [typeof(SharpHookServiceTests)])]
    public void KeysOnce(List<SingleKey> keys, SingleTrigger trigger)
    {
        // Arrange

        using var hook = new TestGlobalHook();
        var scheduler = new TestScheduler();

        using var service = new SharpHookService(
            new ReactiveGlobalHookAdapter(hook, scheduler), scheduler, this.logger);

        var observer = scheduler.CreateObserver<CommandTrigger>();
        service.CommandExecuted.Subscribe(observer);

        var expectedKeyCode = Random.Shared.GetItems((KeyCode[])keys.Select(m => m.KeyCode), 1)[0];
        var expectedModifiers = trigger.Modifiers.Select(m => m.Mask);

        var expected = new CommandTrigger(
                       new KeyCombo(expectedModifiers.ToList(), expectedKeyCode),
                                  trigger.ExecutionKey);

        // Act
        service.Register(expected);

        _ = service.StartHook(CancellationToken.None);
        this.WaitToStart(hook);

        this.HoldModifierKeys(hook, scheduler, expectedModifiers);
        this.SimulateKeyEvents(hook, scheduler, keys.Select(key => key.KeyCode));

        // Assert

        Assert.Equal(1, observer.Messages.Count);
    }

    private void HoldModifierKeys(
        TestGlobalHook hook,
        TestScheduler scheduler,
        IEnumerable<ModifierMask> modifiers,
        TimeSpan? delay = null)
    {
        var actualDelay = delay ?? SmallDelay;

        foreach (var modifier in modifiers)
        {
            hook.SimulateKeyPress((KeyCode)modifier.ToKeyCode()!);
            scheduler.AdvanceBy(actualDelay.Ticks);
        }
    }

    private void SimulateKeyEvents(
        TestGlobalHook hook,
        TestScheduler scheduler,
        IEnumerable<KeyCode> keyCodes,
        TimeSpan? delay = null)
    {
        var actualDelay = delay ?? SmallDelay;

        foreach (var keyCode in keyCodes)
        {
            hook.SimulateKeyPress(keyCode);
            scheduler.AdvanceBy(actualDelay.Ticks);
        }

        foreach (var keyCode in keyCodes.Reverse())
        {
            hook.SimulateKeyRelease(keyCode);
            scheduler.AdvanceBy(actualDelay.Ticks);
        }
    }

    private void WaitToStart(TestGlobalHook hook)
    {
        while (!hook.IsRunning)
        {
            Thread.Sleep(SmallDelay);
        }
    }
}
