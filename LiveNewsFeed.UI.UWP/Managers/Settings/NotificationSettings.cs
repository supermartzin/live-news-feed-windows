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

        private bool _notifyOnlyOnImportantPosts;
        public bool NotifyOnlyOnImportantPosts
        {
            get => _notifyOnlyOnImportantPosts;
            set => Set(ref _notifyOnlyOnImportantPosts, value);
        }
    }
}