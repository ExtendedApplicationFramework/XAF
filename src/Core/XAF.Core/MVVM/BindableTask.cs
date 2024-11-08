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

    private async Task MonitorTaskAsync(Task task)
    {
        try
        {
            await Task.Yield();
            await task;
        }
        catch (Exception ex)
        {

            _onException?.Invoke(ex);
        }
        finally
        {
            NotifyPropertiesChanged(task);
        }
    }

    private void NotifyPropertiesChanged(Task task)
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
}

public class BindableTask<TResult> : NotifyPropertyChanged
{
    private readonly Action<Exception>? _onException;
    private readonly TResult? _defaultResult;

    internal BindableTask(Task<TResult> task, TResult? defaultResult, Action<Exception>? onException)
    {
        Task = task;
        _defaultResult = defaultResult;
        _onException = onException;
        CompletedTask = MonitorTaskAsync(task);
    }

    private async Task MonitorTaskAsync(Task task)
    {
        try
        {
            await System.Threading.Tasks.Task.Yield();
            await task;
        }
        catch (Exception ex)
        {

            _onException?.Invoke(ex);
        }
        finally
        {
            NotifyPropertiesChanged(task);
        }
    }

    private void NotifyPropertiesChanged(Task task)
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

    public Task<TResult> Task { get; }

    public Task CompletedTask { get; }

    public TResult? Result => Task.IsCompletedSuccessfully ? Task.Result : _defaultResult;

    public bool IsCompleted => Task.IsCompleted;

    public bool IsRunning => !Task.IsCompleted;

    public bool IsSuccessfullyCompleted => Task.IsCompletedSuccessfully;

    public bool IsFaulted => Task.IsFaulted;

    public bool IsCanceled => Task.IsCanceled;

    public AggregateException? Exception => Task.Exception;

    public Exception? InnerException => Exception?.InnerException;

    public string? ErrorMessage => InnerException?.Message;
}

