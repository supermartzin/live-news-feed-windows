using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Data.Html;
using Windows.UI.Xaml.Media;
using GalaSoft.MvvmLight.Command;

using LiveNewsFeed.Models;

using LiveNewsFeed.UI.UWP.Common;
using LiveNewsFeed.UI.UWP.Managers;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public class NewsFeedPageViewModel : ViewModelBase
    {
        private readonly IDataSourcesManager _dataSourcesManager;

        private bool _postsLoading;
        public bool PostsLoading
        {
            get => _postsLoading;
            set
            {
                var changed = Set(ref _postsLoading, value);

                if (changed)
                    ReevaluateCommands();
            }
        }

        private ObservableCollection<NewsArticlePostViewModel> _articlePosts;
        public ObservableCollection<NewsArticlePostViewModel> ArticlePosts
        {
            get => _articlePosts;
            set => Set(ref _articlePosts, value);
        }

        public RelayCommand RefreshNewsFeedCommand { get; private set; }

        public NewsFeedPageViewModel(IDataSourcesManager dataSourcesManager)
        {
            _dataSourcesManager = dataSourcesManager ?? throw new ArgumentNullException(nameof(dataSourcesManager));

            InitializeCommands();
            LoadPosts();
        }


        private void LoadPosts()
        {
            PostsLoading = true;

            _dataSourcesManager.GetLatestPostsFromAllAsync()
                .ContinueWith(task => InvokeOnUi(() =>
                {
                    ArticlePosts = new ObservableCollection<NewsArticlePostViewModel>(task.Result.Select(ToViewModel));

                    PostsLoading = false;
                }));
        }

        private void InitializeCommands()
        {
            RefreshNewsFeedCommand = new RelayCommand(ReloadArticlesManually, CanReloadArticlesManually);
        }

        private void ReevaluateCommands()
        {
            RefreshNewsFeedCommand.RaiseCanExecuteChanged();
        }

        private void ReloadArticlesManually()
        {
            PostsLoading = true;

            _dataSourcesManager.GetLatestPostsSinceLastUpdateAsync()
                .ContinueWith(task => InvokeOnUi(() =>
                {
                    foreach (var post in task.Result)
                    {
                        ArticlePosts.Add(ToViewModel(post));
                    }

                    PostsLoading = false;
                }));
        }

        private bool CanReloadArticlesManually() => !PostsLoading;
        
        private NewsArticlePostViewModel ToViewModel(NewsArticlePost articlePost) =>
            new (articlePost.Title,
                                         HtmlUtilities.ConvertToText(articlePost.Content),
                                         articlePost.PublishTime,
                                         articlePost.FullArticleUrl,
                                         GetNewsFeedLogo(articlePost),
                                         articlePost.Image?.Url,
                                         articlePost.Image?.Title);

        private ImageBrush GetNewsFeedLogo(NewsArticlePost articlePost) => Helpers.GetLogoForNewsFeed(articlePost.NewsFeedName) ?? new ImageBrush();
    }
}