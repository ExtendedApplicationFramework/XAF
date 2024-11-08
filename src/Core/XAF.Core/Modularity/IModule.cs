using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAF.Modularity;
public interface IModule
{
    Task StartAsync(IServiceProvider services, IConfiguration configuration);
}
