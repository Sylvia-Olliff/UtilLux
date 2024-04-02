using Avalonia.ReactiveUI;
using ReactiveUI;
using System.Reactive.Disposables;
using UtilLux.App.Core.ViewModels;

using static UtilLux.Core.ObservableExtensions;
using static System.ObservableExtensions;

namespace UtilLux.App.Views;

public partial class MainContentView : ReactiveUserControl<MainContentViewModel>
{
    public MainContentView()
    {
        this.InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(this.ViewModel, vm => vm.PreferencesViewModel, v => v.PreferencesTabItem.Content)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.AboutViewModel, v => v.AboutTabItem.Content)
                .DisposeWith(disposables);

            this.ViewModel!.OpenAboutTab
                .Subscribe(_ => this.AboutTabItem.IsSelected = true)
                .DisposeWith(disposables);
        });
    }
}
