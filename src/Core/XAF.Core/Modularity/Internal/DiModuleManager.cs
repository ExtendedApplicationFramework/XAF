using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XAF.Modularity;
using XAF.Modularity.Catalogs;

namespace XAF.Core.Modularity.Internal;
internal class DiModuleManager : ModuleManager
{
    private readonly ILogger<DiModuleManager> _logger;
    private readonly IServiceProvider _serviceProvider;

    public DiModuleManager(
        IEnumerable<IModuleHandler> moduleHandlers,
        IEnumerable<IModuleCatalog> moduleCatalogs,
        ILogger<ModuleManager> logger,
        ILogger<DiModuleManager> diLogger,
        IServiceProvider serviceProvider)
        : base(moduleHandlers, moduleCatalogs, logger)
    {
        _logger = diLogger;
        _serviceProvider = serviceProvider;
    }

    protected override object CreateModule(IModuleDescription moduleDescription)
    {
        return ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, moduleDescription.ModuleType);
    }
}
