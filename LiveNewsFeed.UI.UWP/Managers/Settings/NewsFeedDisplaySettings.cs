using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LiveNewsFeed.UI.UWP.Managers.Settings
{
    public class NewsFeedDisplaySettings : SettingsBase
    {
        private bool _showOnlyImportantPosts;
        public bool ShowOnlyImportantPosts
        {
            get => _showOnlyImportantPosts;
            set => Set(ref _showOnlyImportantPosts, value);
        }

        private readonly IDictionary<string, bool> _newsFeedDataSourceStates;
        public IDictionary<string, bool> NewsFeedDataSourceStates => new ReadOnlyDictionary<string, bool>(_newsFeedDataSourceStates);

        public NewsFeedDisplaySettings()
        {
            _newsFeedDataSourceStates = new Dictionary<string, bool>();
        }

        public void SetNewsFeedDataSourceState(string newsFeedName, bool isEnabled)
        {
            if (newsFeedName is null)
                throw new ArgumentNullException(nameof(newsFeedName));

            if (!_newsFeedDataSourceStates.ContainsKey(newsFeedName))
                _newsFeedDataSourceStates.Add(newsFeedName, true);

            var currentState = _newsFeedDataSourceStates[newsFeedName];

            if (currentState != isEnabled)
            {
                _newsFeedDataSourceStates[newsFeedName] = isEnabled;

                RaiseSettingChanged(nameof(NewsFeedDataSourceStates), currentState, isEnabled);
            }
        }
    }
}