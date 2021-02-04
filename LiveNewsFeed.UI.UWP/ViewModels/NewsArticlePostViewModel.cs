using System;
using System.Windows.Input;
using Windows.System;
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

        public string NewsFeedLogo { get; }

        public ICommand OpenFullArticleCommand { get; }

        public NewsArticlePostViewModel(string title,
                                        string content,
                                        DateTime publishTime,
                                        Uri articleUrl,
                                        string newsFeedLogo)
        {
            Title = title;
            Content = content;
            PublishTime = publishTime;
            ArticleUrl = articleUrl;
            NewsFeedLogo = newsFeedLogo;

            OpenFullArticleCommand = new RelayCommand(async () => await Launcher.LaunchUriAsync(ArticleUrl));
        }
    }
}