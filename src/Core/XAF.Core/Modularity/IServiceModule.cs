using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using XAF.Modularity;

namespace XAF.Core.Modularity;

public interface IServiceModule : IModule
{
    void RegisterServices(IServiceCollection services, IConfiguration configuration);

    void ConfigureLogging(ILoggingBuilder loggingBuilder);
}
