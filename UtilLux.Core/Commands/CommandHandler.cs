using UtilLux.Core.Services.Hook;

namespace UtilLux.Core.Commands;

public abstract class CommandHandler(CommandTrigger trigger, IKeyboardHookService keyboardHookService)
{
    private readonly IKeyboardHookService _keyboardHookService = keyboardHookService;
    protected CommandTrigger _trigger = trigger;


    public abstract Task HandleAsync();

    private bool ShouldExecute(CommandTrigger trigger)
    {
        return trigger.CommandId == _trigger.CommandId;
    }

    protected void Register()
    {
        this._trigger.CommandId = Guid.NewGuid();
        this._keyboardHookService.Register(this._trigger);
        this._keyboardHookService.CommandExecuted.Subscribe(async trigger =>
        {
            if (ShouldExecute(trigger))
                await HandleAsync();
        });
    }
}
