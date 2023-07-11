﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using XAF.UI.Abstraction;
using XAF.UI.WPF.Attributes;
using XAF.UI.WPF.ViewComposition;

namespace XAF.UI.WPF.Internal;
internal class ViewCollection : IViewCollection
{
    private readonly List<ViewDescriptor> _viewDescriptors = new();
    private readonly Dictionary<object, HashSet<ViewDescriptor>> _lookupDictionary = new();
    private readonly Dictionary<Type, ViewDescriptor> _vmDictionary = new();
    private readonly IServiceCollection _services;

    public IEnumerable<Type> Keys => _vmDictionary.Keys;
    public IEnumerable<ViewDescriptor> Values => _vmDictionary.Values;

    public int Count => _viewDescriptors.Count;
    public bool IsReadOnly => false;

    public ViewDescriptor this[Type key] => _vmDictionary[key];

    public IEnumerable<ViewDescriptor> this[object key] => _lookupDictionary.TryGetValue(key, out var descriptors) ? descriptors : Enumerable.Empty<ViewDescriptor>();

    public ViewCollection(IServiceCollection services)
    {
        _services = services;
    }

    public ViewDescriptor AddView(Type viewType)
    {
        var descriptor = ViewDescriptor.Create(viewType);
        Add(descriptor);

        return descriptor;
    }

    public IEnumerator<ViewDescriptor> GetEnumerator()
    {
        return _viewDescriptors.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _viewDescriptors.GetEnumerator();
    }

    private void Add(ViewDescriptor descriptor)
    {
        _viewDescriptors.Add(descriptor);
        _vmDictionary.Add(descriptor.ViewModelType, descriptor);

        if (descriptor.ViewModelType.IsAssignableFrom(typeof(INavigableViewModel)))
        {
            AddLookupKey(descriptor, ViewDescriptorKeys.IsNavigableKey);
        }

        if (descriptor.ViewType.GetCustomAttribute<ShellAttribute>() != null)
        {
            AddLookupKey(descriptor, ViewDescriptorKeys.IsShellKey);
        }

        if (descriptor.ViewType.GetCustomAttribute<SplashScreenAttribute>() != null)
        {
            AddLookupKey(descriptor, ViewDescriptorKeys.IsSplashScreenKey);
        }
        var dialogWindowAttribtue = descriptor.ViewType.GetCustomAttribute<DialogWindowAttribute>();
        if ( dialogWindowAttribtue != null)
        {
            AddLookupKey(descriptor, ViewDescriptorKeys.HasSpecialDialogWindowKey);
            descriptor.Properties[ViewDescriptorKeys.HasSpecialDialogWindowKey] = dialogWindowAttribtue.WindowType;
            _services.AddTransient(dialogWindowAttribtue.WindowType);
        }

        var navFrameAttributes = descriptor.ViewType.GetCustomAttributes<ContainsViewContainerAttribute>();
        foreach (var navFrameAttribute in navFrameAttributes)
        {
            AddLookupKey(descriptor, ViewDescriptorKeys.ContainsViewContainerKey);

            if (!descriptor.Properties.TryGetValue(ViewDescriptorKeys.ContainsViewContainerKey, out var navKeys))
            {
                navKeys = new HashSet<object>();
                descriptor.Properties[ViewDescriptorKeys.ContainsViewContainerKey] = navKeys;
            }

            var navKeysHasSet = (HashSet<object>)navKeys;
            navKeysHasSet.Add(navFrameAttribute.Key);
        }

        _services.AddTransient(descriptor.ViewType);
        _services.AddTransient(descriptor.ViewModelType);
    }

    public ViewDescriptor GetDescriptorForViewModel(Type viewModelType)
    {
        return _vmDictionary[viewModelType];
    }

    public IEnumerable<ViewDescriptor> GetDescriptorsByKey(object key)
    {
        if (_lookupDictionary.TryGetValue(key, out var viewDescriptors))
        {
            return viewDescriptors;
        }

        return Enumerable.Empty<ViewDescriptor>();
    }

    public bool TryGetDescriptorForViewModel(Type viewModelType, [MaybeNullWhen(false)] out ViewDescriptor descriptor)
    {
        return _vmDictionary.TryGetValue(viewModelType, out descriptor);
    }

    public void AddLookupKey(ViewDescriptor viewDescriptor, object key)
    {
        if (!_lookupDictionary.TryGetValue(key, out var viewDescriptors))
        {
            viewDescriptors = new();
            _lookupDictionary[key] = viewDescriptors;
        }
        viewDescriptors.Add(viewDescriptor);
        viewDescriptor.InternalLookupKeys.Add(key);
    }
}
