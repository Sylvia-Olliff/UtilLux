using Microsoft.Extensions.DependencyInjection;
using UtilLux.Core.Infrastructure;
using UtilLux.Core.Services.InitialSetup;
using UtilLux.Core.Services.Power;
using UtilLux.Core.Services.Startup;
using UtilLux.Core.Services.Users;
using UtilLux.Windows.Services;

namespace UtilLux.Windows;

public static class ServiceExtensions
{
    public static IServiceCollection AddNativeUtiLuxServices(this IServiceCollection services) =>
        services
            .AddSingleton<IStartupService, RegistryStartupService>()
            .AddSingleton<IUserProvider, WinUserProvider>()
            .AddSingleton<IServiceCommunicator, DirectServiceCommunicator>()
            .AddSingleton<IInitialSetupService, StartupSetupService>()
            .AddSingleton<IPowerService, PowerService>()
            .AddSingleton<IMainLoopRunner, NoOpLoopRunner>();
}
