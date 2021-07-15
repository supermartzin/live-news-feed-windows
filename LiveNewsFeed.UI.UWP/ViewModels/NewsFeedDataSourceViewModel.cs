using System;

using LiveNewsFeed.DataSource.Common;

using LiveNewsFeed.UI.UWP.Managers;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public class NewsFeedDataSourceViewModel : ViewModelBase
    {
        private readonly ISettingsManager _settingsManager;

        private readonly NewsFeedDataSource _originalDataSource;

        public string Name => _originalDataSource.Name;

        public NewsFeedLogoViewModel Logo { get; }

        private bool _isEnabled;

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                var set = SetProperty(ref _isEnabled, value);

                _originalDataSource.IsEnabled = value;

                _settingsManager.NewsFeedDisplaySettings.SetNewsFeedDataSourceState(Name, value);

                if (set)
                    IsEnabledChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler IsEnabledChanged;

        public NewsFeedDataSourceViewModel(NewsFeedDataSource originalDataSource,
                                           ISettingsManager settingsManager)
        {
            _originalDataSource = originalDataSource ?? throw new ArgumentNullException(nameof(originalDataSource));
            _settingsManager = settingsManager ?? throw new ArgumentNullException(nameof(settingsManager));

            _isEnabled = originalDataSource.IsEnabled;
            Logo = new NewsFeedLogoViewModel(originalDataSource.Logo.LightThemeUrl,
                                             originalDataSource.Logo.DarkThemeUrl);
        }
    }
}