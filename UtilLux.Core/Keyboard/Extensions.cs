using SharpHook.Native;

namespace UtilLux.Core.Keyboard;

public static class Extensions
{
    public static ModifierMask? ToModifierMask(this KeyCode keyCode) =>
        keyCode switch
        {
            KeyCode.VcLeftControl => ModifierMask.LeftCtrl,
            KeyCode.VcRightControl => ModifierMask.RightCtrl,
            KeyCode.VcLeftShift => ModifierMask.LeftShift,
            KeyCode.VcRightShift => ModifierMask.RightShift,
            KeyCode.VcLeftAlt => ModifierMask.LeftAlt,
            KeyCode.VcRightAlt => ModifierMask.RightAlt,
            KeyCode.VcLeftMeta => ModifierMask.LeftMeta,
            KeyCode.VcRightMeta => ModifierMask.RightMeta,
            _ => null
        };

    public static KeyCode? ToKeyCode(this ModifierMask modifierMask) =>
        modifierMask switch
        {
            ModifierMask.LeftCtrl => KeyCode.VcLeftControl,
            ModifierMask.RightCtrl => KeyCode.VcRightControl,
            ModifierMask.LeftShift => KeyCode.VcLeftShift,
            ModifierMask.RightShift => KeyCode.VcRightShift,
            ModifierMask.LeftAlt => KeyCode.VcLeftAlt,
            ModifierMask.RightAlt => KeyCode.VcRightAlt,
            ModifierMask.LeftMeta => KeyCode.VcLeftMeta,
            ModifierMask.RightMeta => KeyCode.VcRightMeta,
            _ => null
        };

    public static bool IsSubsetKeyOf(this ModifierMask subKey, ModifierMask superKey) =>
        superKey.Contains(subKey, ModifierMask.Ctrl) &&
        superKey.Contains(subKey, ModifierMask.Shift) &&
        superKey.Contains(subKey, ModifierMask.Alt) &&
        superKey.Contains(subKey, ModifierMask.Meta);

    private static bool Contains(this ModifierMask superKey, ModifierMask subKey, ModifierMask mask)
    {
        var subMask = subKey & mask;
        var superMask = superKey & mask;
        bool isAbsent = subMask == ModifierMask.None && superMask == ModifierMask.None;
        return isAbsent || subMask != ModifierMask.None && (subMask & superMask) == subMask;
    }
}
