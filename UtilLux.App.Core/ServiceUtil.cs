using Splat;

namespace UtilLux.App.Core;

public static class ServiceUtil
{
    public static T GetRequiredService<T>() => 
        Locator.Current.GetService<T>() ?? throw new InvalidOperationException($"Service of type {typeof(T).FullName} not found.");
}
