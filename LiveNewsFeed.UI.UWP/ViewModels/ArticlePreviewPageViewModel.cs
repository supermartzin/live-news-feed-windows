using System;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;

using LiveNewsFeed.Models;

using LiveNewsFeed.UI.UWP.Common;
using LiveNewsFeed.UI.UWP.Services;

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
                var changed = SetProperty(ref _newsArticlePost, value);

                if (changed)
                    SetPreviewProperties();
            }
        }

        private Uri _previewUrl;
        public Uri PreviewUrl
        {
            get => _previewUrl;
            set => SetProperty(ref _previewUrl, value);
        }

        private string? _previewHtmlSource;
        public string? PreviewHtmlSource
        {
            get => _previewHtmlSource;
            set => SetProperty(ref _previewHtmlSource, value);
        }

        public bool IsSocialPostPreview { get; set; }

        public IAsyncRelayCommand OpenInBrowserCommand { get; private set; }
        
        public ICommand ClosePreviewCommand { get; private set; }

        public ICommand CopyUrlToClipboardCommand { get; private set; }

        public ArticlePreviewPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            OpenInBrowserCommand = new AsyncRelayCommand(async () => await UiHelpers.OpenInDefaultBrowser(PreviewUrl));
            ClosePreviewCommand = new RelayCommand(_navigationService.GoBack, () => _navigationService.CanGoBack);
            CopyUrlToClipboardCommand = new RelayCommand(() => UiHelpers.ShareLinkViaClipboard(PreviewUrl));
        }

        private void SetPreviewProperties()
        {
            if (IsSocialPostPreview)
            {
                if (_newsArticlePost.SocialPost!.PostType == SocialPostType.Spotify)
                    PreviewHtmlSource = _newsArticlePost.SocialPost!.Content;
                else
                    PreviewUrl = _newsArticlePost.ArticleUrl;
            }
            else
            {
                PreviewUrl = _newsArticlePost.ArticleUrl;
            }

            PreviewUrl = IsSocialPostPreview ? _newsArticlePost.SocialPost!.Url : _newsArticlePost.ArticleUrl;
        }
    }
}