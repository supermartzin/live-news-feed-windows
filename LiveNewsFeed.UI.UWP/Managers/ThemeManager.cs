using System;
using System.ComponentModel;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Microsoft.Extensions.Logging;

using LiveNewsFeed.UI.UWP.Common;

namespace LiveNewsFeed.UI.UWP.Managers
{
    public class ThemeManager : IThemeManager
    {
        private readonly ILogger<ThemeManager>? _logger;

        private readonly UISettings _uiSettings;

        /// <summary>
        /// Theme set in Settings: System default, Light or Dark
        /// </summary>
        public Theme CurrentTheme { get; private set; }

        /// <summary>
        /// Actual theme currently set: Light or Dark
        /// </summary>
        public ApplicationTheme CurrentApplicationTheme => CurrentTheme switch
        {
            Theme.Light => ApplicationTheme.Light,
            Theme.Dark => ApplicationTheme.Dark,
            _ => CurrentSystemTheme
        };

        /// <summary>
        /// Current theme set by system: Light or Dark
        /// </summary>
        private Theme _currentSystemTheme;
        public ApplicationTheme CurrentSystemTheme => ToApplicationTheme(_currentSystemTheme);
        
        public event EventHandler<ThemeChangedEventArgs>? SystemThemeChanged;
        public event EventHandler<ThemeChangedEventArgs>? ApplicationThemeChanged;

        public ThemeManager(ILogger<ThemeManager>? logger = default)
        {
            _logger = logger;

            _uiSettings = new UISettings();
            _uiSettings.ColorValuesChanged += UiSettings_OnColorValuesChanged;

            _currentSystemTheme = DecideSystemTheme();

            CurrentTheme = (Theme) (-1);
        }

        public void SetTheme(Theme theme)
        {
            Helpers.InvokeOnUiAsync(() =>
            {
                var window = (FrameworkElement) Window.Current.Content;
                switch (theme)
                {
                    case Theme.SystemDefault:
                        window.RequestedTheme = ToElementTheme(_currentSystemTheme);
                        break;

                    case Theme.Light:
                        window.RequestedTheme = ElementTheme.Light;
                        break;

                    case Theme.Dark:
                        window.RequestedTheme = ElementTheme.Dark;
                        break;
                }
            }).Wait(50);

            if (CurrentTheme == theme)
                return;

            CurrentTheme = theme;

            _logger?.LogInformation($"Application theme set to '{theme}' | System theme is '{_currentSystemTheme}'.");

            ApplicationThemeChanged?.Invoke(this, new ThemeChangedEventArgs(CurrentTheme));
        }


        private void UiSettings_OnColorValuesChanged(UISettings settings, object args)
        {
            var newTheme = DecideSystemTheme();
            if (newTheme == _currentSystemTheme)
                return;

            _currentSystemTheme = newTheme;

            _logger?.LogInformation($"System theme changed to '{CurrentSystemTheme}'.");

            // reload application theme if needed
            if (CurrentTheme == Theme.SystemDefault)
                SetTheme(CurrentTheme);

            // invoked on system color/theme change
            SystemThemeChanged?.Invoke(this, new ThemeChangedEventArgs(DecideSystemTheme()));
        }

        private Theme DecideSystemTheme()
        {
            var themeColor = _uiSettings.GetColorValue(UIColorType.Background);
            
            if (themeColor == Colors.Black)
                return Theme.Dark;
            if (themeColor == Colors.White)
                return Theme.Light;

            return Theme.Light;
        }
        
        private static ElementTheme ToElementTheme(Theme theme) => theme switch
        {
            Theme.Light => ElementTheme.Light,
            Theme.Dark => ElementTheme.Dark,
            _ => ElementTheme.Default
        };

        private static ApplicationTheme ToApplicationTheme(Theme systemTheme) => systemTheme switch
        {
            Theme.Light => ApplicationTheme.Light,
            Theme.Dark => ApplicationTheme.Dark,
            _ => throw new InvalidEnumArgumentException(nameof(systemTheme), (int) systemTheme, typeof(Theme))
        };
    }
}