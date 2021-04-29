using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

using LiveNewsFeed.UI.UWP.Common;
using LiveNewsFeed.UI.UWP.Managers;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public abstract class ViewModelBase : ObservableObject
    {
        protected ViewModelBase()
        {
            RegisterThemeChangingEventHandlers();
        }
        
        protected virtual async Task InvokeOnUiAsync(Action action) => await Helpers.InvokeOnUiAsync(action);

        protected virtual void InvokeOnUi(Action action) => Helpers.InvokeOnUiAsync(action);

        protected virtual string GetLocalizedString(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var resourceLoader = ResourceLoader.GetForViewIndependentUse();

            return resourceLoader.GetString(key);
        }

        protected virtual void OnSystemThemeChanged(ApplicationTheme theme)
        {
        }

        protected virtual void OnApplicationThemeChanged(Theme theme)
        {
        }

        private void RegisterThemeChangingEventHandlers()
        {
            IThemeManager themeManager = ServiceLocator.Container.GetRequiredService<IThemeManager>();

            themeManager.ApplicationThemeChanged += async (_, _) => await InvokeOnUiAsync(() => OnApplicationThemeChanged(themeManager.CurrentTheme));
            themeManager.SystemThemeChanged += async (_, _) => await InvokeOnUiAsync(() => OnSystemThemeChanged(themeManager.CurrentSystemTheme));
        }
    }
}