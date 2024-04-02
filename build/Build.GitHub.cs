using Nuke.Common.CI.GitHubActions;
using UtilLux.Build.GitHub;

namespace UtilLux.Build;

[MultipleGitHubActions(
    "build",
    "Build UtilLux",
    typeof(Build),
    OnPushBranches = ["main"],
    OnWorkflowDispatch = true)]

[GitHubAction(
    "build-msi",
    "Build Windows Installer",
    GitHubActionsImage.WindowsLatest,
    InvokedTargets = [nameof(CreateWindowsInstaller)],
    Parameters = [nameof(NukePlatform), PlatformValue, nameof(OutputFileSuffix), WindowsOutputFileSuffix],
    Matrix = [MatrixPlatform, $"[ {Platform.X64Value}, {Platform.Arm64Value} ]"],
    ArtifactSuffix = PlatformValue,
    TimeoutMinutes = GitHubActionsTimeout)]

public partial class Build
{
    public const string NukePlatform = "Nuke" + nameof(Platform);
    public const string MatrixPlatform = "platform";
    public const string PlatformValue = $"${{{{ matrix.{MatrixPlatform} }}}}";
    public const int GitHubActionsTimeout = 30;
    public const string WindowsOutputFileSuffix = "win";
}
