using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XAF.Modularity;

namespace SampleApp;
internal class HostedService : IHostedService
{
    private readonly IModuleManager _moduleManager;

    public HostedService(IModuleManager moduleManager)
    {
        _moduleManager = moduleManager;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _moduleManager.DiscoverModules();
        await _moduleManager.LoadModules();
        await _moduleManager.StartLoadedModules();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
