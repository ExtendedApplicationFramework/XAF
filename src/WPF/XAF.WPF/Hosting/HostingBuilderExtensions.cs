using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using XAF.Core.Hosting;
using XAF.Core.UI;
using XAF.Modularity.Context;
using XAF.WPF.Modularity.Internal;
using XAF.WPF.UI;
using XAF.WPF.UI.Internal;

namespace XAF.WPF.Hosting;
public static class HostingBuilderExtensions
{
    public static IServiceCollection AddWpfApp(this IServiceCollection services)
    {
        services
            .AddWpfServices()
            .AddTransient<Application>();

        return services;
    }

    public static IServiceCollection AddWpfApp<TApp>(this IServiceCollection services)
        where TApp : Application
    {
        services
            .AddWpfServices()
            .AddTransient<Application, TApp>();

        return services;
    }

    public static IServiceCollection AddWpfServices(this IServiceCollection services)
    {
        services.AddSingleton<IViewAdapterLocator, DefaultViewAdapterLocator>()
            .AddSingleton<IViewLocator, DefaultViewLocator>()
            .AddSingleton<IViewCompositionService, DefaultViewCompositionService>()
            .AddSingleton<IViewModelPresenterFactory, DefaultViewModelPresenterFactory>()
            .AddSingleton<ViewModelPresenterLocator>()
            .AddTransient<IViewCollection, ViewCollection>()
            .AddModuleHandler<WpfModuleHandler>();

        var wpfAssemblyLocation = typeof(Window).Assembly.Location;

        if (!ModuleContextLoaderOptions.Default.AdditionalRuntimePaths.Contains(wpfAssemblyLocation))
        {
            ModuleContextLoaderOptions.Default.AdditionalRuntimePaths.Add(wpfAssemblyLocation);
        }

        return services;
    }
}
