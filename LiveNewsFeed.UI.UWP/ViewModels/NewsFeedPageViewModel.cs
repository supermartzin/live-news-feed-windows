using System;
using System.Collections.Generic;
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

        private NewsArticlePostViewModel? _selectedPost;
        public NewsArticlePostViewModel? SelectedPost
        {
            get => _selectedPost;
            set => Set(ref _selectedPost, value);
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
                                   var posts = task.Result.Select(Helpers.ToViewModel).ToList();

                                   RegisterEvents(posts);

                                   ArticlePosts = new ObservableCollection<NewsArticlePostViewModel>(posts);

                                   PostsLoading = false;
                               }));
        }

        private void RegisterEvents(IEnumerable<NewsArticlePostViewModel> posts)
        {
            foreach (var post in posts)
            {
                post.ShowImagePreviewRequested += NewsArticlePost_OnShowImagePreviewRequested;
                post.HideImagePreviewRequested += NewsArticlePost_OnHideImagePreviewRequested;
            }
        }
        
        private void NewsArticlePost_OnShowImagePreviewRequested(object sender, EventArgs e)
        {
            if (sender is NewsArticlePostViewModel postViewModel)
            {
                SelectedPost = postViewModel;
            }
        }

        private void NewsArticlePost_OnHideImagePreviewRequested(object sender, EventArgs e)
        {
            SelectedPost = null;
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
                        ArticlePosts.Add(Helpers.ToViewModel(post));
                    }

                    PostsLoading = false;
                }));
        }

        private bool CanReloadArticlesManually() => !PostsLoading;
    }
}