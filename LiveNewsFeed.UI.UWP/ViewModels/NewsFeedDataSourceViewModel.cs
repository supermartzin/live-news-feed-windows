using System;

using LiveNewsFeed.DataSource.Common;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public class NewsFeedDataSourceViewModel : ViewModelBase
    {
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

                // TODO set in settings

                if (set)
                    IsEnabledChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler IsEnabledChanged;

        public NewsFeedDataSourceViewModel(NewsFeedDataSource originalDataSource)
        {
            _originalDataSource = originalDataSource ?? throw new ArgumentNullException(nameof(originalDataSource));

            _isEnabled = originalDataSource.IsEnabled;
            Logo = new NewsFeedLogoViewModel(originalDataSource.Logo.LightThemeUrl,
                                             originalDataSource.Logo.DarkThemeUrl);
        }
    }
}