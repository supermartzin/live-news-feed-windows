using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public class QuickSettingsViewModel : ViewModelBase
    {
        private bool _notificationsTurnedOn;
        public bool NotificationsTurnedOn
        {
            get => _notificationsTurnedOn;
            set => Set(ref _notificationsTurnedOn, value);
        }

        private bool _showOnlyImportantPosts;
        public bool ShowOnlyImportantPosts
        {
            get => _showOnlyImportantPosts;
            set => Set(ref _showOnlyImportantPosts, value);
        }

        public ICommand TurnOnNotificationsCommand { get; private set; }

        public ICommand ShowOnlyImportantPostsCommand { get; private set; }

        public QuickSettingsViewModel()
        {
            InitializeCommands();
        }


        private void InitializeCommands()
        {
            TurnOnNotificationsCommand = new RelayCommand(() => NotificationsTurnedOn = !NotificationsTurnedOn);
            ShowOnlyImportantPostsCommand = new RelayCommand(() => ShowOnlyImportantPosts = !ShowOnlyImportantPosts);
        }
    }
}