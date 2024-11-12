using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using XAF.Core;
using XAF.Core.MVVM;

namespace XAF.WPF.UI.Internal;
internal class ViewModelLocator : IViewModelLocator
{
    private readonly IServiceProvider _serviceProvider;

    public ViewModelLocator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IXafViewModel CreateVm(Type vmType, FrameworkElement view)
    {
        var vm = (IXafViewModel)ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, vmType);
        view.DataContext = vm;
        RunViewModelLifeCycle(view, vm);

        return vm;
    }

    public TViewModel CreateVm<TViewModel>(FrameworkElement view) where TViewModel : IXafViewModel
    {
        var vm = ActivatorUtilities.GetServiceOrCreateInstance<TViewModel>(_serviceProvider);
        view.DataContext = vm;

        RunViewModelLifeCycle(view, vm);

        return vm;
    }

    public TViewModel Prepare<TViewModel>(TViewModel vm, FrameworkElement view) where TViewModel : IXafViewModel
    {
        view.DataContext = vm;
        
        RunViewModelLifeCycle(view, vm);
        
        return vm;
    }

    public TViewModel Load<TViewModel, TParameter>(TParameter parameter, FrameworkElement view) where TViewModel : IXafViewModel<TParameter>
    {
        var vm = ActivatorUtilities.GetServiceOrCreateInstance<TViewModel>(_serviceProvider);
        view.DataContext = vm;

        RunViewModelLifeCycle(view, vm, parameter);

        return vm;
    }

    public TViewModel Prepare<TViewModel, TParameter>(TViewModel vm, TParameter parameter, FrameworkElement view) where TViewModel : IXafViewModel<TParameter>
    {
        view.DataContext = vm;
        RunViewModelLifeCycle(view, vm, parameter);

        return vm;
    }


    protected static void RunViewModelLifeCycle(FrameworkElement view, IXafViewModel vm)
    {
        vm.Initialize();

        view.IsVisibleChanged += (s, e) =>
        {
            if ((bool)e.NewValue)
            {
                vm.LoadTask = BindableTask.Create(vm.LoadAsync);
            }
            else
            {
                vm.UnloadTask = BindableTask.Create(vm.UnloadAsync);
            }
        };
    }

    protected static void RunViewModelLifeCycle<TParameter>(FrameworkElement view, IXafViewModel<TParameter> vm, TParameter parameter)
    {
        vm.Initialize();

        vm.Initialize(parameter);

        view.IsVisibleChanged += (s, e) =>
        {
            if ((bool)e.NewValue)
            {
                vm.LoadTask = BindableTask.Create(vm.LoadAsync);
            }
            else
            {
                vm.UnloadTask = BindableTask.Create(vm.UnloadAsync);
            }
        };
    }
}
