using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace UtilLux.App.State;

public static class Extensions
{
    public static IServiceCollection AddSuspensionDriver(this IServiceCollection services) =>
       services.AddSingleton<ISuspensionDriver, JsonSuspensionDriver>();
}
