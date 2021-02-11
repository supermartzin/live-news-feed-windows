using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.System;
using Windows.UI.Xaml.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public class NewsArticlePostViewModel : ViewModelBase
    {
        public string Title { get; }

        public string Content { get; }

        public DateTime PublishTime { get; }

        public Uri ArticleUrl { get; }

        public Uri? ImageUrl { get; }

        public string? ImageTitle { get; }

        public string? SocialPostContent { get; }

        public ImageBrush? NewsFeedLogo { get; }

        public ObservableCollection<TagViewModel>? Tags { get; }

        public ObservableCollection<CategoryViewModel>? Categories { get; }

        public ICommand OpenFullArticleCommand { get; private set; }

        public ICommand ShowImagePreviewCommand { get; private set; }

        public ICommand HideImagePreviewCommand { get; private set; }

        public event EventHandler ShowImagePreviewRequested;
        public event EventHandler HideImagePreviewRequested;

        public NewsArticlePostViewModel(string title,
                                        string content,
                                        DateTime publishTime,
                                        Uri articleUrl,
                                        ImageBrush? newsFeedLogo,
                                        Uri? imageUrl = default,
                                        string? imageTitle = default,
                                        string? socialPostContent = default,
                                        IEnumerable<CategoryViewModel>? categories = default,
                                        IEnumerable<TagViewModel>? tags = default)
        {
            Title = title;
            Content = content;
            PublishTime = publishTime;
            ArticleUrl = articleUrl;
            NewsFeedLogo = newsFeedLogo;
            ImageUrl = imageUrl;
            ImageTitle = imageTitle;
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
            ShowImagePreviewCommand = new RelayCommand(() => ShowImagePreviewRequested?.Invoke(this, EventArgs.Empty));
            HideImagePreviewCommand = new RelayCommand(() => HideImagePreviewRequested?.Invoke(this, EventArgs.Empty));
        }
    }
}