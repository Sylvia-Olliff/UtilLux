using SharpHook.Native;
using UtilLux.Core.Commands;
using UtilLux.Core.Keyboard;

namespace UtilLux.Core.Services.Hook;

public interface IKeyboardHookService : IDisposable
{
    IObservable<CommandTrigger> CommandExecuted { get; }

    void Register(CommandTrigger command);

    void UnregisterAll();

    Task StartHook(CancellationToken token);
}
