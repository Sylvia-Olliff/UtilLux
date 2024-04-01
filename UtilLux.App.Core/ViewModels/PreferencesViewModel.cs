using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Helpers;
using SharpHook.Native;
using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Resources;
using UtilLux.App.Core.Models;
using UtilLux.Core.Commands.Sleep;
using UtilLux.Core.Keyboard;

namespace UtilLux.App.Core.ViewModels;

public sealed class PreferencesViewModel : ReactiveForm<PreferenceModel, PreferencesViewModel>
{
    private readonly SourceList<ModifierMask> sleepMinKeysSource = new();
    private readonly SourceList<ModifierMask> sleepMaxKeysSource = new();

    private readonly ReadOnlyObservableCollection<ModifierMask> sleepMinKeys;
    private readonly ReadOnlyObservableCollection<ModifierMask> sleepMaxKeys;

    public PreferencesViewModel(
        PreferenceModel preferenceModel,
        ResourceManager? resourceManager = null,
        IScheduler? scheduler = null) : base(resourceManager, scheduler)
    {
        this.PreferenceModel = preferenceModel;
        this.CopyProperties();

        this.sleepMinKeysSource.Connect()
            .Bind(out this.sleepMinKeys)
            .Subscribe();

        this.sleepMaxKeysSource.Connect()
            .Bind(out this.sleepMaxKeys)
            .Subscribe();

        this.BindKeys();



        this.ModifierKeysAreDifferentRule = this.InitModifierKeysAreDifferentRule();

        this.EnableChangeTracking();
    }

    public PreferenceModel PreferenceModel { get; }

    [Reactive]
    public bool Startup { get; set; }

    [Reactive]
    public bool ShowConverter { get; set; }

    [Reactive]
    public ModifierMask SleepMinModifierFirst { get; set; }

    [Reactive]
    public ModifierMask SleepMinModifierSecond { get; set; }

    [Reactive]
    public ModifierMask SleepMinModifierThird { get; set; }

    [Reactive]
    public KeyCode SleepMinKeyCode { get; set; }

    [Reactive]
    public KeyCode SleepMaxKeyCode { get; set; }

    [Reactive]
    public ModifierMask SleepMaxModifierFirst { get; set; }

    [Reactive]
    public ModifierMask SleepMaxModifierSecond { get; set; }

    [Reactive]
    public ModifierMask SleepMaxModifierThird { get; set; }

    [Reactive]
    public int SleepMinValue { get; set; }

    [Reactive]
    public int SleepMaxValue { get; set; }

    public ValidationHelper ModifierKeysAreDifferentRule { get; }

    protected override PreferencesViewModel Self => this;

    protected override void EnableChangeTracking()
    {
        this.TrackChanges(vm => vm.Startup, vm => vm.PreferenceModel.Startup);

        this.TrackChanges(this.IsCollectionChangedSimple(
            vm => vm.sleepMinKeys, vm => (ICollection<ModifierMask>)vm.PreferenceModel.SleepSettings.KeyCombo.Modifiers));

        this.TrackChanges(this.IsCollectionChangedSimple(
            vm => vm.sleepMaxKeys, vm => (ICollection<ModifierMask>)vm.PreferenceModel.SleepSettings.MaxKeyCombo.Modifiers));

        this.TrackChanges(vm => vm.SleepMinValue, vm => vm.PreferenceModel.SleepSettings.MinData.Minutes);
        this.TrackChanges(vm => vm.SleepMaxValue, vm => vm.PreferenceModel.SleepSettings.MaxData.Minutes);
        this.TrackChanges(vm => vm.SleepMinKeyCode, vm => vm.PreferenceModel.SleepSettings.KeyCombo.ExecutionKey);
        this.TrackChanges(vm => vm.SleepMaxKeyCode, vm => vm.PreferenceModel.SleepSettings.MaxKeyCombo.ExecutionKey);

        base.EnableChangeTracking();
    }

    protected override Task<PreferenceModel> OnSaveAsync()
    {
        this.PreferenceModel.Startup = this.Startup;
        this.PreferenceModel.SleepSettings = this.PreferenceModel.SleepSettings with
        {
            KeyCombo = new KeyCombo([.. this.sleepMinKeys], this.SleepMinKeyCode),
            MinData = new SleepData(this.SleepMinValue),
            MaxKeyCombo = new KeyCombo([.. this.sleepMaxKeys], this.SleepMaxKeyCode),
            MaxData = new SleepData(this.SleepMaxValue)
        };

        return Task.FromResult(this.PreferenceModel);
    }

