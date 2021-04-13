using System;
using System.Runtime.CompilerServices;

namespace LiveNewsFeed.UI.UWP.Managers.Settings
{
    public abstract class SettingsBase
    {
        public event EventHandler<SettingChangedEventArgs> SettingChanged;

        protected virtual void Set<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            var oldValue = field;

            if (field.Equals(value))
                return;

            field = value;

            RaiseSettingChanged(propertyName, value, oldValue);
        }

        protected virtual void RaiseSettingChanged(string settingName, object? newValue, object? oldValue = default)
        {
            SettingChanged?.Invoke(this, new SettingChangedEventArgs(settingName, oldValue, newValue));
        }
    }
}