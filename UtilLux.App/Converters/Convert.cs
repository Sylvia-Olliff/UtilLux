using SharpHook.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using UtilLux.App.Properties;

using static UtilLux.Core.Util;

namespace UtilLux.App.Converters;

public static class Convert
{
    private static readonly IReadOnlyDictionary<ModifierMask, string> ModifiersToStrings =
        new List<ModifierMask>
        {
                ModifierMask.None,
                ModifierMask.LeftShift,
                ModifierMask.LeftCtrl,
                ModifierMask.LeftMeta,
                ModifierMask.LeftAlt,
                ModifierMask.RightShift,
                ModifierMask.RightCtrl,
                ModifierMask.RightMeta,
                ModifierMask.RightAlt,
                ModifierMask.Shift,
                ModifierMask.Ctrl,
                ModifierMask.Meta,
                ModifierMask.Alt,
        }.ToDictionary(modifier => modifier, ModifierMaskToString);

    private static readonly IReadOnlyDictionary<KeyCode, string> KeyCodesToStrings = 
        new List<KeyCode>
        {
            KeyCode.Vc0,
            KeyCode.Vc1,
            KeyCode.Vc2,
            KeyCode.Vc3,
            KeyCode.Vc4,
            KeyCode.Vc5,
            KeyCode.Vc6,
            KeyCode.Vc7,
            KeyCode.Vc8,
            KeyCode.Vc9,
            KeyCode.VcA,
            KeyCode.VcB,
            KeyCode.VcC,
            KeyCode.VcD,
            KeyCode.VcE,
            KeyCode.VcF,
            KeyCode.VcG,
            KeyCode.VcH,
            KeyCode.VcI,
            KeyCode.VcJ,
            KeyCode.VcK,
            KeyCode.VcL,
            KeyCode.VcM,
            KeyCode.VcN,
            KeyCode.VcO,
            KeyCode.VcP,
            KeyCode.VcQ,
            KeyCode.VcR,
            KeyCode.VcS,
            KeyCode.VcT,
            KeyCode.VcU,
            KeyCode.VcV,
            KeyCode.VcW,
            KeyCode.VcX,
            KeyCode.VcY,
            KeyCode.VcZ,
            KeyCode.VcF1,
            KeyCode.VcF2,
            KeyCode.VcF3,
            KeyCode.VcF4,
            KeyCode.VcF5,
            KeyCode.VcF6,
            KeyCode.VcF7,
            KeyCode.VcF8,
            KeyCode.VcF9,
            KeyCode.VcF10,
            KeyCode.VcF11,
            KeyCode.VcF12,
            KeyCode.VcSlash,
            KeyCode.VcBackslash,
            KeyCode.VcComma,
            KeyCode.VcPeriod,
            KeyCode.VcQuote,
            KeyCode.VcSemicolon,
            KeyCode.VcEquals,
            KeyCode.VcMinus,
            KeyCode.VcBackQuote
        }.ToDictionary(keyCode => keyCode, KeyCodeToStringMap);

    private static readonly IReadOnlyDictionary<string, ModifierMask> StringsToModifiers =
        ModifiersToStrings.ToDictionary(e => e.Value, e => e.Key);

    private static readonly IReadOnlyDictionary<string, KeyCode> StringsToKeyCodes =
        KeyCodesToStrings.ToDictionary(e => e.Value, e => e.Key);

    public static string ModifierToString(ModifierMask modifierKey) =>
        ModifiersToStrings[modifierKey];

    public static string KeyCodeToString(KeyCode keyCode) =>
        KeyCodesToStrings[keyCode];

    public static ModifierMask StringToModifier(string str) =>
        StringsToModifiers[str];

    public static KeyCode StringToKeyCode(string str) =>
        StringsToKeyCodes[str];

