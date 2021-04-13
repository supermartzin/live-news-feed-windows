using System;

namespace LiveNewsFeed.UI.UWP.Managers.Settings
{
    public class SettingChangedEventArgs : EventArgs
    {
        public string SettingName { get; }

        public object? OldValue { get; }

        public object? NewValue { get; }

        public SettingChangedEventArgs(string settingName, object? oldValue, object? newValue)
        {
            SettingName = settingName;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}