using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XAF.Modularity.Catalogs;

namespace XAF.Modularity;
public interface IModuleDescription
{
    string Name { get; }

    string Description { get; }

    Version Version { get; }

    Type ModuleType { get; }

    Assembly Assembly { get; }

    IModuleCatalog Source { get; }

    IList<ServiceDescriptor> Services { get; }

    object? Instance { get; }

    List<IModuleHandler> Handlers { get; }

    bool SetInstance(object instance);
}
