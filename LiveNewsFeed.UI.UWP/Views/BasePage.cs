using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

using LiveNewsFeed.UI.UWP.Common;
using LiveNewsFeed.UI.UWP.Managers;

namespace LiveNewsFeed.UI.UWP.Views
{
    public abstract class BasePage : Page
    {
        protected BasePage()
        {
            RegisterThemeChangingEventHandlers();
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

            themeManager.ApplicationThemeChanged += async (_, _) => await Helpers.InvokeOnUiAsync(() => OnApplicationThemeChanged(themeManager.CurrentTheme));
            themeManager.SystemThemeChanged += async (_, _) => await Helpers.InvokeOnUiAsync(() => OnSystemThemeChanged(themeManager.CurrentSystemTheme));
        }
    }
}
