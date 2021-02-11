using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI.Helpers;

namespace LiveNewsFeed.UI.UWP.Views
{
    public abstract class BasePage : Page
    {
        protected readonly ThemeListener ThemeListener;

        protected BasePage()
        {
            ThemeListener = new ThemeListener();
            ThemeListener.ThemeChanged += ThemeListener_OnThemeChanged;
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
