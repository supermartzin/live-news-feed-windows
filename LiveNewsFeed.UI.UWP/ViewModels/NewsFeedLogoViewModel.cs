using System;
using Windows.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;

using LiveNewsFeed.UI.UWP.Common;
using LiveNewsFeed.UI.UWP.Managers;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public class NewsFeedLogoViewModel : ViewModelBase
    {
        private readonly IThemeManager _themeManager;
        private readonly Uri _lightThemeFilePath;
        private readonly Uri _darkThemeFilePath;

        public Uri FilePath => _themeManager.CurrentApplicationTheme is ApplicationTheme.Light
                                    ? _lightThemeFilePath
                                    : _darkThemeFilePath;

        public NewsFeedLogoViewModel(Uri lightThemeFilePath, Uri darkThemeFilePath)
        {
            _themeManager = ServiceLocator.Container.GetRequiredService<IThemeManager>();

            _lightThemeFilePath = lightThemeFilePath ?? throw new ArgumentNullException(nameof(lightThemeFilePath));
            _darkThemeFilePath = darkThemeFilePath ?? throw new ArgumentNullException(nameof(darkThemeFilePath));
        }

        protected override void OnSystemThemeChanged(ApplicationTheme theme) => OnPropertyChanged(nameof(FilePath));

        protected override void OnApplicationThemeChanged(Theme theme) => OnPropertyChanged(nameof(FilePath));
    }
}