using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System.Windows;
using XAF.UI.Abstraction.ViewComposition;
using XAF.UI.ViewComposition;
using XAF.UI.WPF.Hosting.Internal;
using XAF.UI.WPF.ViewComposition;
using XAF.UI.WPF.ViewComposition.Internal;

namespace XAF.UI.WPF.Hosting;
public static class HostBuilderExtensions
{
    public static IHostApplicationBuilder AddWPFApp(this IHostApplicationBuilder builder)
    {
        return builder.AddWpfApp<Application>();
    }

    public static IHostApplicationBuilder AddWpfApp<TApplication>(this IHostApplicationBuilder builder)
        where TApplication : Application
    {
        builder.Services.AddDefaultUiServices();
        builder.Services.AddSingleton<WpfEnvironment>();
        builder.Services.AddSingleton<IWpfEnvironment>((s) => s.GetRequiredService<WpfEnvironment>());
        builder.Services.TryAddSingleton<Application, TApplication>();

        builder.Services.AddHostedService<WpfService>();
        builder.Services.TryAddTransient<IBundleProvider, BundleProvider>();
        builder.Services.TryAddSingleton<IWindowService, DefaultWindowService>();
        builder.Services.TryAddSingleton<IViewPresenter, ViewPresenter>();

        return builder;
    }
}
