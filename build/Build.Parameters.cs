using Nuke.Common;
using System.Runtime.InteropServices;

namespace UtilLux.Build;

public partial class Build
{
    [Parameter("Configuration - Release by default")]
    public readonly Configuration Configuration = Configuration.Release;

    [Parameter("Target OS - current OS by default")]
    public readonly TargetOS TargetOS =
        RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
            ? TargetOS.MacOS
            : RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                ? TargetOS.Linux
                : TargetOS.Windows;

    [Parameter("Platform - current architecture by default")]
    public readonly Platform Platform =
        RuntimeInformation.ProcessArchitecture == Architecture.Arm64 ? Platform.Arm64 : Platform.X64;

    [Parameter("Publish single file - true by default")]
    public readonly bool PublishSingleFile = true;

    [Parameter("Output file suffix")]
    public readonly string? OutputFileSuffix;

    private string RuntimeIdentifier =>
        $"{this.TargetOS.RuntimeIdentifierPart}-{this.Platform.RuntimeIdentifierPart}";

    private bool IsSelfContained =>
        this.PublishSingleFile || this.Configuration == Configuration.Release;
}
