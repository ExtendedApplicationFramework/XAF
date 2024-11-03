using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;
using XAF.Modularity;
using XAF.Modularity.Catalogs;
using XAF.Modularity.Context;

namespace XAF.Core.Hosting;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddModuleHandler<THandler>(this IServiceCollection services)
        where THandler : class, IModuleHandler
    {
        services.TryAddEnumerable(new ServiceDescriptor(typeof(IModuleHandler), typeof(THandler)));
        return services;
    }

    public static IServiceCollection AddModularity(this IServiceCollection services)
    {
        services.TryAddSingleton<IModuleManager, ModuleManager>();
        return services;
    }

    public static IServiceCollection AddModuleCatalog(this IServiceCollection services, IModuleCatalog catalog)
    {
        services.TryAddEnumerable(new ServiceDescriptor(typeof(IModuleCatalog), catalog));
        return services;
    }

    public static IServiceCollection AddModuleFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        services.AddModuleCatalog(new AssemblyModuleCatalog(assembly));
        return services;
    }

    public static IServiceCollection AddModuleFromAssembly(this IServiceCollection services, Assembly assembly, Action<AssemblyModuleCatalogOptions> configure)
    {
        var options = new AssemblyModuleCatalogOptions();
        configure(options);
        services.AddModuleCatalog(new AssemblyModuleCatalog(assembly, options));
        return services;
    }

    public static IServiceCollection AddModuleFromAssembly(this IServiceCollection services, string assemblyPath, Action<AssemblyModuleCatalogOptions> configure)
    {
        var options = new AssemblyModuleCatalogOptions();
        configure(options);
        services.AddModuleCatalog(new AssemblyModuleCatalog(assemblyPath, options));
        return services;
    }

    public static IServiceCollection AddModuleFromAssembly(this IServiceCollection services, string assemblyPath)
    {
        services.AddModuleCatalog(new AssemblyModuleCatalog(assemblyPath));
        return services;
    }

    public static IServiceCollection AddModuleFromFolder(this IServiceCollection services, string folderPath)
    {
        services.AddModuleCatalog(new FolderModuleCatalog(folderPath));
        return services;
    }

    public static IServiceCollection AddModuleFromFolder(this IServiceCollection services, string folderPath, Action<FolderModuleCatalogOptions> configure)
    {
        var options = new FolderModuleCatalogOptions();
        configure(options);
        services.AddModuleCatalog(new FolderModuleCatalog(folderPath, options));
        return services;
    }

    private class ModularityConfigurationService : IHostedLifecycleService
    {
        private readonly IServiceProvider _serviceProvider;

        public ModularityConfigurationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartingAsync(CancellationToken cancellationToken)
        {
            ModuleContextLoaderOptions.Default.LoggerFactory = _serviceProvider.GetRequiredService<ILogger<ModuleContextLoader>>;
            return Task.CompletedTask;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StartedAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StoppedAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StoppingAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
