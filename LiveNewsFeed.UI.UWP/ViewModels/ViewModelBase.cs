using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI.Helpers;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public abstract class ViewModelBase : ObservableObject
    {
        private readonly CoreDispatcher _dispatcher;

        protected readonly ThemeListener ThemeListener;

        protected ViewModelBase()
        {
            _dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

            ThemeListener = new ThemeListener();
            ThemeListener.ThemeChanged += ThemeListener_OnThemeChanged;
        }
        
        protected virtual async Task InvokeOnUiAsync(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            await CoreApplication.MainView
                                 .DispatcherQueue
                                 .EnqueueAsync(action);
        }

        protected virtual void InvokeOnUi(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            
            CoreApplication.MainView
                           .DispatcherQueue
                           .EnqueueAsync(action);
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