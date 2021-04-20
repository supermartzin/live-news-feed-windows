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
    }
}