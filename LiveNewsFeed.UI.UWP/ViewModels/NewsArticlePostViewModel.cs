using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using Windows.UI.Xaml.Media;
using GalaSoft.MvvmLight.Command;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public class NewsArticlePostViewModel : ViewModelBase
    {
        public string Title { get; }

        public string Content { get; }

        public DateTime PublishTime { get; }

        public Uri ArticleUrl { get; }

        public ImageViewModel? Image { get; }

        public string? SocialPostContent { get; }

        public ImageBrush? NewsFeedLogo { get; }

        public ObservableCollection<TagViewModel>? Tags { get; }

        public ObservableCollection<CategoryViewModel>? Categories { get; }

        public ICommand OpenFullArticleCommand { get; private set; }

        public ICommand CopyArticleUrlToClipboardCommand { get; private set; }

        public ICommand ShareArticleCommand { get; private set; }

        public ICommand OpenImageInBrowserCommand { get; private set; }

        public ICommand ShowImagePreviewCommand { get; private set; }

        public ICommand HideImagePreviewCommand { get; private set; }

        public ICommand OpenArticlePreviewCommand { get; private set; }

        public event EventHandler ShowImagePreviewRequested;
        public event EventHandler HideImagePreviewRequested;
        public event EventHandler OpenArticlePreviewRequested;

        public NewsArticlePostViewModel(string title,
                                        string content,
                                        DateTime publishTime,
                                        Uri articleUrl,
                                        ImageBrush? newsFeedLogo,
                                        ImageViewModel? image = default,
                                        string? socialPostContent = default,
                                        IEnumerable<CategoryViewModel>? categories = default,
                                        IEnumerable<TagViewModel>? tags = default)
        {
            Title = title;
            Content = content;
            PublishTime = publishTime;
            ArticleUrl = articleUrl;
            NewsFeedLogo = newsFeedLogo;
            Image = image;
            SocialPostContent = socialPostContent;
            if (categories != null)
                Categories = new ObservableCollection<CategoryViewModel>(categories);
            if (tags != null)
                Tags = new ObservableCollection<TagViewModel>(tags);

            InitializeCommands();
        }


        private void InitializeCommands()
        {
            OpenFullArticleCommand = new RelayCommand(async () => await Launcher.LaunchUriAsync(ArticleUrl));
            CopyArticleUrlToClipboardCommand = new RelayCommand(CopyArticleLinkToClipboard);
            ShareArticleCommand = new RelayCommand(ShareArticle);
            OpenImageInBrowserCommand = new RelayCommand(async () => await Launcher.LaunchUriAsync(Image?.LargeImageUrl));
            ShowImagePreviewCommand = new RelayCommand(() => ShowImagePreviewRequested?.Invoke(this, EventArgs.Empty));
            HideImagePreviewCommand = new RelayCommand(() => HideImagePreviewRequested?.Invoke(this, EventArgs.Empty));
            OpenArticlePreviewCommand = new RelayCommand(() => OpenArticlePreviewRequested?.Invoke(this, EventArgs.Empty));
        }
        
        private void CopyArticleLinkToClipboard()
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(ArticleUrl.AbsoluteUri);

            Clipboard.SetContent(dataPackage);
        }

        private void ShareArticle()
        {
            DataTransferManager.GetForCurrentView().DataRequested += DataTransferManager_OnDataRequested;
            DataTransferManager.ShowShareUI();
        }

        private void DataTransferManager_OnDataRequested(DataTransferManager sender, DataRequestedEventArgs eventArgs)
        {
            DataTransferManager.GetForCurrentView().DataRequested -= DataTransferManager_OnDataRequested;

            // set shared content
            eventArgs.Request.Data.SetWebLink(ArticleUrl);
            eventArgs.Request.Data.Properties.ContentSourceWebLink = ArticleUrl;
            eventArgs.Request.Data.Properties.Title = Title;
            eventArgs.Request.Data.Properties.Description = GetContentForSharing();
            eventArgs.Request.Data.Properties.ApplicationName = Package.Current.DisplayName;
        }

        private string GetContentForSharing() => Content.Length > 100
                                                    ? Content.Substring(0, 100) + "..."
                                                    : Content;
    }
}