namespace LiveNewsFeed.UI.UWP.Managers.Settings
{
    public class NotificationSettings : SettingsBase
    {
        private bool _notificationAllowed;
        public bool NotificationsAllowed
        {
            get => _notificationAllowed;
            set => Set(ref _notificationAllowed, value);
        }

        private bool _onlyImportantPosts;
        public bool OnlyImportantPosts
        {
            get => _onlyImportantPosts;
            set => Set(ref _onlyImportantPosts, value);
        }
    }
}