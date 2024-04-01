using Microsoft.Extensions.Logging;
using SharpHook.Native;
using SharpHook.Reactive;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using UtilLux.Core.Commands;
using UtilLux.Core.Keyboard;

namespace UtilLux.Core.Services.Hook;

internal sealed class SharpHookService : Core.Disposable, IKeyboardHookService
{
    private static readonly TimeSpan KeyPressWaitThresshold = TimeSpan.FromSeconds(3);

    private DateTimeOffset lastKeyPress = DateTimeOffset.MinValue;

    private readonly IReactiveGlobalHook hook;
    private readonly IScheduler scheduler;
    private readonly ILogger<SharpHookService> logger;

    private readonly Subject<CommandTrigger> rawCommandSubject = new();
    private readonly Subject<CommandTrigger> commandSubject = new();
    private CompositeDisposable commandSubscriptions = [];

    private readonly HashSet<KeyCode> pressedKeys = [];
    private readonly HashSet<KeyCode> releasedKeys = [];
    private readonly HashSet<CommandTrigger> registeredTriggers = [];

    private readonly IDisposable hookSubscription;

    public SharpHookService(
        IReactiveGlobalHook hook,
        IScheduler scheduler,
        ILogger<SharpHookService> logger)
    {
        this.hook = hook;
        this.scheduler = scheduler;
        this.logger = logger;

        this.hookSubscription = this.hook.KeyPressed
            .Merge(this.hook.KeyReleased)
            .Delay(TimeSpan.FromMilliseconds(16), scheduler)
            .Subscribe(args =>
            {
                if (args.RawEvent.Type == EventType.KeyPressed)
                {
                    this.HandleKeyDown(args.Data.KeyCode);
                }
                else
                {
                    this.HandleKeyUp(args.Data.KeyCode);
                }
            });
    }

    public IObservable<CommandTrigger> CommandExecuted =>
        this.commandSubject.AsObservable();

    public void Register(CommandTrigger command) 
    {
        this.ThrowIfDisposed();

        this.registeredTriggers.Add(command);

        var subscription = this.SubscribeCommand(command);

        this.commandSubscriptions.Add(subscription);

        this.logger.LogDebug("Registered Command {Command}", command);
    }

    public void UnregisterAll()
    {
        this.ThrowIfDisposed();

        this.logger.LogDebug("Unregistering all commands");

        this.registeredTriggers.Clear();
        this.commandSubscriptions.Clear();

        this.logger.LogDebug("Unregistered all commands");
    }

    private IDisposable SubscribeCommand(CommandTrigger command) =>
        this.rawCommandSubject
            .Where(rCommand => rCommand.CommandId == command.CommandId)
            .Subscribe(this.commandSubject);

    public async Task StartHook(CancellationToken token)
    {
        this.hook.HookEnabled.Subscribe(e => this.logger.LogInformation("Created a global keyboard hook"));
        token.Register(this.hook.Dispose);
        await this.hook.RunAsync();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.logger.LogDebug("Destroying the global hook");

            this.commandSubject.OnCompleted();

            this.hookSubscription.Dispose();
            this.commandSubscriptions.Dispose();
        }
    }

    private void HandleKeyDown(KeyCode keyCode)
    {
        if (this.scheduler.Now - this.lastKeyPress > KeyPressWaitThresshold)
        {
            this.pressedKeys.Clear();
        }

        this.releasedKeys.Clear();
        this.pressedKeys.Add(keyCode);
        this.lastKeyPress = this.scheduler.Now;
    }

    private void HandleKeyUp(KeyCode keyCode)
    {
        if (!this.pressedKeys.Contains(keyCode))
        {
            return;
        }

        this.pressedKeys.Remove(keyCode);
        this.releasedKeys.Add(keyCode);

        var modifiers = this.releasedKeys
            .Select(key => key.ToModifierMask());

        this.releasedKeys.Clear();

        var triggerSet = this.registeredTriggers.Where(trig => trig.Trigger.ExecutionKey == keyCode).ToList();

        foreach (CommandTrigger trigger in triggerSet)
        {
            if (trigger.Trigger.AreKeysPressed([.. pressedKeys, keyCode]))
                this.commandSubject.OnNext(trigger);
        }
    }
}
