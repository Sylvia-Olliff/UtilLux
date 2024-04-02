using Avalonia.Controls;
using Avalonia.Logging;
using Avalonia.ReactiveUI;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using SharpHook.Native;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Disposables;
using UtilLux.App.Core.ViewModels;

using Convert = UtilLux.App.Converters.Convert;

namespace UtilLux.App.Views;

public partial class PreferencesView : ReactiveUserControl<PreferencesViewModel>
{
    public PreferencesView()
    {
        this.InitializeComponent();

        var modifiers = this.Modifiers()
            .Where(key => key != ModifierMask.None)
            .Select(Convert.ModifierToString)
            .ToImmutableList();

        var allModifiers = modifiers.Insert(0, Convert.ModifierToString(ModifierMask.None));

        var keyCodes = this.KeyCodes()
            .Select(Convert.KeyCodeToString)
            .ToImmutableList();

        this.SleepMinFirstComboBox.ItemsSource = modifiers;
        this.SleepMinSecondComboBox.ItemsSource = modifiers;
        this.SleepMinThirdComboBox.ItemsSource = allModifiers;

        this.SleepMaxFirstComboBox.ItemsSource = modifiers;
        this.SleepMaxSecondComboBox.ItemsSource = modifiers;
        this.SleepMaxThirdComboBox.ItemsSource = allModifiers;

        this.SleepMinExecutionKeyComboBox.ItemsSource = keyCodes;
        this.SleepMaxExecutionKeyComboBox.ItemsSource = keyCodes;

        this.WhenActivated(disposables =>
        {
            this.BindCheckboxes(disposables);
            this.BindControls(disposables);
            this.BindValidations(disposables);
            this.BindCommands(disposables);
        });
    }

    private void BindCheckboxes(CompositeDisposable disposables)
    {
        this.Bind(this.ViewModel, vm => vm.Startup, v => v.StartupCheckBox.IsChecked)
            .DisposeWith(disposables);
    }

    private void BindControls(CompositeDisposable disposables)
    {
        this.Bind(this.ViewModel, vm => vm.SleepMinModifierFirst, v => v.SleepMinFirstComboBox.SelectedItem)
            .DisposeWith(disposables);

        this.Bind(this.ViewModel, vm => vm.SleepMinModifierSecond, v => v.SleepMinSecondComboBox.SelectedItem)
            .DisposeWith(disposables);

        this.Bind(this.ViewModel, vm => vm.SleepMinModifierThird, v => v.SleepMinThirdComboBox.SelectedItem)
            .DisposeWith(disposables);

        this.Bind(this.ViewModel, vm => vm.SleepMaxModifierFirst, v => v.SleepMaxFirstComboBox.SelectedItem)
            .DisposeWith(disposables);

        this.Bind(this.ViewModel, vm => vm.SleepMaxModifierSecond, v => v.SleepMaxSecondComboBox.SelectedItem)
            .DisposeWith(disposables);

        this.Bind(this.ViewModel, vm => vm.SleepMaxModifierThird, v => v.SleepMaxThirdComboBox.SelectedItem)
            .DisposeWith(disposables);

        this.Bind(this.ViewModel, vm => vm.SleepMinKeyCode, v => v.SleepMinExecutionKeyComboBox.SelectedItem)
            .DisposeWith(disposables);

        this.Bind(this.ViewModel, vm => vm.SleepMaxKeyCode, v => v.SleepMaxExecutionKeyComboBox.SelectedItem)
            .DisposeWith(disposables);

        this.Bind(this.ViewModel, vm => vm.SleepMinValue, v => v.SleepMinValueBox.Value)
            .DisposeWith(disposables);

        this.Bind(this.ViewModel, vm => vm.SleepMaxValue, v => v.SleepMaxValueBox.Value)
            .DisposeWith(disposables);
    }

    private void BindValidations(CompositeDisposable disposables)
    {
        this.BindValidation(
           this.ViewModel, vm => vm!.ModifierKeysAreDifferentRule, v => v.ModifierKeysValidationTextBlock.Text)
           .DisposeWith(disposables);
    }

    private void BindCommands(CompositeDisposable disposables)
    {
        this.BindCommand(this.ViewModel, vm => vm.Save, v => v.SaveButton)
            .DisposeWith(disposables);

        this.BindCommand(this.ViewModel, vm => vm.Cancel, v => v.CancelButton)
            .DisposeWith(disposables);

        this.ViewModel!.Cancel.CanExecute
            .BindTo(this, v => v.ActionPanel.IsVisible)
            .DisposeWith(disposables);
    }

    private List<KeyCode> KeyCodes() =>
    [
        KeyCode.VcA,
        KeyCode.VcB,
        KeyCode.VcC,
        KeyCode.VcD,
        KeyCode.VcE,
        KeyCode.VcF,
        KeyCode.VcG,
        KeyCode.VcH,
        KeyCode.VcI,
        KeyCode.VcJ,
        KeyCode.VcK,
        KeyCode.VcL,
        KeyCode.VcM,
        KeyCode.VcN,
        KeyCode.VcO,
        KeyCode.VcP,
        KeyCode.VcQ,
        KeyCode.VcR,
        KeyCode.VcS,
        KeyCode.VcT,
        KeyCode.VcU,
        KeyCode.VcV,
        KeyCode.VcW,
        KeyCode.VcX,
        KeyCode.VcY,
        KeyCode.VcZ,
        KeyCode.Vc1,
        KeyCode.Vc2,
        KeyCode.Vc3,
        KeyCode.Vc4,
        KeyCode.Vc5,
        KeyCode.Vc6,
        KeyCode.Vc7,
        KeyCode.Vc8,
        KeyCode.Vc9,
        KeyCode.Vc0,
        KeyCode.VcF1,
        KeyCode.VcF2,
        KeyCode.VcF3,
        KeyCode.VcF4,
        KeyCode.VcF5,
        KeyCode.VcF6,
        KeyCode.VcF7,
        KeyCode.VcF8,
        KeyCode.VcF9,
        KeyCode.VcF10,
        KeyCode.VcF11,
        KeyCode.VcF12,
        KeyCode.VcSlash,
        KeyCode.VcBackslash,
        KeyCode.VcComma,
        KeyCode.VcPeriod,
        KeyCode.VcQuote,
        KeyCode.VcSemicolon,
        KeyCode.VcEquals,
        KeyCode.VcMinus,
        KeyCode.VcBackQuote
    ];

    private List<ModifierMask> Modifiers() =>
    [
        ModifierMask.None,
        ModifierMask.Ctrl,
        ModifierMask.Shift,
        ModifierMask.Alt,
        ModifierMask.Meta,
        ModifierMask.LeftCtrl,
        ModifierMask.LeftShift,
        ModifierMask.LeftAlt,
        ModifierMask.LeftMeta,
        ModifierMask.RightCtrl,
        ModifierMask.RightShift,
        ModifierMask.RightAlt,
        ModifierMask.RightMeta
    ];
}
