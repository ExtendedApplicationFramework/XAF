﻿using System.Windows;
using System.Diagnostics;
using XAF.Hosting.Abstraction;
using XAF.UI.Abstraction;
using XAF.UI.WPF.Hosting;

namespace XAF.UI.WPF.StartupActions;
internal class WpfAppSplashScreenInitializer : IHostStartupAction
{
    private readonly IWpfThread _wpfThread;
    private readonly ISplashWindowViewModel _splashViewModel;

    public WpfAppSplashScreenInitializer(IWpfThread wpfThread, ISplashWindowViewModel splashViewModel)
    {
        _wpfThread = wpfThread;
        _splashViewModel = splashViewModel;
    }

    public int Priority => UiStartupActionPriorities.ShowSplashScreen;
    public HostStartupActionExecution ExecutionTime => HostStartupActionExecution.AfterHostedServicesStarted;

    public async Task Execute(CancellationToken cancellation)
    {
        if (_splashViewModel != null)
        {
            await _wpfThread.UiDispatcher!.InvokeAsync(() =>
            {
                var splashWindow = Activator.CreateInstance(_splashViewModel.WindowType) as Window ??
                    throw new NotSupportedException("the provided splashWindowType is not valid. The splash window must be an Window and " +
                    "it must contain a parameterless constructor");

                splashWindow.DataContext = _splashViewModel;
                if (!_wpfThread.AppCreated)
                {
                    throw new UnreachableException();
                }

                _wpfThread.SplashWindow = splashWindow;
                _wpfThread.Application.MainWindow = splashWindow;
                splashWindow.Show();
            });
            await _splashViewModel.OnAppStartAsync();
        }
    }
}

internal class WpfAppShellAfterModuleInitialization : IHostStartupAction
{
    private readonly IWpfThread _wpfThread;
    private readonly ISplashWindowViewModel _splashViewModel;

    public int Priority => UiStartupActionPriorities.ShowSplashScreen + 1;
    public HostStartupActionExecution ExecutionTime => HostStartupActionExecution.AfterHostedServicesStarted;

    public WpfAppShellAfterModuleInitialization(IWpfThread wpfThread, ISplashWindowViewModel splashViewModel)
    {
        _wpfThread = wpfThread;
        _splashViewModel = splashViewModel;
    }

    public async Task Execute(CancellationToken cancellation)
    {
        await _splashViewModel.AfterModuleInitializationAsync().ConfigureAwait(false);
    }
}
