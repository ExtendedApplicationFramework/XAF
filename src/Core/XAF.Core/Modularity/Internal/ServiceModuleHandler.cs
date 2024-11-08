using DynamicData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;
using XAF.Core.Modularity.Attributes;
using XAF.Modularity;

namespace XAF.Core.Modularity.Internal;
internal class ServiceModuleHandler : ModuleHandler<IServiceModule>
{
    private readonly IList<ServiceDescriptor> _globalServices;
    private readonly ILogger<ServiceModuleHandler> _logger;
    private readonly IConfiguration _configuration;

    private IList<ServiceDescriptor> _moduleServices = [];

    public ServiceModuleHandler(IList<ServiceDescriptor> global, ILogger<ServiceModuleHandler> logger, IConfiguration configuration)
    {
        _globalServices = global;
        _logger = logger;
        _configuration = configuration;
    }

    public override Task ConfigureHandler(IModuleDescription description)
    {
        _moduleServices = description.Services;
        return Task.CompletedTask;
    }

    protected override Task LoadAsync(IServiceModule module)
    {
        var collection = new ServiceCollection();
        collection.AddRange(_globalServices);
        module.RegisterServices(collection, _configuration);
        var hostedServices = collection
            .Except(_globalServices)
            .Where(s => s.ServiceType == typeof(IHostedService));

        if (hostedServices.Any())
        {
            _logger.LogWarning("Registering hosted services is not supported in modules");
        }

        collection.Remove(hostedServices);

        var builder = new LoggingBuilder(collection);
        module.ConfigureLogging(builder);

        var exportAttributes = module.GetType().GetCustomAttributes(typeof(ExportsAttribute<>));

        foreach (var exportAttribute in exportAttributes)
        {
            var type = exportAttribute.GetType().GetGenericArguments()[0];
            var service = collection.FirstOrDefault(d => d.ServiceType == type)
                ?? throw new InvalidOperationException($"No service of type {type.FullName} registered in Module {module.GetType()}");

            _globalServices.Add(service);
        }
        _moduleServices.Add(collection);
        return Task.CompletedTask;
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
