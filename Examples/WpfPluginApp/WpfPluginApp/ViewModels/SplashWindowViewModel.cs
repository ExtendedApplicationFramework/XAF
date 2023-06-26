﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfPluginApp.Views;
using XAF.UI;
using XAF.UI.Abstraction;

namespace WpfPluginApp.ViewModels;
public class SplashWindowViewModel : ViewModelBase, ISplashWindowViewModel
{
    public Type WindowType => typeof(SplashWindow);

    public Window? SplashWindow { get; set; }

    public async Task AfterModuleInitializationAsync()
    {
        await Task.Delay(3000);
    }

    public Task OnAppStartAsync()
    {
        return Task.CompletedTask;
    }
}
