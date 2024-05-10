using System.Diagnostics;
using UtilLux.Core.Services.Hook;

namespace UtilLux.Core.Commands.OpenFolder;

public class OpenFolderCommand : CommandHandler
{
    public OpenFolderCommand(CommandTrigger trigger, IKeyboardHookService keyboardHookService) :
        base(trigger, keyboardHookService)
    {
        Register();
    }

    public async override Task HandleAsync()
    {
        if (this._trigger.Data is OpenFolderData)
        {
            var data = (OpenFolderData)this._trigger.Data!;
            Process.Start("explorer.exe", data.Path);
        }
        else
            return;
    }
}
