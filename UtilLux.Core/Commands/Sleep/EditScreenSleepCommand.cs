using UtilLux.Core.Exceptions;
using UtilLux.Core.Services.Hook;
using UtilLux.Core.Services.Power;

namespace UtilLux.Core.Commands.Sleep;

public class EditScreenSleepCommand : CommandHandler
{
    private readonly IPowerService _powerService;

    public EditScreenSleepCommand(CommandTrigger trigger, IKeyboardHookService keyboardHookService, IPowerService powerService) :
        base(trigger, keyboardHookService)
    {
        Register();
        _powerService = powerService;
    }

    public async override Task HandleAsync()
    {
        if (this._trigger.Data is SleepData)
        {
            if (!await this._powerService.SetScreenSleepTime(((SleepData)this._trigger.Data!).Minutes))
                throw new CommandExecutionException(this.GetType().Name);
        }
        else
            return;   
    }
}
