namespace XAF.Modularity.Catalogs;
public interface IModuleCatalog
{
    Task<IModuleDescription[]> GetModulesAsync(Func<Type, bool> typeMatch);
}
