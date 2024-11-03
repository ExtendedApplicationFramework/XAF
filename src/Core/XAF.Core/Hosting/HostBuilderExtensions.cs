﻿using Microsoft.Extensions.DependencyInjection;
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
        builder.Services.AddHostedService<XafConfigurationService>();
        builder.Services.AddModuleHandler<ServiceModuleHandler>();
        builder.Services.AddSingleton(builder.Services);

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
            ModuleManagerOptions.Default.ModuleInstanceFactory = module => Task.FromResult(ActivatorUtilities.GetServiceOrCreateInstance(_services, module.Type));

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