    protected override void CopyProperties()
    {
        this.Startup = this.PreferenceModel.Startup;

        var sleepSettings = this.PreferenceModel.SleepSettings;

        if (sleepSettings.KeyCombo.Modifiers.Count() > 0)
        {
            this.SleepMinModifierFirst = sleepSettings.KeyCombo.Modifiers[0];
        }

        if (sleepSettings.KeyCombo.Modifiers.Count() > 1)
        {
            this.SleepMinModifierSecond = sleepSettings.KeyCombo.Modifiers[1];
        }

        if (sleepSettings.KeyCombo.Modifiers.Count() > 2)
        {
            this.SleepMinModifierThird = sleepSettings.KeyCombo.Modifiers[2];
        }

        this.SleepMinKeyCode = sleepSettings.KeyCombo.ExecutionKey;

        if (sleepSettings.MaxKeyCombo.Modifiers.Count() > 0)
        {
            this.SleepMaxModifierFirst = sleepSettings.MaxKeyCombo.Modifiers[0];
        }

        if (sleepSettings.MaxKeyCombo.Modifiers.Count() > 1)
        {
            this.SleepMaxModifierSecond = sleepSettings.MaxKeyCombo.Modifiers[1];
        }

        if (sleepSettings.MaxKeyCombo.Modifiers.Count() > 2)
        {
            this.SleepMaxModifierThird = sleepSettings.MaxKeyCombo.Modifiers[2];
        }

        this.SleepMaxKeyCode = sleepSettings.MaxKeyCombo.ExecutionKey;

        this.SleepMinValue = sleepSettings.MinData.Minutes;
        this.SleepMaxValue = sleepSettings.MaxData.Minutes;
    }

    private void BindKeys()
    {
        this.WhenAnyValue(
            vm => vm.SleepMinModifierFirst,
            vm => vm.SleepMinModifierSecond,
            vm => vm.SleepMinModifierThird,
            vm => vm.SleepMinKeyCode)
            .Subscribe(_ => this.sleepMinKeysSource.Edit(list =>
            {
                list.Clear();
                if (this.SleepMinModifierFirst != ModifierMask.None)
                {
                    list.Add(this.SleepMinModifierFirst);
                }

                if (this.SleepMinModifierSecond != ModifierMask.None)
                {
                    list.Add(this.SleepMinModifierSecond);
                }

                if (this.SleepMinModifierThird != ModifierMask.None)
                {
                    list.Add(this.SleepMinModifierThird);
                }
            }));

        this.WhenAnyValue(
            vm => vm.SleepMaxModifierFirst,
            vm => vm.SleepMaxModifierSecond,
            vm => vm.SleepMaxModifierThird,
            vm => vm.SleepMaxKeyCode)
            .Subscribe(_ => this.sleepMaxKeysSource.Edit(list =>
            {
                list.Clear();
                if (this.SleepMaxModifierFirst != ModifierMask.None)
                {
                    list.Add(this.SleepMaxModifierFirst);
                }

                if (this.SleepMaxModifierSecond != ModifierMask.None)
                {
                    list.Add(this.SleepMaxModifierSecond);
                }

                if (this.SleepMaxModifierThird != ModifierMask.None)
                {
                    list.Add(this.SleepMaxModifierThird);
                }
            }));
    }

    private ValidationHelper InitModifierKeysAreDifferentRule()
    {
        var modifierKeysAreDifferent = Observable.CombineLatest(
            this.sleepMinKeysSource
                .Connect()
                .ToCollection()
                .Select(this.ContainsDistinctElements),
            this.sleepMaxKeysSource
                .Connect()
                .ToCollection()
                .Select(this.ContainsDistinctElements),
            (minKeys, maxKeys) => minKeys && maxKeys);

        return this.LocalizedValidationRule(modifierKeysAreDifferent, "ModifierKeysAreSame");
    }

    private bool ContainsDistinctElements(IReadOnlyCollection<ModifierMask> keys) =>
        !keys
            .SelectMany((x, index1) =>
                keys.Select((y, index2) => (Key1: x, Key2: y, Index1: index1, Index2: index2)))
            .Where(keys => keys.Index1 < keys.Index2)
            .Select(keys => (keys.Key1 & keys.Key2) == ModifierMask.None)
            .Any(equals => !equals);
}
