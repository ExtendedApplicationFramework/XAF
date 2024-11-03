namespace XAF.Modularity;

public class ModuleManagerOptions
{
    public Func<Module, Task<object>> ModuleInstanceFactory { get; set; } = Default.ModuleInstanceFactory;

    private static Task<object> CreateModuleInstance(Module module)
    {
        var ctor = module.Type.GetConstructor([]);

        return ctor == null
            ? throw new NotSupportedException($"Can't load module {module.Type.FullName}. Default module Factory only supports modules with empty constructor")
            : Task.FromResult(ctor.Invoke([]));
    }

    public static class Default
    {
        public static Func<Module, Task<object>> ModuleInstanceFactory { get; set; } = CreateModuleInstance;
    }
}