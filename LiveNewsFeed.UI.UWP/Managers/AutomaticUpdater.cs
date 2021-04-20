using System;
using System.Timers;
using Microsoft.Extensions.Logging;

using LiveNewsFeed.UI.UWP.Managers.Settings;

namespace LiveNewsFeed.UI.UWP.Managers
{
    public class AutomaticUpdater : IAutomaticUpdater
    {
        private readonly ILogger<AutomaticUpdater>? _logger;
        private readonly ISettingsManager _settingsManager;
        
        private Timer? _periodicTimer;

        public AutomaticUpdateSettings Settings => _settingsManager.AutomaticUpdateSettings;

        public event EventHandler? AutomaticUpdateRequested;

        public AutomaticUpdater(ISettingsManager settingsManager,
                                ILogger<AutomaticUpdater>? logger = default)
        {
            _settingsManager = settingsManager ?? throw new ArgumentNullException(nameof(settingsManager));
            _logger = logger;

            if (!_settingsManager.AreSettingsLoaded)
            {
                _settingsManager.SettingsLoaded += (_, _) =>
                {
                    _settingsManager.AutomaticUpdateSettings.SettingChanged += OnSettingsChanged;
                    Start();
                };
            }
            else
            {
                _settingsManager.AutomaticUpdateSettings.SettingChanged += OnSettingsChanged;
            }
        }
        
        public void Start()
        {
            if (!_settingsManager.AreSettingsLoaded)
                return;
            if (!Settings.AutomaticUpdateAllowed)
                return;
            if (_periodicTimer is { Enabled: true })
                return;

            _periodicTimer = new Timer
            {
                AutoReset = true,
                Interval = Settings.UpdateInterval.TotalMilliseconds
            };
            _periodicTimer.Elapsed += (_, _) => AutomaticUpdateRequested?.Invoke(this, EventArgs.Empty);

            _periodicTimer.Start();

            _logger?.LogInformation($"Automatic updates started with {Settings.UpdateInterval.TotalSeconds} seconds update interval.");
        }

        public void Stop()
        {
            if (_periodicTimer is not {Enabled: true})
                return;

            _periodicTimer.Stop();

            _logger?.LogInformation("Automatic updates stopped.");
        }


        private void OnSettingsChanged(object sender, SettingChangedEventArgs eventArgs)
        {
            switch (eventArgs.SettingName)
            {
                case nameof(AutomaticUpdateSettings.AutomaticUpdateAllowed):
                    if (eventArgs.NewValue is true)
                        Start();
                    if (eventArgs.NewValue is false)
                        Stop();
                    break;

                case nameof(AutomaticUpdateSettings.UpdateInterval):
                    if (_periodicTimer != null && eventArgs.TryGetNewValue<TimeSpan>(out var newInterval))
                    {
                        _periodicTimer.Interval = newInterval.TotalMilliseconds;

                        _logger?.LogInformation($"Automatic updates interval changed to {newInterval.TotalSeconds} seconds.");
                    }
                    break;
            }
        }
    }
}