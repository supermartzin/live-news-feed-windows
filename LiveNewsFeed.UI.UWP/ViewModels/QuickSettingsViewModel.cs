using System;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;

using LiveNewsFeed.UI.UWP.Managers;
using LiveNewsFeed.UI.UWP.Managers.Settings;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public class QuickSettingsViewModel : ViewModelBase
    {
        private readonly ISettingsManager _settingsManager;

        private bool _notificationsTurnedOn;
        public bool NotificationsTurnedOn
        {
            get => _notificationsTurnedOn;
            set
            {
                var changed = SetProperty(ref _notificationsTurnedOn, value);
                if (changed)
                    _settingsManager.NotificationSettings.NotificationsAllowed = value;
            }
        }

        private bool _showOnlyImportantPosts;
        public bool ShowOnlyImportantPosts
        {
            get => _showOnlyImportantPosts;
            set
            {
                var changed = SetProperty(ref _showOnlyImportantPosts, value);
                if (changed)
                    _settingsManager.NewsFeedDisplaySettings.ShowOnlyImportantPosts = value;
            }
        }

        public ICommand TurnOnNotificationsCommand { get; private set; }

        public ICommand ShowOnlyImportantPostsCommand { get; private set; }

        public QuickSettingsViewModel(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager ?? throw new ArgumentNullException(nameof(settingsManager));
            if (!_settingsManager.AreSettingsLoaded)
            {
                _settingsManager.SettingsLoaded += (_, _) => LoadSettings();
            }
            else
            {
                LoadSettings();
            }
            
            InitializeCommands();
        }


        private void LoadSettings()
        {
            _notificationsTurnedOn = _settingsManager.NotificationSettings.NotificationsAllowed;
            _showOnlyImportantPosts = _settingsManager.NewsFeedDisplaySettings.ShowOnlyImportantPosts;

            _settingsManager.AutomaticUpdateSettings.SettingChanged += AutomaticUpdateSettings_OnChanged;
            _settingsManager.NewsFeedDisplaySettings.SettingChanged += NewsFeedDisplaySettings_OnChanged;
            _settingsManager.NotificationSettings.SettingChanged += NotificationSettings_OnSettingChanged;
        }

        private void InitializeCommands()
        {
            TurnOnNotificationsCommand = new RelayCommand(() => NotificationsTurnedOn = !NotificationsTurnedOn);
            ShowOnlyImportantPostsCommand = new RelayCommand(() => ShowOnlyImportantPosts = !ShowOnlyImportantPosts);
        }

        private void AutomaticUpdateSettings_OnChanged(object sender, SettingChangedEventArgs e)
        {
        }

        private void NewsFeedDisplaySettings_OnChanged(object sender, SettingChangedEventArgs e)
        {
        }

        private void NotificationSettings_OnSettingChanged(object sender, SettingChangedEventArgs e)
        {
        }
    }
}