﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Windows;
using XAF.Hosting.Abstraction;
using XAF.Modularity.Extensions;
using XAF.UI.Abstraction;
using XAF.UI.WPF.Hosting.Internal;
using XAF.UI.WPF.Internal;
using XAF.UI.WPF.StartupActions;
using XAF.UI.WPF.ViewAdapters;
using XAF.UI.WPF.ViewComposition;

namespace XAF.UI.WPF.Hosting;
public static class HostBuilderExtensions
{
    public static IRxHostBuilder UseWPF(this IRxHostBuilder builder)
    {
        SetupBaseApp(builder);
        builder.Services.TryAddSingleton<Application, Application>();
        return builder;
    }

    public static IRxHostBuilder ConfigureWpfApp<TApplication>(this IRxHostBuilder builder)
        where TApplication : Application
    {

        SetupBaseApp(builder);
        builder.Services.TryAddSingleton<Application, TApplication>();
        return builder;
    }

    public static IRxHostBuilder UseSplashWindow<TViewModel>(this IRxHostBuilder builder)
        where TViewModel : class, ISplashWindowViewModel
    {
        builder.Services.AddSingleton<ISplashWindowViewModel, TViewModel>();
        builder.Services.AddStartupActions<WpfAppSplashScreenInitializer>();
        builder.Services.AddStartupActions<WpfAppShellAfterModuleInitialization>();
        return builder;
    }

    private static void SetupBaseApp(IRxHostBuilder builder)
    {
        var viewAdapters = new ViewAdapterCollection();
        var viewCollection = new ViewCollection(builder.Services);

        builder.Services.AddHostedService<WpfApp>();
        builder.Services.AddStartupActions<WpfAppInitializer>();

        builder.Services.AddSingleton<IHostLifetime, WpfLifetime>();
        builder.Services.AddSingleton<IViewAdapterCollection>(viewAdapters);
        builder.Services.AddSingleton<IViewCollection>(viewCollection);

        builder.Services.TryAddSingleton<IWpfThread, WpfThread>();
        builder.Services.TryAddSingleton<IViewCompositionService, ViewCompositionService>();
        builder.Services.TryAddSingleton<IViewProvider, ViewProvider>();
        builder.Services.TryAddSingleton<INavigationService, NavigationService>();
        builder.Services.TryAddSingleton<IViewCompositionService, ViewCompositionService>();

        var executingAssembly = Assembly.GetEntryAssembly()!;

        viewCollection.AddViewsFromAssembly(executingAssembly);
        viewAdapters.AddAdaptersFromAssembly(executingAssembly);
        viewAdapters.AddAdaptersFromAssembly(Assembly.GetAssembly(typeof(ContentControlAdapter))!);

        builder.UseModularity();
        builder.UseModuleRegistrationContextBuilder(new WpfModuleContextBuilder(viewCollection, viewAdapters));
    }
}
