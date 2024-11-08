using DynamicData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XAF.Modularity;

namespace XAF.Core.Modularity.Internal;
internal class ModuleHandler : ModuleHandler<IModule>
{
    private readonly IList<ServiceDescriptor> _globalServices;
    private readonly IConfiguration _configuration;

    private IList<ServiceDescriptor> _moduleServices = [];

    public ModuleHandler(IList<ServiceDescriptor> globalServices, IConfiguration configuration)
    {
        _globalServices = globalServices;
        _configuration = configuration;
    }

    public override Task ConfigureHandler(IModuleDescription description)
    {
        _moduleServices = description.Services;

        return Task.CompletedTask;
    }

    protected override Task StartAsync(IModule module)
    {
        var services = new ServiceCollection();
        services.AddRange(_globalServices);
        services.AddRange(_moduleServices);

        return module.StartAsync(services.BuildServiceProvider(), _configuration);
    }
}
