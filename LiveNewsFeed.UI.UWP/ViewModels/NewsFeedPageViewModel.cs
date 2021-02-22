using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Command;

using LiveNewsFeed.UI.UWP.Common;
using LiveNewsFeed.UI.UWP.Managers;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public class NewsFeedPageViewModel : ViewModelBase
    {
        private readonly IDataSourcesManager _dataSourcesManager;

        private bool _allPostsLoading;
        public bool AllPostsLoading
        {
            get => _allPostsLoading;
            set
            {
                var changed = Set(ref _allPostsLoading, value);

                if (changed)
                    ReevaluateCommands();
            }
        }

        private bool _newPostsLoading;
        public bool NewPostsLoading
        {
            get => _newPostsLoading;
            set
            {
                var changed = Set(ref _newPostsLoading, value);

                if (changed)
                    ReevaluateCommands();
            }
        }
        
        private QuickSettingsViewModel _quickSettings;
        public QuickSettingsViewModel QuickSettings
        {
            get => _quickSettings;
            set => Set(ref _quickSettings, value);
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

        public NewsFeedPageViewModel(IDataSourcesManager dataSourcesManager,
                                     QuickSettingsViewModel quickSettingsViewModel)
        {
            _dataSourcesManager = dataSourcesManager ?? throw new ArgumentNullException(nameof(dataSourcesManager));
            _quickSettings = quickSettingsViewModel ?? throw new ArgumentNullException(nameof(quickSettingsViewModel));

            InitializeCommands();
            LoadPosts();
        }


        private void LoadPosts()
        {
            AllPostsLoading = true;
            
            _dataSourcesManager.GetLatestPostsFromAllAsync()
                               .ContinueWith(task => InvokeOnUi(() =>
                               {
                                   var posts = task.Result
                                                                              .Select(Helpers.ToViewModel)
                                                                              .ToList();

                                   RegisterEvents(posts);

                                   ArticlePosts = new ObservableCollection<NewsArticlePostViewModel>(posts);

                                   AllPostsLoading = false;
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
            NewPostsLoading = true;

            _dataSourcesManager.GetLatestPostsSinceLastUpdateAsync()
                               .ContinueWith(task => InvokeOnUi(() =>
                               {
                                   var newPosts = task.Result
                                                                                 .Select(Helpers.ToViewModel)
                                                                                 .ToList();
                                   
                                   RegisterEvents(newPosts);

                                   foreach (var post in newPosts)
                                   {
                                       ArticlePosts.Insert(0, post);
                                   }

                                   ArticlePosts .SortDescending(post => post.PublishTime);

                                   NewPostsLoading = false;
                               }));
        }

        private bool CanReloadArticlesManually() => !NewPostsLoading;
    }
}