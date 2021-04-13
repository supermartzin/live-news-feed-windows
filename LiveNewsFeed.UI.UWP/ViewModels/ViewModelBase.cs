﻿using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.System;
using Windows.UI.Xaml;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI.Helpers;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public abstract class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase
    {
        protected readonly ThemeListener ThemeListener;

        protected ViewModelBase()
        {
            ThemeListener = new ThemeListener();
            ThemeListener.ThemeChanged += ThemeListener_OnThemeChanged;
        }
        
        protected virtual async Task InvokeOnUiAsync(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            
            await DispatcherQueue.GetForCurrentThread().EnqueueAsync(action);
        }

        protected virtual void InvokeOnUi(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            DispatcherQueue.GetForCurrentThread().EnqueueAsync(action);
        }

        protected virtual string GetLocalizedString(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var resourceLoader = ResourceLoader.GetForViewIndependentUse();

            return resourceLoader.GetString(key);
        }

        protected virtual void OnApplicationThemeChanged(ApplicationTheme theme)
        {
        }


        private void ThemeListener_OnThemeChanged(ThemeListener sender)
        {
            OnApplicationThemeChanged(sender.CurrentTheme);
        }
    }
}