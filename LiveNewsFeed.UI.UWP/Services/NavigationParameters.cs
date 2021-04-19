using System;
using System.Collections.Generic;

namespace LiveNewsFeed.UI.UWP.Services
{
    public sealed class NavigationParameters
    {
        private readonly IDictionary<string, object> _parameters;

        public NavigationParameters(IDictionary<string, object>? parameters = default)
        {
            _parameters = parameters ?? new Dictionary<string, object>();
        }

        public void SetValue<T>(string key, T value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value != null)
                _parameters[key] = value;
        }

        public T? GetValue<T>(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (!_parameters.ContainsKey(key))
                return default;

            if (_parameters[key] is T value)
                return value;

            return default;
        }
    }
}