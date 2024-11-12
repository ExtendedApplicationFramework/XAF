using DynamicData;
using System.Windows;
using System.Windows.Controls;
using XAF.Core.MVVM;

namespace XAF.WPF.UI.ViewAdapters;
internal class ContentControlAdapter : ViewAdapter<ContentControl>
{
    public override IDisposable Attach(ContentControl container, IViewModelPresenter presenter)
    {
        if (container.Content is IXafViewModel contentViewModel)
        {
            presenter.Add(contentViewModel, CancellationToken.None);
        }
        else if (container.Content is FrameworkElement element && element.DataContext is IXafViewModel dataContentVm)
        {
            presenter.Views.Add(dataContentVm, element);
        }

        return presenter.Views
            .OnItemAdded(i => ItemAdded(presenter, i.view, container))
            .Subscribe();
    }

    private void ItemAdded(IViewModelPresenter presenter, FrameworkElement view, ContentControl control)
    {
        var current = presenter.Views.Keys.LastOrDefault();
        if (current is not null)
        {
            presenter.Remove(current, CancellationToken.None);
        }

        control.Content = view;
    }
}
