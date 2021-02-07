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

        public ImageBrush? NewsFeedLogo { get; }

        public ObservableCollection<TagViewModel>? Tags { get; }

        public ObservableCollection<CategoryViewModel>? Categories { get; }

        public ICommand OpenFullArticleCommand { get; }

        public NewsArticlePostViewModel(string title,
                                        string content,
                                        DateTime publishTime,
                                        Uri articleUrl,
                                        ImageBrush? newsFeedLogo,
                                        Uri? imageUrl = default,
                                        string? imageTitle = default,
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
            if (categories != null)
                Categories = new ObservableCollection<CategoryViewModel>(categories);
            if (tags != null)
                Tags = new ObservableCollection<TagViewModel>(tags);

            OpenFullArticleCommand = new RelayCommand(async () => await Launcher.LaunchUriAsync(ArticleUrl));
        }
    }
}