    private static string KeyCodeToStringMap(KeyCode code) =>
        code switch
        {
            KeyCode.Vc0 => Messages.KeyCode0,
            KeyCode.Vc1 => Messages.KeyCode1,
            KeyCode.Vc2 => Messages.KeyCode2,
            KeyCode.Vc3 => Messages.KeyCode3,
            KeyCode.Vc4 => Messages.KeyCode4,
            KeyCode.Vc5 => Messages.KeyCode5,
            KeyCode.Vc6 => Messages.KeyCode6,
            KeyCode.Vc7 => Messages.KeyCode7,
            KeyCode.Vc8 => Messages.KeyCode8,
            KeyCode.Vc9 => Messages.KeyCode9,
            KeyCode.VcA => Messages.KeyCodeA,
            KeyCode.VcB => Messages.KeyCodeB,
            KeyCode.VcC => Messages.KeyCodeC,
            KeyCode.VcD => Messages.KeyCodeD,
            KeyCode.VcE => Messages.KeyCodeE,
            KeyCode.VcF => Messages.KeyCodeF,
            KeyCode.VcG => Messages.KeyCodeG,
            KeyCode.VcH => Messages.KeyCodeH,
            KeyCode.VcI => Messages.KeyCodeI,
            KeyCode.VcJ => Messages.KeyCodeJ,
            KeyCode.VcK => Messages.KeyCodeK,
            KeyCode.VcL => Messages.KeyCodeL,
            KeyCode.VcM => Messages.KeyCodeM,
            KeyCode.VcN => Messages.KeyCodeN,
            KeyCode.VcO => Messages.KeyCodeO,
            KeyCode.VcP => Messages.KeyCodeP,
            KeyCode.VcQ => Messages.KeyCodeQ,
            KeyCode.VcR => Messages.KeyCodeR,
            KeyCode.VcS => Messages.KeyCodeS,
            KeyCode.VcT => Messages.KeyCodeT,
            KeyCode.VcU => Messages.KeyCodeU,
            KeyCode.VcV => Messages.KeyCodeV,
            KeyCode.VcW => Messages.KeyCodeW,
            KeyCode.VcX => Messages.KeyCodeX,
            KeyCode.VcY => Messages.KeyCodeY,
            KeyCode.VcZ => Messages.KeyCodeZ,
            KeyCode.VcF1 => Messages.KeyCodeF1,
            KeyCode.VcF2 => Messages.KeyCodeF2,
            KeyCode.VcF3 => Messages.KeyCodeF3,
            KeyCode.VcF4 => Messages.KeyCodeF4,
            KeyCode.VcF5 => Messages.KeyCodeF5,
            KeyCode.VcF6 => Messages.KeyCodeF6,
            KeyCode.VcF7 => Messages.KeyCodeF7,
            KeyCode.VcF8 => Messages.KeyCodeF8,
            KeyCode.VcF9 => Messages.KeyCodeF9,
            KeyCode.VcF10 => Messages.KeyCodeF10,
            KeyCode.VcF11 => Messages.KeyCodeF11,
            KeyCode.VcF12 => Messages.KeyCodeF12,
            KeyCode.VcSlash => Messages.KeyCodeSlash,
            KeyCode.VcBackslash => Messages.KeyCodeBackslash,
            KeyCode.VcComma => Messages.KeyCodeComma,
            KeyCode.VcPeriod => Messages.KeyCodePeriod,
            KeyCode.VcQuote => Messages.KeyCodeQuote,
            KeyCode.VcSemicolon => Messages.KeyCodeSemicolon,
            KeyCode.VcEquals => Messages.KeyCodeEquals,
            KeyCode.VcMinus => Messages.KeyCodeMinus,
            KeyCode.VcBackQuote => Messages.KeyCodeBackQuote,
            _ => String.Empty
        };

    private static string ModifierMaskToString(ModifierMask modifier) =>
        modifier switch
        {
            ModifierMask.None => Messages.ModifierKeyNone,

            ModifierMask.LeftCtrl => Messages.ModifierKeyLeftCtrl,
            ModifierMask.RightCtrl => Messages.ModifierKeyRightCtrl,
            ModifierMask.Ctrl => Messages.ModifierKeyCtrl,

            ModifierMask.LeftShift => Messages.ModifierKeyLeftShift,
            ModifierMask.RightShift => Messages.ModifierKeyRightShift,
            ModifierMask.Shift => Messages.ModifierKeyShift,

            ModifierMask.LeftAlt => PlatformDependent(
                windows: () => Messages.ModifierKeyLeftAlt,
                macos: () => Messages.ModifierKeyLeftOption,
                linux: () => Messages.ModifierKeyLeftAlt),
            ModifierMask.RightAlt => PlatformDependent(
                windows: () => Messages.ModifierKeyRightAlt,
                macos: () => Messages.ModifierKeyRightOption,
                linux: () => Messages.ModifierKeyRightAlt),
            ModifierMask.Alt => PlatformDependent(
                windows: () => Messages.ModifierKeyAlt,
                macos: () => Messages.ModifierKeyOption,
                linux: () => Messages.ModifierKeyAlt),

            ModifierMask.LeftMeta => PlatformDependent(
                windows: () => Messages.ModifierKeyLeftWin,
                macos: () => Messages.ModifierKeyLeftCommand,
                linux: () => Messages.ModifierKeyLeftSuper),
            ModifierMask.RightMeta => PlatformDependent(
                windows: () => Messages.ModifierKeyRightWin,
                macos: () => Messages.ModifierKeyRightCommand,
                linux: () => Messages.ModifierKeyRightSuper),
            ModifierMask.Meta => PlatformDependent(
                windows: () => Messages.ModifierKeyWin,
                macos: () => Messages.ModifierKeyCommand,
                linux: () => Messages.ModifierKeySuper),

            _ => String.Empty
        };
}
