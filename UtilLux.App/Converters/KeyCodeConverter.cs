using ReactiveUI;
using SharpHook.Native;

namespace UtilLux.App.Converters;

public sealed class KeyCodeConverter : IBindingTypeConverter
{
    public int GetAffinityForObjects(Type fromType, Type toType) =>
        fromType == typeof(KeyCode) || toType == typeof(KeyCode)
            ? 10000
            : 0;

    public bool TryConvert(object? from, Type toType, object? conversionHint, out object? result)
    {
        switch (from)
        {
            case KeyCode keyCode:
                result = Convert.KeyCodeToString(keyCode);
                return true;
            case string str:
                result = Convert.StringToKeyCode(str);
                return true;
            default:
                result = null;
                return false;
        }
    }
}
