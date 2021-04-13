using System;
using System.Timers;

using LiveNewsFeed.UI.UWP.Managers.Settings;

namespace LiveNewsFeed.UI.UWP.Managers
{
    public class AutomaticUpdater : IAutomaticUpdater
    {
        private readonly ISettingsManager _settingsManager;
        
        private Timer _periodicTimer;

        public AutomaticUpdateSettings Settings => _settingsManager.AutomaticUpdateSettings;

        public event EventHandler? AutomaticUpdateRequested;

        public AutomaticUpdater(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager ?? throw new ArgumentNullException(nameof(settingsManager));

            if (!_settingsManager.AreSettingsLoaded)
                _settingsManager.SettingsLoaded += (_, _) => Start();
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
        }

        public void Stop()
        {
            if (!_periodicTimer.Enabled)
                return;

            _periodicTimer.Stop();
        }
    }
}