using System;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public class ArticlePreviewPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        private NewsArticlePostViewModel _newsArticlePost;
        public NewsArticlePostViewModel NewsArticlePost
        {
            get => _newsArticlePost;
            set => Set(ref _newsArticlePost, value);
        }

        public ICommand ClosePreviewCommand { get; private set; }

        public ArticlePreviewPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            ClosePreviewCommand = new RelayCommand(() => _navigationService.GoBack());
        }
    }
}