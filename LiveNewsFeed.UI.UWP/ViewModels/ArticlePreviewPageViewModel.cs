using System;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;

using LiveNewsFeed.UI.UWP.Common;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public class ArticlePreviewPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        private NewsArticlePostViewModel _newsArticlePost;
        public NewsArticlePostViewModel NewsArticlePost
        {
            get => _newsArticlePost;
            set
            {
                var changed = Set(ref _newsArticlePost, value);

                if (changed)
                {
                    PreviewUrl = IsSocialPostPreview ? _newsArticlePost.SocialPost!.Url : _newsArticlePost.ArticleUrl;
                }
            }
        }

        private Uri _previewUrl;
        public Uri PreviewUrl
        {
            get => _previewUrl;
            set => Set(ref _previewUrl, value);
        }

        public bool IsSocialPostPreview { get; set; }

        public ICommand OpenInBrowserCommand { get; private set; }
        
        public ICommand ClosePreviewCommand { get; private set; }

        public ICommand CopyUrlToClipboardCommand { get; private set; }

        public ArticlePreviewPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            OpenInBrowserCommand = new RelayCommand(async () => await UiHelpers.OpenInDefaultBrowser(PreviewUrl));
            ClosePreviewCommand = new RelayCommand(() => _navigationService.GoBack());
            CopyUrlToClipboardCommand = new RelayCommand(() => UiHelpers.ShareLinkViaClipboard(PreviewUrl));
        }

        private void SetNewsArticlePost(NewsArticlePostViewModel articlePost)
        {
            
        }
    }
}