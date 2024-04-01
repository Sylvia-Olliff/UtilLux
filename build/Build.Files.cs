using Nuke.Common.IO;
using static Nuke.Common.NukeBuild;

namespace UtilLux.Build;

public partial class Build
{
    private const string UtilLux = nameof(UtilLux);
    private const string UtilLuxLower = "utillux";
    private const string UtilLuxApp = nameof(UtilLuxApp);

    private const string AppSettings = "appsettings.json";
    private const string LinuxIconFile = $"{UtilLuxLower}.png";

    private static readonly AbsolutePath ArtifactsDirectory = RootDirectory / "artifacts";
    private static readonly AbsolutePath PublishOutputDirectory = ArtifactsDirectory / "publish";

    private static AbsolutePath AppSettingsWindows =>
       PublishOutputDirectory / "appsettings.windows.json";

    private static AbsolutePath AppSettingsMacOS =>
        PublishOutputDirectory / "appsettings.macos.json";

    private static AbsolutePath AppSettingsLinux =>
        PublishOutputDirectory / "appsettings.linux.json";

    private AbsolutePath BuildDirectory =>
        this.Solution.UtilLux_Build.Directory;
}
