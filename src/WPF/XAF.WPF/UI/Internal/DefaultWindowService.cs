using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.X86;
using System.Windows;
using XAF.Core;
using XAF.Core.MVVM;
using XAF.Core.UI;
using XAF.WPF.Hosting;

namespace XAF.WPF.UI.Internal;
internal class DefaultWindowService : IWindowService
{
    private readonly IViewLocator _viewLocator;
    private readonly IViewModelLocator _viewModelLocator;
    private readonly WpfEnvironment _wpfEnvironment;
    private Type _defaultWindowType;

    private readonly Dictionary<IXafViewModel, Window> _openWindows = new Dictionary<IXafViewModel, Window>();

    public DefaultWindowService(IViewLocator viewLocator, IViewModelLocator viewModelLocator, WpfEnvironment wpfEnvironment)
    {
        _viewLocator = viewLocator;
        _viewModelLocator = viewModelLocator;
        _wpfEnvironment = wpfEnvironment;
    }

    public IObservable<DialogParameters> DialogClosed()
    {
        throw new NotImplementedException();
    }

    public IObservable<DialogParameters> DialogOpened()
    {
        throw new NotImplementedException();
    }

    public IObservable<DialogParameters> OpenDialogRequested()
    {
        throw new NotImplementedException();
    }

    public Task<bool> OpenWindowAsync<TViewModel>(CancellationToken cancle)
        where TViewModel : IXafViewModel
    {
        var vm = _viewModelLocator.CreateVm<TViewModel>();
        return OpenWindowAsync(vm, cancle);
    }

    public async Task<bool> OpenWindowAsync<TViewModel>(TViewModel vm, CancellationToken cancle)
        where TViewModel : IXafViewModel
    {
        var view = _viewLocator.GetViewFor<TViewModel>();
        _viewModelLocator.Prepare(vm, view);

        if (view is not Window window)
        {
            window = await _wpfEnvironment.Dispatcher.InvokeAsync(() => (Window)Activator.CreateInstance(_defaultWindowType)!);
            window!.DataContext = vm;
        }

        await _wpfEnvironment.Dispatcher.InvokeAsync(window.Show);

        _openWindows.Add(vm, window);

        return true;
    }

    public async Task<bool> OpenWindowAsync<TViewModel, TParameter>(TViewModel vm, TParameter parameter, CancellationToken cancle)
        where TViewModel : IXafViewModel<TParameter>
    {
        var view = _viewLocator.GetViewFor<TViewModel>();
        _viewModelLocator.Prepare(vm, parameter, view);

        if (view is not Window window)
        {
            window = await _wpfEnvironment.Dispatcher.InvokeAsync(() => (Window)Activator.CreateInstance(_defaultWindowType)!);
            window!.DataContext = vm;
        }

        await _wpfEnvironment.Dispatcher.InvokeAsync(window.Show);

        _openWindows.Add(vm, window);

        return true;
    }

    public Task<bool> OpenWindowAsync<TViewModel, TParameter>(TParameter parameter, CancellationToken cancle)
         where TViewModel : class, IXafViewModel<TParameter>
    {
        var vm = _viewModelLocator.CreateVm<TViewModel>();
        return OpenWindowAsync(vm, parameter, cancle);
    }

    public async Task<bool> CloseWindowAsync<TViewModel>(TViewModel vm, CancellationToken cancle1) where TViewModel : IXafViewModel
    {
        if (_openWindows.TryGetValue(vm, out var window))
        {
            await _wpfEnvironment.Dispatcher.InvokeAsync(window.Close);
            _openWindows.Remove(vm);
            return true;
        }

        return true;
    }

    public Task<TResult> ShowDialogAsync<TResult, TViewModel>()
        where TViewModel : class, IXafDialogViewModel<TResult>
    {
        var vm = _viewModelLocator.CreateVm<TViewModel>();
        return ShowDialogAsync<TResult, TViewModel>(vm);
    }

    public async Task<TResult> ShowDialogAsync<TResult, TViewModel>(TViewModel vm)
        where TViewModel : class, IXafDialogViewModel<TResult>
    {
        var view = _viewLocator.GetViewFor<TViewModel>();
        _viewModelLocator.Prepare(vm, view);

        var tcs = new TaskCompletionSource<TResult>();

        if (view is not Window window)
        {
            window = await _wpfEnvironment.Dispatcher.InvokeAsync(() => (Window)Activator.CreateInstance(_defaultWindowType)!);
            window!.DataContext = vm;
        }

        void WindowClosing(object? sender, CancelEventArgs e)
        {
            if (!vm.Cancle())
            {
                e.Cancel = true;
                return;
            }

            window.Closing -= WindowClosing;
        }

        window.Closing += WindowClosing;

        void SetResult(object? sender, TResult result)
        {
            tcs.SetResult(result);
            vm.CloseDialogRequested -= SetResult;
        }

        vm.CloseDialogRequested += SetResult;

        return await tcs.Task;
    }

    public Task<TResult> ShowDialogAsync<TResult, TViewModel, TParameter>(TParameter parameter)
       where TViewModel : class, IXafDialogViewModel<TResult, TParameter>
    {
        var vm = _viewModelLocator.CreateVm<TViewModel>();

        return ShowDialogAsync<TResult, TViewModel, TParameter>(vm, parameter);
    }

    public async Task<TResult> ShowDialogAsync<TResult, TViewModel, TParameter>(TViewModel vm, TParameter parameter)
        where TViewModel : class, IXafDialogViewModel<TResult, TParameter>
    {
        var view = _viewLocator.GetViewFor<TViewModel>();
        _viewModelLocator.Prepare(vm, view);

        var tcs = new TaskCompletionSource<TResult>();

        if (view is not Window window)
        {
            window = await _wpfEnvironment.Dispatcher.InvokeAsync(() => (Window)Activator.CreateInstance(_defaultWindowType)!);
            window!.DataContext = vm;
        }

        void WindowClosing(object? sender, CancelEventArgs e)
        {
            if (!vm.Cancle())
            {
                e.Cancel = true;
                return;
            }

            window.Closing -= WindowClosing;
        }

        window.Closing += WindowClosing;

        void SetResult(object? sender, TResult result)
        {
            tcs.SetResult(result);
            vm.CloseDialogRequested -= SetResult;
        }

        vm.CloseDialogRequested += SetResult;

        return await tcs.Task;
    }

    public void SetDefaultWindow<TWindow>()
        where TWindow : Window
    {
        _defaultWindowType = typeof(TWindow);
    }
}
