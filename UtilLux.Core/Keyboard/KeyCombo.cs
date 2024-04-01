using SharpHook.Native;

namespace UtilLux.Core.Keyboard;

public record KeyCombo(List<ModifierMask> Modifiers, KeyCode ExecutionKey)
{
    public bool AreKeysPressed(IEnumerable<KeyCode> keys)
    {
        HashSet<ModifierMask> mods = [.. Modifiers];
        var includesExecutionKey = false;
        var keysTracker = keys.ToList();

        foreach (var key in keys)
        {
            ModifierMask? keyAsMod = key.ToModifierMask();

            if (key == ExecutionKey)
            {
                includesExecutionKey = true;
                keysTracker.Remove(key);
            }
            else if (keyAsMod.HasValue && mods.Contains((ModifierMask)keyAsMod))
            {
                mods.Remove((ModifierMask)keyAsMod);
                keysTracker.Remove(key);
            }
        }

        return mods.Count == 0 && includesExecutionKey && keysTracker.Count == 0;
    }
}
