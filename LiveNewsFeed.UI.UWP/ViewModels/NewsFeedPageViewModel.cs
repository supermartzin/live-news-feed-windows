using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using LiveNewsFeed.Models;
using Microsoft.Toolkit.Uwp.UI;

using LiveNewsFeed.UI.UWP.Common;
using LiveNewsFeed.UI.UWP.Managers;
using LiveNewsFeed.UI.UWP.Views;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public class NewsFeedPageViewModel : ViewModelBase
    {
        private readonly IDataSourcesManager _dataSourcesManager;
        private readonly INavigationService _navigationService;
        private readonly INotificationsManager _notificationsManager;

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

        private NewsArticlePostViewModel? _selectedPost;
        public NewsArticlePostViewModel? SelectedPost
        {
            get => _selectedPost;
            set => Set(ref _selectedPost, value);
        }

        public AdvancedCollectionView ArticlePosts { get; }
        
        public RelayCommand RefreshNewsFeedCommand { get; private set; }

        public NewsFeedPageViewModel(IDataSourcesManager dataSourcesManager,
                                     INavigationService navigationService,
                                     INotificationsManager notificationsManager,
                                     QuickSettingsViewModel quickSettingsViewModel)
        {
            _dataSourcesManager = dataSourcesManager ?? throw new ArgumentNullException(nameof(dataSourcesManager));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _notificationsManager = notificationsManager ?? throw new ArgumentNullException(nameof(notificationsManager));
            _quickSettings = quickSettingsViewModel ?? throw new ArgumentNullException(nameof(quickSettingsViewModel));

            ArticlePosts = new AdvancedCollectionView(new List<NewsArticlePostViewModel>(), true);
            ArticlePosts.SortDescriptions.Add(new SortDescription(nameof(NewsArticlePostViewModel.PublishTime), SortDirection.Descending));

            _dataSourcesManager.NewsArticlePostReceived += DataSourcesManager_OnNewsArticlePostReceived;
            _notificationsManager.Settings.NotificationsAllowed = true;

            InitializeCommands();
            LoadPosts();
        }
        

        private void LoadPosts()
        {
            AllPostsLoading = true;
            
            _dataSourcesManager.GetLatestPostsFromAllAsync()
                               .ContinueWith(task =>
                               {
                                   var posts = task.Result;

                                   return InvokeOnUi(() =>
                                   {
                                       using (ArticlePosts.DeferRefresh())
                                       {
                                           foreach (var post in posts)
                                           {
                                               var viewModel = Helpers.ToViewModel(post);

                                               RegisterEvents(viewModel);

                                               ArticlePosts.Add(viewModel);
                                           }
                                       }

                                       AllPostsLoading = false;
                                   });
                               });
        }

        private void RegisterEvents(NewsArticlePostViewModel post)
        {
            post.ShowImagePreviewRequested += NewsArticlePost_OnShowImagePreviewRequested;
            post.HideImagePreviewRequested += NewsArticlePost_OnHideImagePreviewRequested;
            post.OpenArticlePreviewRequested += NewsArticlePost_OnOpenArticlePreviewRequested;
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

        private void NewsArticlePost_OnOpenArticlePreviewRequested(object sender, EventArgs e)
        {
            _navigationService.NavigateTo(nameof(ArticlePreviewPage), sender);
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
                               .ContinueWith(task =>
                               {
                                   var newPosts = task.Result;

                                   return InvokeOnUi(() =>
                                   {
                                       foreach (var post in newPosts)
                                       {
                                           using (ArticlePosts.DeferRefresh())
                                           {
                                               var viewModel = Helpers.ToViewModel(post);

                                               RegisterEvents(viewModel);

                                               ArticlePosts.Add(viewModel);
                                           }
                                       }

                                       NewPostsLoading = false;
                                   });
                               });
        }

        private bool CanReloadArticlesManually() => !NewPostsLoading;

        private async void DataSourcesManager_OnNewsArticlePostReceived(object sender, NewsArticlePost newsArticlePost)
        {
            var viewModel = Helpers.ToViewModel(newsArticlePost);

            RegisterEvents(viewModel);

            await InvokeOnUi(() => ArticlePosts.Add(viewModel));
        }
    }
}