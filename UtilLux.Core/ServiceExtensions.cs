using Microsoft.Extensions.DependencyInjection;
using SharpHook;
using SharpHook.Reactive;
using System.IO.Abstractions;
using UtilLux.Core.Infrastructure;
using UtilLux.Core.Services.Hook;
using UtilLux.Core.Services.Power;
using UtilLux.Core.Services.Settings;

namespace UtilLux.Core;

public static class ServiceExtensions
{
    public static IServiceCollection AddCoreUtilLuxServices(this IServiceCollection services) =>
        services
            .AddSingleton<IReactiveGlobalHook>(sp => new SimpleReactiveGlobalHook(GlobalHookType.Keyboard))
            .AddSingleton<IKeyboardHookService, SharpHookService>()
            .AddSingleton<IAppSettingsService, JsonSettingsService>()
            .AddSingleton<INamedPipeService, NamedPipeService>()
            .AddSingleton<ISingleInstanceService, SingleInstanceService>()
            .AddSingleton<IFileSystem, FileSystem>();
            
}
