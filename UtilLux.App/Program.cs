using Avalonia;
using Avalonia.ReactiveUI;

namespace UtilLux.App;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Directory.SetCurrentDirectory(
            Path.GetDirectoryName(AppContext.BaseDirectory) ?? String.Empty);

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace(Avalonia.Logging.LogEventLevel.Debug)
            .UseReactiveUI();
}
