using System.Collections.Generic;

using LiveNewsFeed.Models;

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

        private ISet<Category> _showCategories;
        public ISet<Category> ShowCategories
        {
            get => _showCategories;
            set => Set(ref _showCategories, value);
        }
    }
}