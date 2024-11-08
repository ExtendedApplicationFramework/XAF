using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XAF.Core.MVVM;

/// <summary>
/// A base implementation for the <see cref="INotifyPropertyChanged"/> interface
/// </summary>
public abstract class NotifyPropertyChanged : INotifyPropertyChanged
{

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;


    /// <summary>
    /// A method to set a property an rise the <see cref="PropertyChangedEventHandler"/>
    /// </summary>
    /// <typeparam name="T">the type of the property</typeparam>
    /// <param name="backingField">a reference to the backing field</param>
    /// <param name="value">the new value of the property</param>
    /// <param name="propertyName">the name of the property</param>
    /// <returns>a value that indicates whether the value was changed or not</returns>
    protected virtual bool SetProperty<T>(ref T backingField, T value, [CallerMemberName] string? propertyName = null)
        => SetProperty(ref backingField, value, EqualityComparer<T>.Default, propertyName);

    /// <summary>
    /// A method to set a property an rise the <see cref="PropertyChangedEventHandler"/>
    /// </summary>
    /// <typeparam name="T">the type of the property</typeparam>
    /// <param name="backingField">a reference to the backing field</param>
    /// <param name="value">the new value of the property</param>
    /// <param name="comparer">the <see cref="IEqualityComparer{T}"/> to compare the current value with the new value to</param>
    /// <param name="propertyName">the name of the property</param>
    /// <returns>a value indicating whether the value has been changed or not</returns>
    protected virtual bool SetProperty<T>(ref T backingField, T value, IEqualityComparer<T> comparer, [CallerMemberName] string? propertyName = null)
    {
        ArgumentNullException.ThrowIfNull(comparer);
        ArgumentNullException.ThrowIfNull(propertyName);

        if (comparer.Equals(backingField, value))
        {
            return false;
        }
        
        if(!OnPropertyChanging(propertyName, backingField, value))
        {
            return false;
        }

        backingField = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    /// <summary>
    /// A method to set a property an rise the <see cref="PropertyChangedEventHandler"/>
    /// </summary>
    /// <typeparam name="T">the type of the property</typeparam>
    /// <param name="backingField">a reference to the backing field</param>
    /// <param name="value">the new value of the property</param>
    /// <param name="callback">a method that gets called if the value has been updated</param>
    /// <param name="propertyName">the name of the property</param>
    /// <returns>a value indicating whether the value has been changed or not</returns>
    protected virtual bool SetProperty<T>(ref T backingField, T value, Action<T> callback, [CallerMemberName] string? propertyName = null)
    {
        if (SetProperty(ref backingField, value, propertyName))
        {
            callback(value);
            return true;
        }

        return false;
    }

    /// <summary>
    /// A method to set a property an rise the <see cref="PropertyChangedEventHandler"/>
    /// </summary>
    /// <typeparam name="T">the type of the property</typeparam>
    /// <param name="backingField">a reference to the backing field</param>
    /// <param name="value">the new value of the property</param>
    /// <param name="callback">a method that gets called if the value has been updated</param>
    /// <param name="comparer">the <see cref="IEqualityComparer{T}"/> to compare the current value with the new value to</param>
    /// <param name="propertyName">the name of the property</param>
    /// <returns>a value indicating whether the value has been changed or not</returns>
    protected virtual bool SetProperty<T>(
        ref T backingField,
        T value,
        Action<T> callback,
        IEqualityComparer<T> comparer,
        [CallerMemberName] string? propertyName = null)
    {
        if (SetProperty(ref backingField, value, comparer, propertyName))
        {
            callback(value);
            return true;
        }

        return false;
    }

    /// <summary>
    /// A Method that is called when a property is changing
    /// </summary>
    /// <param name="propertyName">the name of the updated property</param>
    /// <param name="oldValue">the old value</param>
    /// <param name="newValue">the new value</param>
    protected virtual bool OnPropertyChanging(string propertyName, object? oldValue, object? newValue)
    {
        return true;
    }

    /// <summary>
    /// A Method that is called when a property has been changed
    /// </summary>
    /// <param name="propertyName">the name of the updated property</param>
    /// <param name="newValue">the new value</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
