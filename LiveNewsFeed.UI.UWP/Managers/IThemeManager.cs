using System;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace LiveNewsFeed.UI.UWP.Managers
{
    public interface IThemeManager
    {
        Theme CurrentTheme { get; }

        ApplicationTheme CurrentApplicationTheme { get; }

        ApplicationTheme CurrentSystemTheme { get; }

        Color CurrentAccentColor { get; }

        event EventHandler<ThemeChangedEventArgs> SystemThemeChanged;
        event EventHandler<ThemeChangedEventArgs> ApplicationThemeChanged;
        event EventHandler<ColorChangedEventArgs> SystemAccentColorChanged; 

        void SetTheme(Theme theme);

        Color GetSystemColor(UIColorType colorType);
    }
}