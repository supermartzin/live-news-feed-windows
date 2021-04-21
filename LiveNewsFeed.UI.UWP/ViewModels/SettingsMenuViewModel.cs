using System;

using LiveNewsFeed.UI.UWP.Managers;
using LiveNewsFeed.UI.UWP.Managers.Settings;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public class SettingsMenuViewModel : ViewModelBase
    {
        private readonly ISettingsManager _settingsManager;

        private bool _automaticUpdates;
        public bool AutomaticUpdates
        {
            get => _automaticUpdates;
            set
            {
                var changed = SetProperty(ref _automaticUpdates, value);
                if (changed)
                    _settingsManager.AutomaticUpdateSettings.AutomaticUpdateAllowed = value;
            }
        }

        private double _automaticUpdatesInterval;
        public double AutomaticUpdatesInterval
        {
            get => _automaticUpdatesInterval;
            set
            {
                var changed = SetProperty(ref _automaticUpdatesInterval, value);
                if (changed)
                    _settingsManager.AutomaticUpdateSettings.UpdateInterval = TimeSpan.FromSeconds(value);
            }
        }
        
        public SettingsMenuViewModel(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager ?? throw new ArgumentNullException(nameof(settingsManager));
            
            LoadSettings();
        }

        private void LoadSettings()
        {
            _automaticUpdates = _settingsManager.AutomaticUpdateSettings.AutomaticUpdateAllowed;
            _automaticUpdatesInterval = _settingsManager.AutomaticUpdateSettings.UpdateInterval.TotalSeconds;

            _settingsManager.AutomaticUpdateSettings.SettingChanged += AutomaticUpdateSettings_OnChanged;
        }

        private void AutomaticUpdateSettings_OnChanged(object sender, SettingChangedEventArgs eventArgs)
        {
            switch (eventArgs.SettingName)
            {
                case nameof(AutomaticUpdateSettings.AutomaticUpdateAllowed):
                    if (eventArgs.TryGetNewValue<bool>(out var updatesAllowed) && AutomaticUpdates != updatesAllowed)
                        AutomaticUpdates = updatesAllowed;
                    break;

                case nameof(AutomaticUpdateSettings.UpdateInterval):
                    if (eventArgs.TryGetNewValue<TimeSpan>(out var interval) && Math.Abs(AutomaticUpdatesInterval - interval.TotalSeconds) > 0.001)
                        AutomaticUpdatesInterval = interval.TotalSeconds;
                    break;
            }
        }
    }
}