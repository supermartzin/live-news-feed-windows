using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Windows.Data.Html;
using Windows.System;
using Windows.UI.Xaml.Media;
using GalaSoft.MvvmLight.Command;

using LiveNewsFeed.Models;
using LiveNewsFeed.UI.UWP.Common;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public class NewsArticlePostViewModel : ViewModelBase
    {
        #region Properties

        public NewsArticlePost OriginalPost { get; }

        public string Title => OriginalPost.Title;

        public string Content => HtmlUtilities.ConvertToText(OriginalPost.Content).Trim();

        public DateTime PublishTime => OriginalPost.PublishTime;

        public Uri ArticleUrl => OriginalPost.FullArticleUrl;

        public ImageViewModel? Image { get; }

        public SocialPostViewModel? SocialPost { get; }

        public ImageBrush? NewsFeedLogo { get; }

        public IList<TagViewModel>? Tags { get; }

        public IList<CategoryViewModel>? Categories { get; }

        #endregion

        #region Commands

        public ICommand OpenFullArticleCommand { get; private set; }

        public ICommand CopyArticleUrlToClipboardCommand { get; private set; }

        public ICommand ShareArticleCommand { get; private set; }

        public ICommand OpenImageInBrowserCommand { get; private set; }

        public ICommand ShowImagePreviewCommand { get; private set; }

        public ICommand HideImagePreviewCommand { get; private set; }

        public ICommand OpenArticlePreviewCommand { get; private set; }

        public ICommand OpenSocialPostPreviewCommand { get; private set; }

        #endregion

        #region Events

        public event EventHandler ShowImagePreviewRequested;
        public event EventHandler HideImagePreviewRequested;
        public event EventHandler OpenArticlePreviewRequested;
        public event EventHandler OpenSocialPostPreviewRequested;

        #endregion

        public NewsArticlePostViewModel(NewsArticlePost newsArticlePost)
        {
            OriginalPost = newsArticlePost ?? throw new ArgumentNullException(nameof(newsArticlePost));

            Image = OriginalPost.Image != null ? new ImageViewModel(OriginalPost.Image) : default;
            SocialPost = OriginalPost.SocialPost != null ? new SocialPostViewModel(OriginalPost.SocialPost) : default;
            NewsFeedLogo = new ImageBrush {ImageSource = Helpers.GetLogoForNewsFeed(OriginalPost.NewsFeedName)};
            Categories = OriginalPost.Categories
                                      .Where(category => category != Category.NotCategorized)
                                      .Select(Helpers.GetCategoryViewModel)
                                      .ToList();
            Tags = OriginalPost.Tags
                                .Select(tag => new TagViewModel(tag))
                                .ToList();

            InitializeCommands();
        }


        private void InitializeCommands()
        {
            OpenFullArticleCommand = new RelayCommand(async () => await UiHelpers.OpenInDefaultBrowser(ArticleUrl));
            CopyArticleUrlToClipboardCommand = new RelayCommand(() => UiHelpers.ShareLinkViaClipboard(ArticleUrl));
            ShareArticleCommand = new RelayCommand(() => UiHelpers.ShareArticleViaSystemUI(OriginalPost));
            OpenImageInBrowserCommand = new RelayCommand(async () => await UiHelpers.OpenInDefaultBrowser(Image?.LargeImageUrl));
            ShowImagePreviewCommand = new RelayCommand(() => ShowImagePreviewRequested?.Invoke(this, EventArgs.Empty));
            HideImagePreviewCommand = new RelayCommand(() => HideImagePreviewRequested?.Invoke(this, EventArgs.Empty));
            OpenArticlePreviewCommand = new RelayCommand(() => OpenArticlePreviewRequested?.Invoke(this, EventArgs.Empty));
            OpenSocialPostPreviewCommand = new RelayCommand(() => OpenSocialPostPreviewRequested?.Invoke(this, EventArgs.Empty));
        }
    }
}