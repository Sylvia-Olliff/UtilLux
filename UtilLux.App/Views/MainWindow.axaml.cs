using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using UtilLux.App.Core.ViewModels;

using static UtilLux.Core.ObservableExtensions;

namespace UtilLux.App.Views;

public partial class MainWindow : ReactiveWindow<MainViewModel>
{
    public MainWindow()
    {
        this.InitializeComponent();

        this.WhenActivated(disposables =>
        {
            this.OneWayBind(this.ViewModel, vm => vm.MainContentViewModel, v => v.MainContent.Content)
                .DisposeWith(disposables);

            this.OneWayBind(this.ViewModel, vm => vm.ServiceViewModel, v => v.ServiceViewContent.Content)
                .DisposeWith(disposables);

            this.ViewModel!.OpenExternally
                .Subscribe(this.BringToForeground)
                .DisposeWith(disposables);

            this.GetObservable(KeyUpEvent)
                .Select(e => e.Key)
                .Where(key => key == Key.F1)
                .Discard()
                .InvokeCommand(this.ViewModel.OpenAboutTab)
                .DisposeWith(disposables);
        });
    }

    private void BringToForeground()
    {
        Console.WriteLine("THIS IS A TEST");
        if (!this.IsVisible)
        {
            this.Show();
        }

        if (this.WindowState == WindowState.Minimized)
        {
            this.WindowState = WindowState.Normal;
        }

        this.Activate();
        this.Topmost = true;
        this.Topmost = false;
        this.Focus();
    }
}
