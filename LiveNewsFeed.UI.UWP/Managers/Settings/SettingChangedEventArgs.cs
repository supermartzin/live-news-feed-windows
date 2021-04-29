using System;

namespace LiveNewsFeed.UI.UWP.Managers.Settings
{
    public class SettingChangedEventArgs : EventArgs
    {
        public string SettingName { get; }

        private object? _oldValue;
        public object? OldValue => _oldValue;

        private object? _newValue;
        public object? NewValue => _newValue;

        public SettingChangedEventArgs(string settingName, object? oldValue, object? newValue)
        {
            SettingName = settingName;
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public T? GetNewValueAs<T>() => GetValue<T>(ref _newValue);

        public T? GetOldValueAs<T>() => GetValue<T>(ref _oldValue);

        public bool TryGetNewValueAs<T>(out T? value) => TryGetValue(out value, ref _newValue);
        
        public bool TryGetOldValueAs<T>(out T? value) => TryGetValue(out value, ref _oldValue);


        private static T? GetValue<T>(ref object? value)
        {
            if (value is T castValue)
                return castValue;

            return default;
        }

        private static bool TryGetValue<T>(out T? value, ref object? backingValue)
        {
            if (backingValue is T valueToReturn)
            {
                value = valueToReturn;
                return true;
            }

            value = default;
            return false;
        }
    }
}