using XAF.Core.MVVM;
using XAF.Core.UI;

namespace XAF.WPF.UI.Internal;
internal class DefaultWindowService : IWindowService
{
    private readonly IViewLocator _viewLocator;

    public DefaultWindowService(IViewLocator viewLocator)
    {
        _viewLocator = viewLocator;
    }

    public IObservable<DialogParameters> DialogClosed()
    {
        throw new NotImplementedException();
    }

    public IObservable<DialogParameters> DialogOpened()
    {
        throw new NotImplementedException();
    }

    public IObservable<DialogParameters> OpenDialogRequested()
    {
        throw new NotImplementedException();
    }

    public Task<bool> OpenWindowAsync<TViewModel>(CancellationToken cancle) 
        where TViewModel : IXafViewModel
    {
        var view = _viewLocator.GetViewFor<TViewModel>();
    }

    public Task<bool> OpenWindowAsync<TViewModel>(TViewModel vm, CancellationToken cancle) 
        where TViewModel : IXafViewModel
    {
        throw new NotImplementedException();
    }

    public Task<bool> OpenWindowAsync<TViewModel, TParameter>(TViewModel vm, TParameter parameter, CancellationToken cancle) 
        where TViewModel : IXafViewModel<TParameter>
    {
        throw new NotImplementedException();
    }

    public Task<bool> OpenWindowAsync<TViewModel, TParameter>(TParameter parameter, CancellationToken cancle)
         where TViewModel : class, IXafViewModel<TParameter>
    {
        throw new NotImplementedException();
    }

    public Task<bool> CloseWindowAsync<TViewModel>(TViewModel vm, CancellationToken cancle1) where TViewModel : IXafViewModel
    {
        throw new NotImplementedException();
    }

    public Task<TResult?> ShowDialogAsync<TResult, TViewModel>()
        where TViewModel : class, IXafDialogViewModel<TResult>
    {
        throw new NotImplementedException();
    }

    public Task<TResult?> ShowDialogAsync<TResult, TViewModel>(TViewModel vm)
        where TViewModel : class, IXafDialogViewModel<TResult>
    {
        throw new NotImplementedException();
    }

    public Task<TResult?> ShowDialogAsync<TResult, TViewModel, TParameter>(TParameter parameter)
       where TViewModel : class, IXafDialogViewModel<TResult, TParameter>
    {
        throw new NotImplementedException();
    }

    public Task<TResult?> ShowDialogAsync<TResult, TViewModel, TParameter>(TViewModel vm, TParameter parameter)
        where TViewModel : class, IXafDialogViewModel<TResult, TParameter>
    {
        throw new NotImplementedException();
    }
}
