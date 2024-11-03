using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using XAF.UI.Abstraction;
using XAF.UI.Abstraction.Attributes;
using XAF.UI.Abstraction.ViewComposition;
using XAF.UI.Abstraction.ViewModels;
using XAF.UI.WPF.Attributes;
using XAF.UI.WPF.Hosting;

namespace XAF.UI.WPF.ViewComposition.Internal;
internal class DefaultWindowService : IWindowService
{
    private readonly IBundleProvider _bundleProvider;
    private readonly IServiceProvider _serviceProvider;
    private readonly IWpfEnvironment _wpfEnvironment;
    private readonly List<IXafBundle> _shells = new();
    private readonly List<IXafBundle> _openWindows = new();
    private Type _defaultWindowType = typeof(Window);

    public DefaultWindowService(IBundleProvider bundleProvider, IServiceProvider serviceProvider, IWpfEnvironment wpfEnvironment)
    {
        _bundleProvider = bundleProvider;
        _serviceProvider = serviceProvider;
        _wpfEnvironment = wpfEnvironment;

        _bundleProvider.CacheBundles = false;
    }

    public async Task CloseAsync<TViewModel>() where TViewModel : IXafViewModel
    {
        var toClose = _openWindows.FindAll(b => b.ViewModelType == typeof(TViewModel));
        foreach (var bundle in toClose)
        {
            if (bundle.View is not Window window)
            {
                continue;
            }
            window.Close();
        }
    }

    public async Task CloseAsync<TViewModel>(TViewModel viewModel) where TViewModel : IXafViewModel
    {
        var bundle = _openWindows.Find(b => b.ViewModel.Equals(viewModel));
        if (bundle is null)
        {
            return;
        }

        if (bundle.View is not Window window)
        {
            return;
        }

        window.Close();
    }

    public async Task ShowAsync<TViewModel>() where TViewModel : IXafViewModel
    {
        var bundle = await _bundleProvider.CreateBundleAsync<TViewModel>().ConfigureAwait(false);
        await ShowAsync(bundle);
    }

    public async Task ShowAsync<TViewModel>(TViewModel viewModel) where TViewModel : IXafViewModel
    {
        var bundle = await _bundleProvider.CreateBundleAsync(viewModel).ConfigureAwait(false);
        await ShowAsync(bundle);
    }

    public async Task ShowAsync(IXafBundle bundle)
    {
        var bundleWindow = GetWindow(bundle);

        bundle.ViewModel.Prepare();
        bundleWindow.Closed += (s, e) => bundle.ViewModel.UnloadAsync();
        Schedulers.MainScheduler.Schedule(() => bundleWindow.Show());
        await bundle.ViewModel.LoadAsync().ConfigureAwait(false);

    }

    public async Task<TViewModel> ShowDialogAsync<TViewModel>() where TViewModel : IXafViewModel
    {
        var bundle = await _bundleProvider.CreateBundleAsync<TViewModel>().ConfigureAwait(false);
        await ShowDialogAsync(bundle).ConfigureAwait(false);
        return bundle.ViewModel;
    }

    public async Task ShowDialogAsync<TViewModel>(TViewModel viewModel) where TViewModel : IXafViewModel
    {
        var bundle = await _bundleProvider.CreateBundleAsync(viewModel).ConfigureAwait(false);
        await ShowDialogAsync(bundle)
            .ConfigureAwait(false);
    }

    public async Task<TViewModel> ShowDialogAsync<TViewModel, TParameter>(TParameter parameter) where TViewModel : IXafViewModel<TParameter>
    {
        var bundle = await _bundleProvider.CreateBundleAsync<TViewModel>().ConfigureAwait(false);
        await ShowDialogAsync(bundle, parameter).ConfigureAwait(false);
        return bundle.ViewModel;
    }

    public async Task ShowDialogAsync<TViewModel, TParameter>(TViewModel viewModel, TParameter parameter) where TViewModel : IXafViewModel<TParameter>
    {
        var bundle = await _bundleProvider.CreateBundleAsync(viewModel).ConfigureAwait(false);

        await ShowDialogAsync(bundle, parameter);
    }

    public async Task ShowDialogAsync(IXafBundle bundle)
    {
        var bundleWindow = GetWindow(bundle);
        bundleWindow.Closed += (s, e) => bundle.ViewModel.UnloadAsync();

        bundle.ViewModel.Prepare();
        Schedulers.MainScheduler.Schedule(() => bundleWindow.ShowDialog());
        await bundle.ViewModel.LoadAsync().ConfigureAwait(false);
        await bundle.ViewModel.WaitForViewClose();
    }

    public async Task ShowDialogAsync<TParameter>(IXafBundle bundle, TParameter parameter)
    {
        var bundleWindow = GetWindow(bundle);
        bundleWindow.Closed += (s, e) => bundle.ViewModel.UnloadAsync();

        var vm = (IXafViewModel<TParameter>)bundle.ViewModel
            ?? throw new ArgumentException($"The bundle ViewModle is not an {typeof(IXafViewModel<TParameter>)}");

        vm.Prepare();
        vm.Prepare(parameter);
        Schedulers.MainScheduler.Schedule(() => bundleWindow.ShowDialog());
        await bundle.ViewModel.LoadAsync().ConfigureAwait(false);
        await bundle.ViewModel.WaitForViewClose();
    }

    public async Task ShowShells()
    {
        bool hasShell = false;
        foreach (var bundle in _shells)
        {
            if (bundle.View is not Window window)
            {
                continue;
            }

            Schedulers.MainScheduler.Schedule(window.Show);

            if (!hasShell)
            {
                _wpfEnvironment.WpfDispatcher.Invoke(() => _wpfEnvironment.WpfApp!.MainWindow = window);
            }

            hasShell = true;
        }

        if (!hasShell)
        {
            throw new InvalidOperationException("No shell window found");
        }
    }

    public async Task PrepareShells()
    {
        var bundles = _bundleProvider.CreateBundlesWithDecoratorAsync<ShellAttribute>();
        await foreach (var bundle in bundles)
        {
            _shells.Add(bundle);
            bundle.ViewModel.Prepare();
        }
    }

    public async Task LoadShells()
    {
        foreach (var bundle in _shells)
        {
            await bundle.ViewModel.LoadAsync().ConfigureAwait(false);
        }
    }

    public void SetDefaultWindowType<TWindow>() where TWindow : class
    {
        SetDefaultWindowType(typeof(TWindow));
    }

    public void SetDefaultWindowType(Type type)
    {
        if (!type.IsAssignableTo(typeof(Window)))
        {
            throw new InvalidOperationException($"Window must be of type {typeof(Window)}");
        }

        _defaultWindowType = type;
    }

    private Window GetWindow(IXafBundle bundle)
    {
        if (bundle.View is Window window)
        {
            return window;
        }

        if (bundle.ViewDecorators.TryGetDecorator<WindowAttribute>(out var windowAttribute))
        {
            window = (Window)ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, windowAttribute.WindowType);
            window.Content = bundle.View;
            return window;
        }

        window = (Window)ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, _defaultWindowType);
        window.Content = bundle.View;
        return window;
    }
}
