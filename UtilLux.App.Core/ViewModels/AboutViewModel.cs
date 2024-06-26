﻿using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System.Reactive;
using System.Reflection;

using static UtilLux.App.Core.Constants;
using static UtilLux.Core.Extensions;

namespace UtilLux.App.Core.ViewModels;

public sealed class AboutViewModel : ReactiveObject
{
    public AboutViewModel()
    {
        this.AppVersion = Assembly.GetExecutingAssembly().GetName().Version!;
        this.CheckForUpdates = ReactiveCommand.CreateFromTask(this.OnCheckForUpdates);
        this.GetNewVersion = ReactiveCommand.Create(this.OnGetNewVersion);
        this.OpenDocs = ReactiveCommand.Create(this.OnOpenDocs);

        this.CheckForUpdates.ToPropertyEx(this, vm => vm.LatestVersion, initialValue: this.AppVersion);
    }

    public Version AppVersion { get; }
    public Version LatestVersion { [ObservableAsProperty] get; } = null!;

    public ReactiveCommand<Unit, Version> CheckForUpdates { get; }
    public ReactiveCommand<Unit, Unit> GetNewVersion { get; }
    public ReactiveCommand<Unit, Unit> OpenDocs { get; }

    public async Task<Version> OnCheckForUpdates()
    {
        try
        {
            using var httpClient = new HttpClient();
            string version = await Task.Run(() => httpClient.GetStringAsync(VersionInfoLocation));
            return Version.Parse(version.Trim());
        }
        catch (Exception e)
        {
            this.Log().Error(e, "Cannot get the latest version info when checking for updates");
            return this.AppVersion;
        }
    }

    public void OnGetNewVersion() =>
        new Uri(AppReleaseLocation).OpenInBrowser();

    public void OnOpenDocs() =>
        new Uri(DocsLocation).OpenInBrowser();
}
