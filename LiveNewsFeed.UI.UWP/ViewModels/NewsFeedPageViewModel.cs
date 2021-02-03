using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Data.Html;
using Windows.UI.Core;
using GalaSoft.MvvmLight;

using LiveNewsFeed.Models;

using LiveNewsFeed.UI.UWP.Managers;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public class NewsFeedPageViewModel : ViewModelBase
    {
        private readonly IDataSourcesManager _dataSourcesManager;

        private ObservableCollection<NewsArticlePostViewModel> _articlePosts;
        public ObservableCollection<NewsArticlePostViewModel> ArticlePosts
        {
            get => _articlePosts;
            set => Set(ref _articlePosts, value);
        }

        public NewsFeedPageViewModel(IDataSourcesManager dataSourcesManager)
        {
            _dataSourcesManager = dataSourcesManager ?? throw new ArgumentNullException(nameof(dataSourcesManager));

            Task.Factory.StartNew(async () => await LoadPosts());
        }

        public async Task LoadPosts()
        {
            var posts = await _dataSourcesManager.GetLatestPostsFromAllAsync();

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () => ArticlePosts = new ObservableCollection<NewsArticlePostViewModel>(posts.Select(ToViewModel)));
        }

        
        private static NewsArticlePostViewModel ToViewModel(NewsArticlePost articlePost) =>
            new NewsArticlePostViewModel(articlePost.Title,
                                         HtmlUtilities.ConvertToText(articlePost.Content),
                                         articlePost.PublishTime,
                                         articlePost.FullArticleUrl);
    }
}