using UtilLux.Core.Keyboard;

namespace UtilLux.Core.Commands;

public record struct CommandTrigger(KeyCombo Trigger, object Data)
{
    public Guid CommandId { get; set; } = Guid.NewGuid();
}
