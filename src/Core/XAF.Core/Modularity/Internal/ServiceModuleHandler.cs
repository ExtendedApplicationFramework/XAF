using DynamicData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;
using XAF.Core.Modularity.Attributes;
using XAF.Modularity;

namespace XAF.Core.Modularity.Internal;
internal class ServiceModuleHandler : ModuleHandler<IServiceModule>
{
    private readonly Dictionary<IServiceModule, ServiceCollection> _serviceCollections = new Dictionary<IServiceModule, ServiceCollection>();

    private readonly IList<ServiceDescriptor> _hostServices;
    private readonly ILogger<ServiceModuleHandler> _logger;
    private readonly List<ServiceDescriptor> _exportedServices = new();

    public ServiceModuleHandler(IServiceCollection hostServices, ILogger<ServiceModuleHandler> logger)
    {
        _hostServices = hostServices;
        _logger = logger;
    }

    public override Task LoadAsync(IServiceModule module)
    {
        var collection = new ServiceCollection();
        collection.AddRange(_hostServices);
        module.RegisterServices(collection);
        var hostedServices = collection.Where(s => s.ServiceType == typeof(IHostedService));

        if (hostedServices.Any())
        {
            _logger.LogWarning("Hosted Services are not supported in service module");
        }

        collection.Remove(hostedServices);

        _serviceCollections.Add(module, collection);
        var builder = new LoggingBuilder(collection);
        module.ConfigureLogging(builder);

        var exportAttributes = module.GetType().GetCustomAttributes(typeof(ExportsAttribute<>));

        foreach (var exportAttribute in exportAttributes)
        {
            var type = exportAttribute.GetType().GetGenericArguments()[0];
            var service = collection.FirstOrDefault(d => d.ServiceType == type);
            if (service is null)
            {
                throw new InvalidOperationException($"No service of type {type.FullName} registered in Module {module.GetType()}");
            }

            _exportedServices.Add(service);
        }

        return Task.CompletedTask;
    }

    public override Task StartAsync(IServiceModule module)
    {
        var serviceCollection = _serviceCollections[module];

        serviceCollection.AddRange(_exportedServices);

        var provider = serviceCollection.BuildServiceProvider();

        return module.StartAsync(provider);
    }


    private class LoggingBuilder : ILoggingBuilder
    {
        public IServiceCollection Services { get; }

        public LoggingBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
