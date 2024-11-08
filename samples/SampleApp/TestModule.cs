using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration.Internal;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XAF.Core.Modularity;
using XAF.Core.Modularity.Attributes;

namespace SampleApp;
[Exports<ITestService>]
public class TestModule : IServiceModule
{
    private readonly ILogger<TestModule> _logger;

    public TestModule(ILogger<TestModule> logger)
    {
        _logger = logger;
    }

    public void ConfigureLogging(ILoggingBuilder loggingBuilder)
    {        
    }

    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        _logger.LogDebug("Registering services");
        services.AddTransient<ITestService, TestService>();
    }

    public Task StartAsync(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _logger.LogDebug("Start Async Called");
        serviceProvider.GetRequiredService<ITestService>().PrintToConsole();
        return Task.CompletedTask;
    }
}
