using System;
using Windows.UI.Xaml;

namespace LiveNewsFeed.UI.UWP.Managers
{
    public interface IThemeManager
    {
        Theme CurrentTheme { get; }

        ApplicationTheme CurrentApplicationTheme { get; }

        ApplicationTheme CurrentSystemTheme { get; }

        event EventHandler<ThemeChangedEventArgs> SystemThemeChanged;
        event EventHandler<ThemeChangedEventArgs> ApplicationThemeChanged;

        void SetTheme(Theme theme);
    }
}