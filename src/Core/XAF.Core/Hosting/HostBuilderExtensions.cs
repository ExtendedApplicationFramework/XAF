using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using XAF.Core.Modularity.Internal;
using XAF.Modularity;
using XAF.Modularity.Context;

namespace XAF.Core.Hosting;
public static class HostBuilderExtensions
{
    public static IHostApplicationBuilder UseXaf(this IHostApplicationBuilder builder)
    {
        builder.Services.AddModularity();
        builder.Services.AddSingleton<IModuleManager, DiModuleManager>();
        builder.Services.AddHostedService<XafConfigurationService>();
        builder.Services.AddModuleHandler<ModuleHandler>();
        builder.Services.AddModuleHandler<ServiceModuleHandler>();
        
        var globalServices = new List<ServiceDescriptor>(builder.Services);
        builder.Services.AddSingleton<IList<ServiceDescriptor>>(globalServices);

        return builder;
    }

    private class XafConfigurationService : IHostedLifecycleService
    {
        private readonly IServiceProvider _services;

        public XafConfigurationService(IServiceProvider services)
        {
            _services = services;
        }

        public Task StartingAsync(CancellationToken cancellationToken)
        {
            ModuleContextLoaderOptions.Default.LoggerFactory = _services.GetRequiredService<ILogger<ModuleContextLoader>>;
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
            return Task.CompletedTask;
        }
    }

}
