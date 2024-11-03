namespace XAF.Core.MVVM;
public abstract class XafViewModel : NotifyPropertyChanged, IXafViewModel
{
    private readonly SemaphoreSlim _waitForClose = new SemaphoreSlim(0);
    public virtual void Prepare() { }

    public virtual Task WhenSelected()
    {
        return Task.CompletedTask;
    }

    public virtual Task UnloadAsync()
    {
        _waitForClose.Release();
        return Task.CompletedTask;
    }

    public virtual int CompareTo(IXafViewModel? other)
    {
        return 0;
    }

    public virtual Task WaitForViewClose()
    {
        return _waitForClose.WaitAsync();
    }

    public virtual Task LoadAsync()
    {
        return Task.CompletedTask;
    }

    public virtual Task WhenUnselected()
    {
        return Task.CompletedTask;
    }
}

public abstract class XafViewModel<TParameter> : XafViewModel, IXafViewModel<TParameter>
{
    public abstract void Prepare(TParameter parameter);
}
