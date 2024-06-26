using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System;
using System.Globalization;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using UtilLux.App.Core.ViewModels;
using UtilLux.App.Properties;

using static UtilLux.Core.ObservableExtensions;

namespace UtilLux.App.Views;

public partial class AboutView : ReactiveUserControl<AboutViewModel>
{
    public AboutView()
    {
        this.InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.VersionTextBlock.Text = String.Format(
                CultureInfo.InvariantCulture,
                Messages.VersionFormat,
                this.FormatVersion(this.ViewModel!.AppVersion));

            this.BindCommand(this.ViewModel, vm => vm.CheckForUpdates, v => v.CheckForUpdatesButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.OpenDocs, v => v.ViewDocsButton)
                .DisposeWith(disposables);

            this.BindCommand(this.ViewModel, vm => vm.GetNewVersion, v => v.GetNewVersionButton)
                .DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel!.LatestVersion)
                .WhereNotNull()
                .Select(version => String.Format(
                    CultureInfo.InvariantCulture, Messages.NewVersionAvailable, this.FormatVersion(version)))
                .BindTo(this, v => v.NewVersionTextBlock.Text)
                .DisposeWith(disposables);

            this.BindElementsVisibility(disposables);
        });
    }

    private void BindElementsVisibility(CompositeDisposable disposables)
    {
        var newVersionAvailable = this.WhenAnyValue(v => v.ViewModel!.LatestVersion)
            .Select(latestVersion => latestVersion > this.ViewModel!.AppVersion);

        newVersionAvailable.Invert()
            .BindTo(this, v => v.CheckForUpdatesButton.IsVisible)
            .DisposeWith(disposables);

        this.ViewModel!.CheckForUpdates
            .Select(latestVersion => latestVersion <= this.ViewModel.AppVersion)
            .BindTo(this, v => v.NoNewVersionsTextBlock.IsVisible)
            .DisposeWith(disposables);

        newVersionAvailable
            .BindTo(this, v => v.NewVersionTextBlock.IsVisible)
            .DisposeWith(disposables);

        newVersionAvailable
            .BindTo(this, v => v.GetNewVersionButton.IsVisible)
            .DisposeWith(disposables);

        newVersionAvailable
            .BindTo(this, v => v.GetNewVersionHintTextBlock.IsVisible)
            .DisposeWith(disposables);
    }

    private string FormatVersion(Version version) =>
        $"{version.Major}.{version.Minor}";
}
