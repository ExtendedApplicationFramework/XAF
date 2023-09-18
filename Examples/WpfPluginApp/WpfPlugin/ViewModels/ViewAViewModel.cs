﻿using System.Reactive.Linq;
using XAF.UI;
using XAF.UI.Abstraction;
using XAF.UI.Reactive.Commands;
using XAF.UI.Reactive.ReactiveProperty;

namespace WpfPlugin.ViewModels;
public class ViewAViewModel : ViewModelBase, IActivatableViewModel
{
    public RxProperty<string> Message { get; } = new();

    public RxCommand NavigateToViewBCommand { get; }

    public ViewAViewModel(INavigationService navigationService)
    {
        // Executes navigation to View B. 
        // Can only be executed if Message is not empty.
        NavigateToViewBCommand = RxCommand.Create(
            () => navigationService.NavigateTo<ViewBViewModel, string>("PageViews", Message),
            Message.Select(s => !string.IsNullOrWhiteSpace(s)));
    }

    public void OnDeactivated()
    {
        // Do Some stuff after navigating away from this view.
    }

    public void OnActivated()
    {
        // Do Some stuff after navigating to this view.
        Message.Value = string.Empty;
    }
}
