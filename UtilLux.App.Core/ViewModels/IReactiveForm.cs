﻿using ReactiveUI;

namespace UtilLux.App.Core.ViewModels;

public interface IReactiveForm : IReactiveObject
{
    IObservable<bool> FormChanged { get; }
    bool IsFormChanged { get; }

    IObservable<bool> Valid { get; }
}
