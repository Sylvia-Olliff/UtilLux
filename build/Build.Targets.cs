using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Serilog;

using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace UtilLux.Build;

public partial class Build
{
    public Target Restore => _ => _
        .Description("Restores the projects")
        .Executes(() =>
        {
            foreach (var project in this.GetProjects(includeTests: true, includeInstaller: true))
            {
                Log.Information("Restoring {Project}", project.Name);
                DotNetRestore(s => s
                    .SetProjectFile(project)
                    .SetRuntime(this.RuntimeIdentifier)
                    .SetPlatform(this.Platform.MSBuild)
                    .SetProperty(nameof(TargetOS), this.TargetOS));
            }
        });

    public Target Clean => t => t
        .Description("Cleans the build outputs")
        .DependsOn(this.Restore)
        .Executes(() =>
        {
            foreach (var project in this.GetProjects(includeTests: true, includeInstaller: true))
            {
                Log.Information("Cleaning project {Name}", project.Name);
                DotNetClean(s => s
                    .SetProject(project)
                    .SetRuntime(this.RuntimeIdentifier)
                    .SetConfiguration(this.Configuration)
                    .SetPlatform(this.Platform.MSBuild)
                    .SetProperty(nameof(TargetOS), this.TargetOS));
            }
        });

    public Target Compile => t => t
        .Description("Builds the project")
        .DependsOn(this.Clean)
        .Executes(() =>
        {
            foreach (var project in this.GetProjects())
            {
                Log.Information("Building project {Name}", project.Name);
                DotNetBuild(s => s
                    .SetProjectFile(project)
                    .SetRuntime(this.RuntimeIdentifier)
                    .SetConfiguration(this.Configuration)
                    .SetPlatform(this.Platform.MSBuild)
                    .SetProperty(nameof(TargetOS), this.TargetOS)
                    .SetNoRestore(true)
                    .SetSelfContained(this.IsSelfContained)
                    .SetPublishSingleFile(this.PublishSingleFile)
                    .SetContinuousIntegrationBuild(true));
            }
        });

    public Target Test => t => t
        .Description("Tests the project")
        .DependsOn(this.Compile)
        .Executes(() =>
        {
            /*Log.Information("Running tests in the project {Name}", this.Solution.UtilLux_Tests.Name);

            DotNetTest(s => s
                .SetProjectFile(this.Solution.UtilLux_Tests)
                .SetConfiguration(this.Configuration)
                .SetNoRestore(true));*/
        });

    public Target Publish => t => t
        .Description("Publishes the project")
        .DependsOn(this.Test)
        .Requires(() => this.Configuration == Configuration.Release)
        .Executes(() =>
        {
            Log.Information("Cleaning the publish directory");
            PublishOutputDirectory.CreateOrCleanDirectory();

            Log.Information("Publishing projects");

            var projects =
                from project in new[] { this.Solution.UtilLux, this.Solution.UtilLux_App }
                select new { project };

            DotNetPublish(s => s
                .SetRuntime(this.RuntimeIdentifier)
                .SetConfiguration(this.Configuration)
                .SetPlatform(this.Platform.MSBuild)
                .SetProperty(nameof(TargetOS), this.TargetOS)
                .SetNoBuild(true)
                .SetNoRestore(true)
                .SetOutput(PublishOutputDirectory)
                .SetSelfContained(this.IsSelfContained)
                .SetPublishSingleFile(this.PublishSingleFile)
                .CombineWith(projects, (s, c) => s.SetProject(c.project)));

            Log.Information("Deleting unneeded files after publish");
            var appSettingsFile = this.PlatformDependent(
                windows: AppSettingsWindows,
                macos: AppSettingsMacOS,
                linux: AppSettingsLinux);

            appSettingsFile.Rename(AppSettings);

            AppSettingsWindows.DeleteFile();
        });

    /*public Target PreCreateArchive => t => t
        .Description("Copies additional files to the publish directory")
        .DependentFor(this.CreateZipArchive, this.CreateTarArchive)
        .OnlyWhenStatic(() => this.TargetOS == TargetOS.Linux)
        .After(this.Publish)
        .Unlisted()
        .Executes(() =>
        {
            Log.Information("Copying additional files to the publish directory");

            CopyFileToDirectory(this.SourceLinuxInstallFile, PublishOutputDirectory);
            CopyFileToDirectory(this.SourceLinuxUninstallFile, PublishOutputDirectory);
        });

    public Target CreateZipArchive => t => t
        .Description("Creates a zip archive containing the published project")
        .DependsOn(this.Publish)
        .Produces(AnyZipFile)
        .Executes(() =>
        {
            Log.Information("Archiving the publish output into {ZipFile}", this.ZipFile);
            this.ZipFile.DeleteFile();
            PublishOutputDirectory.ZipTo(this.ZipFile);
        });

    public Target CreateTarArchive => t => t
        .Description("Creates a tar archive containing the published project")
        .DependsOn(this.Publish)
        .Produces(AnyTarFile)
        .Executes(() =>
        {
            Log.Information("Archiving the publish output into {TarFile}", this.ZipFile);
            this.TarFile.DeleteFile();
            PublishOutputDirectory.TarGZipTo(this.TarFile);
        });*/

    public Target CreateWindowsInstaller => t => t
        .Description("Creates a Windows installer which installs the published project")
        .DependsOn(this.Publish)
        .Requires(() => this.TargetOS == TargetOS.Windows, () => this.PublishSingleFile)
        .Produces(AnyMsiFile)
        .Executes(() =>
        {
            Log.Information("Creating a Windows installer");

            DotNetBuild(s => s
                .SetProjectFile(this.Solution.UtilLux_Windows_Installer)
                .SetRuntime(this.RuntimeIdentifier)
                .SetConfiguration(this.Configuration)
                .SetPlatform(this.Platform.MSBuild)
                .SetProperty(nameof(TargetOS), this.TargetOS)
                .SetNoRestore(true)
                .SetSelfContained(this.IsSelfContained)
                .SetPublishSingleFile(this.PublishSingleFile)
                .SetContinuousIntegrationBuild(true));

            this.MsiFile.DeleteFile();
            this.SourceMsiFile.Move(this.MsiFile);
        });

    public Target CleanUp => t => t
        .Description("Deletes leftover files")
        .TriggeredBy(
            this.CreateWindowsInstaller)
        .Unlisted()
        .Executes(() =>
        {
            Log.Information("Deleting leftover files");

            PublishOutputDirectory.DeleteDirectory();

            /*UtilLuxPkgFile.DeleteFile();
            UtilLuxSettingsPkgFile.DeleteFile();
            UtilLuxUninstallerPkgFile.DeleteFile();
            PkgResourcesDirectory.DeleteDirectory();
            PkgScriptsDirectory.DeleteDirectory();
            UtilLuxAppDirectory.DeleteDirectory();
            UtilLuxSettingsAppDirectory.DeleteDirectory();
            TargetPkgDistributionFile.DeleteFile();
            TargetUninstallerPkgDistributionFile.DeleteFile();
            TargetPkgEntitlementsFile.DeleteFile();*/

        });
}
