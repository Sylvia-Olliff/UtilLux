using Nuke.Common.IO;

namespace UtilLux.Build;

public partial class Build
{
    private static AbsolutePath AnyMsiFile =>
        ArtifactsDirectory / "*.msi";

    private AbsolutePath SourceMsiFile =>
        this.Solution.UtilLux_Windows_Installer.Path.Parent / "bin" / this.Platform.MSBuild 
        / this.Configuration / "en-Us" / (this.Solution.UtilLux_Windows_Installer.Name + ".msi");

    private AbsolutePath MsiFile =>
        this.WithSuffix(ArtifactsDirectory / $"UtilLux-{Version}-{this.Platform.Msi}.msi");
}
