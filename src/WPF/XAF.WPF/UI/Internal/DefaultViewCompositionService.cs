using Microsoft.Extensions.DependencyInjection;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using XAF.Core.MVVM;
using XAF.Core.UI;

namespace XAF.WPF.UI.Internal;
internal sealed class DefaultViewCompositionService : IViewCompositionService
{
    private readonly ViewModelPresenterLocator _presenterRepository;
    private readonly IServiceProvider _serviceProvider;
    private readonly Subject<ViewManipulation> _viewManipulationRequestedSubject;
    private readonly Subject<ViewManipulation> _viewManipulationCompletedSubject;

    public DefaultViewCompositionService(ViewModelPresenterLocator presenterRepository, IServiceProvider serviceProvider)
    {
        _viewManipulationCompletedSubject = new();
        _viewManipulationRequestedSubject = new();
        _presenterRepository = presenterRepository;
        _serviceProvider = serviceProvider;
    }

    public Task<bool> AddViewAsync<TViewModel>(object presenterKey, CancellationToken cancellation) where TViewModel : class, IXafViewModel
    {
        var vm = _serviceProvider.GetRequiredService<TViewModel>();
        var manipulation = new ViewManipulation(ViewManipulationType.Add, vm, presenterKey);

        _viewManipulationRequestedSubject.OnNext(manipulation);

        if (manipulation.Cancle)
        {
            _viewManipulationCompletedSubject.OnNext(manipulation);
            return Task.FromResult(false);
        }

        var presenter = _presenterRepository.GetPresenter(presenterKey);

        if (cancellation.IsCancellationRequested)
        {
            manipulation.Cancle = true;
            _viewManipulationCompletedSubject.OnNext(manipulation);
            return Task.FromResult(false);
        }

        if (!presenter.Add(vm, cancellation))
        {
            return Task.FromResult(false);
        }

        _viewManipulationCompletedSubject.OnNext(manipulation);
        return Task.FromResult(true);
    }

    public async Task<bool> AddViewAsync<TViewModel, TParameter>(TParameter parameter, object presenterKey, CancellationToken cancellation) where TViewModel : class, IXafViewModel<TParameter>
    {

        var vm = _serviceProvider.GetRequiredService<TViewModel>();
        var manipulation = new ViewManipulation(ViewManipulationType.Add, vm, presenterKey, parameter);

        _viewManipulationRequestedSubject.OnNext(manipulation);

        if (manipulation.Cancle)
        {
            _viewManipulationCompletedSubject.OnNext(manipulation);
            return false;
        }

        var presenter = _presenterRepository.GetPresenter(presenterKey);

        if (cancellation.IsCancellationRequested)
        {
            manipulation.Cancle = true;
            _viewManipulationCompletedSubject.OnNext(manipulation);
            return false;
        }

        vm.Prepare(parameter);

        if (!presenter.Add(vm, cancellation))
        {
            return false;
        }

        _viewManipulationCompletedSubject.OnNext(manipulation);
        return true;
    }

    public Task<bool> AddViewAsync<TViewModel>(TViewModel vm, object presenterKey, CancellationToken cancle) where TViewModel : IXafViewModel
    {
        var manipulation = new ViewManipulation(ViewManipulationType.Add, vm, presenterKey);

        _viewManipulationRequestedSubject.OnNext(manipulation);

        if (manipulation.Cancle)
        {
            _viewManipulationCompletedSubject.OnNext(manipulation);
            return Task.FromResult(false);
        }

        var presenter = _presenterRepository.GetPresenter(presenterKey);

        if (cancle.IsCancellationRequested)
        {
            _viewManipulationCompletedSubject.OnNext(manipulation);
            return Task.FromResult(false);
        }

        if (!presenter.Add(vm, cancle))
        {
            return Task.FromResult(false);
        }

        _viewManipulationCompletedSubject.OnNext(manipulation);

        return Task.FromResult(true);
    }

    public async Task<bool> AddViewAsync<TViewModel, TParameter>(TViewModel vm, TParameter parameter, object presenterKey, CancellationToken cancle) where TViewModel : IXafViewModel<TParameter>
    {
        var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancle);

        var manipulation = new ViewManipulation(ViewManipulationType.Add, vm, presenterKey, parameter);

