using XAF.Core.UI;
using XAF.Modularity;
using XAF.WPF.UI;

namespace XAF.WPF.Modularity.Internal;
internal class WpfModuleHandler : ModuleHandler<IWpfModule>
{
    private readonly IViewLocator _viewLocator;

    public WpfModuleHandler(IViewLocator viewLocator)
    {
        _viewLocator = viewLocator;
    }

    protected override Task LoadAsync(IWpfModule module)
    {
        return _viewLocator.DiscoverViewsAsync(module.GetType().Assembly);
    }
}
