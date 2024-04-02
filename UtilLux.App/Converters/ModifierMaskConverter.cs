﻿using ReactiveUI;
using SharpHook.Native;
using System;

namespace UtilLux.App.Converters;

public sealed class ModifierMaskConverter : IBindingTypeConverter
{
    public int GetAffinityForObjects(Type fromType, Type toType) =>
        fromType == typeof(ModifierMask) || toType == typeof(ModifierMask)
            ? 10000
            : 0;

    public bool TryConvert(object? from, Type toType, object? conversionHint, out object? result)
    {
        switch (from)
        {
            case ModifierMask modifier:
                result = Convert.ModifierToString(modifier);
                return true;
            case string str:
                result = Convert.StringToModifier(str);
                return true;
            default:
                result = null;
                return false;
        }
    }
}