        _viewManipulationRequestedSubject.OnNext(manipulation);

        if (manipulation.Cancle)
        {
            _viewManipulationCompletedSubject.OnNext(manipulation);
            return false;
        }

        var presenter = _presenterRepository.GetPresenter(presenterKey);

        if (tokenSource.IsCancellationRequested)
        {
            return false;
        }

        vm.Prepare(parameter);

        if (!presenter.Add(vm, tokenSource.Token))
        {
            return false;
        }

        _viewManipulationCompletedSubject.OnNext(manipulation);
        return true;
    }

    public Task<bool> RemoveViewAsync<TViewModel>(TViewModel vm, object presenterKey, CancellationToken cancle) where TViewModel : IXafViewModel
    {
        var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancle);

        var manipulation = new ViewManipulation(ViewManipulationType.Remove, vm, presenterKey);

        _viewManipulationRequestedSubject.OnNext(manipulation);

        if (manipulation.Cancle)
        {
            _viewManipulationCompletedSubject.OnNext(manipulation);
            return Task.FromResult(true);
        }

        var presenter = _presenterRepository.GetPresenter(presenterKey);

        if (tokenSource.IsCancellationRequested)
        {
            manipulation.Cancle = true;
            _viewManipulationCompletedSubject.OnNext(manipulation);
            return Task.FromResult(false);
        }

        if (!presenter.Remove(vm, tokenSource.Token))
        {
            return Task.FromResult(false);
        }

        _viewManipulationCompletedSubject.OnNext(manipulation);
        return Task.FromResult(true);
    }

    public Task<bool> SelectViewAsync<TViewModel>(TViewModel vm, object presenterKey, CancellationToken cancellation) where TViewModel : IXafViewModel
    {
        var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellation);

        var manipulation = new ViewManipulation(ViewManipulationType.Select, vm, presenterKey);

        _viewManipulationRequestedSubject.OnNext(manipulation);

        if (manipulation.Cancle)
        {
            _viewManipulationCompletedSubject.OnNext(manipulation);
            return Task.FromResult(false);
        }

        var presenter = _presenterRepository.GetPresenter(presenterKey);

        if (tokenSource.IsCancellationRequested)
        {
            manipulation.Cancle = true;
            _viewManipulationCompletedSubject.OnNext(manipulation);
            return Task.FromResult(false);
        }

        if (!presenter.Select(vm, tokenSource.Token))
        {
            return Task.FromResult(false);
        }

        _viewManipulationCompletedSubject.OnNext(manipulation);
        return Task.FromResult(true);
    }

    public async Task<bool> SelectViewAsync<TViewModel, TParameter>(TViewModel vm, TParameter parameter, object presenterKey, CancellationToken cancellation) where TViewModel : IXafViewModel<TParameter>
    {
        var tokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellation);

        var manipulation = new ViewManipulation(ViewManipulationType.Select, vm, presenterKey, parameter);

        _viewManipulationRequestedSubject.OnNext(manipulation);

        if (manipulation.Cancle)
        {
            _viewManipulationCompletedSubject.OnNext(manipulation);
            return false;
        }

        var presenter = _presenterRepository.GetPresenter(presenterKey);

        if (tokenSource.IsCancellationRequested)
        {
            manipulation.Cancle = true;
            _viewManipulationCompletedSubject.OnNext(manipulation);
            return false;
        }

        vm.Prepare(parameter);

        if (!presenter.Select(vm, tokenSource.Token))
        {
            return false;
        }

        _viewManipulationCompletedSubject.OnNext(manipulation);
        return true;
    }

    public IObservable<ViewManipulation> ViewManipulationCompleted()
    {
        return _viewManipulationCompletedSubject;
    }

    public IObservable<ViewManipulation> ViewManipulationCompleted(object presenterKey)
    {
        return _viewManipulationCompletedSubject.Where(m => m.PresenterKey == presenterKey);
    }

    public IObservable<ViewManipulation> ViewManipulationRequested()
    {
        return _viewManipulationRequestedSubject;
    }

    public IObservable<ViewManipulation> ViewManipulationRequested(object presenterKey)
    {
        return _viewManipulationRequestedSubject.Where(m => m.PresenterKey == presenterKey);
    }
}
