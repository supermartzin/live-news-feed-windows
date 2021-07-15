using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.Xaml;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

using LiveNewsFeed.UI.UWP.Common;
using LiveNewsFeed.UI.UWP.Managers;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public abstract class ViewModelBase : ObservableObject
    {
        protected IThemeManager ThemeManager { get; }

        protected ViewModelBase()
        {
            ThemeManager = ServiceLocator.Container.GetRequiredService<IThemeManager>();

            RegisterThemeChangingEventHandlers();
        }
        
        protected virtual async Task InvokeOnUiAsync(Action action) => await Helpers.InvokeOnUiAsync(action);
        
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

        protected virtual void OnSystemAccentColorChanged(Color accentColor)
        {
        }

        private void RegisterThemeChangingEventHandlers()
        {
            ThemeManager.ApplicationThemeChanged += async (_, _) => await InvokeOnUiAsync(() => OnApplicationThemeChanged(ThemeManager.CurrentTheme));
            ThemeManager.SystemThemeChanged += async (_, _) => await InvokeOnUiAsync(() => OnSystemThemeChanged(ThemeManager.CurrentSystemTheme));
            ThemeManager.SystemAccentColorChanged += async (_, _) => await InvokeOnUiAsync(() => OnSystemAccentColorChanged(ThemeManager.CurrentAccentColor));
        }
    }
}