using XAF.Core.UI;

namespace XAF.WPF.Modularity;
public interface IWpfModule
{
    Task StartAsync(IViewCompositionService viewCompositionService);
}
