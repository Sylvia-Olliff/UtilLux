using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Serilog;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using Splat.Serilog;
using System.Threading;
using System.Threading.Tasks;
using UtilLux.App.Converters;
using UtilLux.App.Core.ViewModels;
using UtilLux.App.Properties;
using UtilLux.App.State;
using UtilLux.Core;
using UtilLux.Core.Exceptions;
using UtilLux.Core.Logging;
using UtilLux.Core.Services.InitialSetup;
using UtilLux.Core.Services.Settings;
using UtilLux.Core.Settings;
using UtilLux.Windows;
using static UtilLux.Core.Util;
using static UtilLux.App.Core.Constants;
using UtilLux.App.Views;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;
using System.Reactive.Linq;
using System;
using System.Reactive.Subjects;
using System.Reactive;
using System.Reflection;
using UtilLux.Core.Infrastructure;

namespace UtilLux.App;

public partial class App : Application, IEnableLogger
{
    private IClassicDesktopStyleApplicationLifetime desktop = null!;
    private Mutex? mutex;
    private ServiceProvider? serviceProvider;

    public override void Initialize() =>
        AvaloniaXamlLoader.Load(this);


    public override async void OnFrameworkInitializationCompleted()
    {
        if (this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            this.Log().Debug("Setting up UtilLux App");
            this.desktop = desktop;

            var mainViewModel = await this.InitializeApp();

            this.Log().Debug("Creating main window");
            this.desktop.MainWindow = await this.CreateMainWindow(mainViewModel);
            this.desktop.MainWindow.Show();
            this.Log().Debug("Main window created and shown");

            this.desktop.Exit += this.OnExit;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private async Task<MainViewModel> InitializeApp()
    {
        TransitioningContentControl.PageTransitionProperty.OverrideDefaultValue(typeof(ViewModelViewHost), null);

        var services = new ServiceCollection();
        this.ConfigureServices(services);

        this.serviceProvider = services.BuildServiceProvider();
        this.serviceProvider.UseMicrosoftDependencyResolver();

        var openExternally = this.ConfigureSingleInstance(serviceProvider);
        this.ConfigureSuspensionDriver();

        this.Log().Info("Starting the settings app");

        this.serviceProvider.GetRequiredService<IInitialSetupService>().InitializeUtiLuxSetup();

        try
        {
            var appSettings = await this.serviceProvider.GetRequiredService<IAppSettingsService>().GetAppSettings();
            this.Log().Debug("App settings loaded");
            var mainViewModel = new MainViewModel(appSettings);
            this.Log().Debug("MainViewModel created, invoking OpenExternally");
            openExternally.InvokeCommand(mainViewModel.OpenExternally);

            return mainViewModel;
        }
        catch (IncompatibleAppVersionException ex)
        {
            this.Log().Fatal(
                ex,
                "Incompatible app version found in settings: {Version}. " +
                "Delete the settings and let the app recreate a compatible version",
                ex.Version);

            this.desktop.Shutdown(2);
            return null!;
        }
        catch (Exception ex)
        {
            this.Log().Fatal(ex, "Error during app initialization");
            this.desktop.Shutdown(1);
            return null!;
        }
    }

    private void ConfigureServices(IServiceCollection services)
    {
        var configDirectory = GetConfigDirectory();
        var environment = PlatformDependent(windows: () => "windows", macos: () => "macos", linux: () => "linux");
        var genericProvider = this.JsonProvider(configDirectory, "appsettings.json");
        var platformSpecificProvider = this.JsonProvider(configDirectory, $"appsettings.{environment}.json");
        var config = new ConfigurationRoot([genericProvider, platformSpecificProvider]);

        var logger = SerilogLoggerFactory.CreateLogger(config);

        services
            .AddOptions()
            .AddLogging(config => config.AddSerilog(logger))
            .Configure<GlobalSettings>(config.GetSection("Settings"))
            .AddSingleton(Messages.ResourceManager)
            .AddSingleton<IActivationForViewFetcher>(new AvaloniaActivationForViewFetcher())
            .AddSuspensionDriver()
            .AddCoreUtilLuxServices()
            .AddNativeUtiLuxServices()
            .UseMicrosoftDependencyResolver();

        Locator.CurrentMutable.UseSerilogFullLogger(logger);
        Locator.CurrentMutable.InitializeSplat();

        Locator.CurrentMutable.InitializeReactiveUI(RegistrationNamespace.Avalonia);
        Locator.CurrentMutable.RegisterConstant(RxApp.TaskpoolScheduler, TaskPoolKey);
        Locator.CurrentMutable.RegisterConstant<IBindingTypeConverter>(new ModifierMaskConverter());
        Locator.CurrentMutable.RegisterConstant<IBindingTypeConverter>(new KeyCodeConverter());

        this.RegisterViews();

        RxApp.MainThreadScheduler = AvaloniaScheduler.Instance;
    }

    private void RegisterViews()
    {
        Locator.CurrentMutable.Register<IViewFor<MainViewModel>>(() => new MainWindow());

        Locator.CurrentMutable.Register<IViewFor<AboutViewModel>>(() => new AboutView());
        Locator.CurrentMutable.Register<IViewFor<MainContentViewModel>>(() => new MainContentView());
        Locator.CurrentMutable.Register<IViewFor<PreferencesViewModel>>(() => new PreferencesView());
        Locator.CurrentMutable.Register<IViewFor<ServiceViewModel>>(() => new ServiceView());
    }

    private void ConfigureSuspensionDriver()
    {
        var autoSuspendHelper = new AutoSuspendHelper(this.desktop);

        RxApp.SuspensionHost.CreateNewAppState = () => new AppState();
        RxApp.SuspensionHost.SetupDefaultSuspendResume();

        autoSuspendHelper.OnFrameworkInitializationCompleted();
    }

    private JsonConfigurationProvider JsonProvider(string directory, string fileName) =>
        new(new JsonConfigurationSource
        {
            Path = fileName,
            FileProvider = new PhysicalFileProvider(directory),
            Optional = true
        });

    private async Task<MainWindow> CreateMainWindow(MainViewModel viewModel)
    {
        var state = await RxApp.SuspensionHost.ObserveAppState<AppState>().Take(1);

        var window = new MainWindow
        {
            ViewModel = viewModel
        };

        if (state.IsInitialized)
        {
            window.Width = state.WindowWidth;
            window.Height = state.WindowHeight;
            window.WindowState = state.IsWindowMaximized ? WindowState.Maximized : WindowState.Normal;
        }

        var windowStateChanged = window
            .GetObservable(Window.WindowStateProperty)
            .DistinctUntilChanged()
            .Discard();

        var windowResized = window
            .GetObservable(TopLevel.ClientSizeProperty)
            .DistinctUntilChanged()
            .Discard();

        var windowPositionChanged = Observable
            .FromEventPattern<PixelPointEventArgs>(h => window.PositionChanged += h, h => window.PositionChanged -= h)
            .Discard();

        Observable.Merge(windowStateChanged, windowResized, windowPositionChanged)
            .Throttle(TimeSpan.FromMilliseconds(500))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(this.SaveAppState);

        return window;
    }

    private void SaveAppState()
    {
        if (this.desktop.MainWindow == null)
        {
            return;
        }

        var state = RxApp.SuspensionHost.GetAppState<AppState>();

        state.IsWindowMaximized = this.desktop.MainWindow.WindowState == WindowState.Maximized;

        if (!state.IsWindowMaximized)
        {
            state.WindowWidth = this.desktop.MainWindow.Width;
            state.WindowHeight = this.desktop.MainWindow.Height;
        }

        state.IsInitialized = true;
    }

    private void OnExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        this.Log().Info("Shutting down the settings app");

        try
        {
            this.serviceProvider?.Dispose();
        }
        finally
        {
            this.mutex?.ReleaseMutex();
            this.mutex?.Dispose();
        }
    }

    private Subject<Unit> ConfigureSingleInstance(IServiceProvider services)
    {
        this.Log().Debug("Configuring single instance");
        string assemblyName = Assembly.GetExecutingAssembly().GetName()?.Name ?? String.Empty;

        this.mutex = services
            .GetRequiredService<ISingleInstanceService>()
            .TryAcquireMutex(assemblyName);

        var namedPipeService = services.GetRequiredService<INamedPipeService>();

        namedPipeService.StartServer(assemblyName);

        var openExternally = new Subject<Unit>();

        namedPipeService.ReceivedString
            .Discard()
            .Subscribe(openExternally);

        this.Log().Debug("Single instance configured");

        return openExternally;
    }
}
