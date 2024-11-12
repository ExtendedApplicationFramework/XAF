using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XAF.Core.MVVM;
public class BindableTask : NotifyPropertyChanged
{
    private readonly Action<Exception>? _onException;


    internal BindableTask(Task task, Action<Exception>? onException)
    {
        Task = task;
        _onException = onException;
        CompletedTask = MonitorTaskAsync(task);
    }

    protected virtual async Task MonitorTaskAsync(Task task)
    {
        try
        {
            await Task.Yield();
            await task;
        }
        catch (Exception ex)
        {
            OnException(ex);
        }
        finally
        {
            NotifyPropertiesChanged(task);
        }
    }

    protected virtual void OnException(Exception exception)
    {
        _onException?.Invoke(exception);
    }

    protected virtual void NotifyPropertiesChanged(Task task)
    {
        if (task.IsCanceled)
        {
            OnPropertyChanged(nameof(IsCanceled));
        }
        else if (task.IsFaulted)
        {
            OnPropertyChanged(nameof(Exception));
            OnPropertyChanged(nameof(InnerException));
            OnPropertyChanged(nameof(ErrorMessage));
            OnPropertyChanged(nameof(IsFaulted));
        }
        else 
        {
            OnPropertyChanged(nameof(IsSuccessfullyCompleted));
        }

        OnPropertyChanged(nameof(IsCompleted));
        OnPropertyChanged(nameof(IsRunning));
    }

    public Task Task { get; }

    public Task CompletedTask { get; }

    public bool IsCompleted => Task.IsCompleted;

    public bool IsRunning => !Task.IsCompleted;

    public bool IsSuccessfullyCompleted => Task.IsCompletedSuccessfully;

    public bool IsFaulted => Task.IsFaulted;

    public bool IsCanceled => Task.IsCanceled;

    public AggregateException? Exception => Task.Exception;

    public Exception? InnerException => Exception?.InnerException;

    public string? ErrorMessage => InnerException?.Message;


    public static BindableTask Create(Task task, Action<Exception>? onException = null)
        => new(task, onException);

    public static BindableTask Create(Func<Task> asyncAction, Action<Exception>? onException = null)
        => Create(asyncAction(), onException);

    public static BindableTask<TResult> Create<TResult>(
        Task<TResult> task,
        TResult? defaultResult = default,
        Action<Exception>? onException = null)
        => new(task, defaultResult, onException);

    public static BindableTask<TResult> Create<TResult>(
        Func<Task<TResult>> asyncAction,
        TResult? defaultResult = default,
        Action<Exception>? onException = null)
        => Create(asyncAction(), defaultResult, onException);

}

public class BindableTask<TResult> : BindableTask
{
    private readonly TResult? _defaultResult;

    internal BindableTask(Task<TResult> task, TResult? defaultResult, Action<Exception>? onException)
        : base(task, onException)
    {
        Task = task;
        _defaultResult = defaultResult;
    }

    public new Task<TResult> Task { get; }

    public TResult? Result => Task.IsCompletedSuccessfully ? Task.Result : _defaultResult;
}

