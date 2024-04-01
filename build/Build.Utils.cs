using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common;
using Serilog;
using System;
using System.Collections.Generic;
using Nuke.Common.ProjectModel;

namespace UtilLux.Build;

public partial class Build
{
    private static T AssertFail<T>(string message)
    {
        Assert.Fail(message);
        return default!;
    }

    private static void DebugOnly(OutputType type, string text) =>
        Log.Debug(text);

    private static void ResolvePlaceholders(AbsolutePath file, string architecture) =>
        file.UpdateText(text => text
            .Replace(VersionPlaceholder, Version)
            .Replace(MajorVersionPlaceholder, MajorVersion)
            .Replace(MinorVersionPlaceholder, MinorVersion)
            .Replace(ReleasePlaceholder, ReleaseNumber)
            .Replace(ArchitecturePlaceholder, architecture)
            .Replace(OutputPlaceholder, PublishOutputDirectory));

    private AbsolutePath WithSuffix(AbsolutePath path) =>
        !String.IsNullOrEmpty(this.OutputFileSuffix)
            ? path.Parent / (path.NameWithoutExtension + "-" + this.OutputFileSuffix + path.Extension)
            : path;

    private IEnumerable<Project> GetProjects(bool includeTests = false, bool includeInstaller = false)
    {
        yield return this.Solution.UtilLux_Core;

        yield return this.PlatformDependent(
            windows: this.Solution.UtilLux_Windows,
            macos: this.Solution.UtilLux_Windows,
            linux: this.Solution.UtilLux_Windows);

        yield return this.Solution.UtilLux;

        yield return this.Solution.UtilLux_App_Core;
        yield return this.Solution.UtilLux_App;

        if (includeTests)
        {
            // yield return this.Solution.UtilLux_Tests;
        }

        if (includeInstaller)
        {
            yield return this.Solution.UtilLux_Windows_Installer;
        }
    }

    private T PlatformDependent<T>(T windows, T macos, T linux) =>
        this.TargetOS switch
        {
            var os when os == TargetOS.Windows => windows,
            var os when os == TargetOS.MacOS => macos,
            var os when os == TargetOS.Linux => linux,
            _ => AssertFail<T>($"Unsupported target OS: {this.TargetOS}")
        };
}
