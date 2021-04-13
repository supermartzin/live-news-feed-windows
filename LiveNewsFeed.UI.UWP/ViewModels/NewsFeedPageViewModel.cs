using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private readonly IAutomaticUpdater _automaticUpdater;

        private readonly ObservableCollection<NewsArticlePostViewModel> _articlePosts;

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
        
        public QuickSettingsViewModel QuickSettings { get; }

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
                                     IAutomaticUpdater automaticUpdater,
                                     QuickSettingsViewModel quickSettingsViewModel)
        {
            _dataSourcesManager = dataSourcesManager ?? throw new ArgumentNullException(nameof(dataSourcesManager));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _notificationsManager = notificationsManager ?? throw new ArgumentNullException(nameof(notificationsManager));
            _automaticUpdater = automaticUpdater ?? throw new ArgumentNullException(nameof(automaticUpdater));
            QuickSettings = quickSettingsViewModel ?? throw new ArgumentNullException(nameof(quickSettingsViewModel));

            _articlePosts = new ObservableCollection<NewsArticlePostViewModel>();
            ArticlePosts = new AdvancedCollectionView(_articlePosts);
            ArticlePosts.SortDescriptions.Add(new SortDescription(nameof(NewsArticlePostViewModel.PublishTime), SortDirection.Descending));

            _dataSourcesManager.NewsArticlePostReceived += DataSourcesManager_OnNewsArticlePostReceived;
           
            InitializeCommands();
            LoadPosts();
            StartUpdater();
        }
        

        private void LoadPosts()
        {
            AllPostsLoading = true;
            
            _dataSourcesManager.GetLatestPostsFromAllAsync()
                               .ContinueWith(task =>
                               {
                                   var posts = task.Result;

                                   InvokeOnUi(() =>
                                   {
                                       using (ArticlePosts.DeferRefresh())
                                       {
                                           foreach (var post in posts)
                                           {
                                               var viewModel = new NewsArticlePostViewModel(post);

                                               RegisterEvents(viewModel);

                                               _articlePosts.Add(viewModel);
                                           }
                                       }

                                       AllPostsLoading = false;
                                   });
                               });
        }

        private void StartUpdater()
        {
            _automaticUpdater.AutomaticUpdateRequested +=
                (_, _) =>
                {
                    _dataSourcesManager.LoadLatestPostsSinceLastUpdateAsync();
                };

            _automaticUpdater.Start();
        }

        private void RegisterEvents(NewsArticlePostViewModel post)
        {
            post.ShowImagePreviewRequested += NewsArticlePost_OnShowImagePreviewRequested;
            post.HideImagePreviewRequested += NewsArticlePost_OnHideImagePreviewRequested;
            post.OpenArticlePreviewRequested += NewsArticlePost_OnOpenArticlePreviewRequested;
            post.OpenSocialPostPreviewRequested += NewsArticlePost_OnOpenSocialPostPreviewRequested;
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
            _navigationService.NavigateTo(nameof(ArticlePreviewPage), new NavigationParameters(new Dictionary<string, object>
            {
                { "post", sender },
                { "isSocialPost", false }
            }));
        }

        private void NewsArticlePost_OnOpenSocialPostPreviewRequested(object sender, EventArgs e)
        {
            _navigationService.NavigateTo(nameof(ArticlePreviewPage), new NavigationParameters(new Dictionary<string, object>
            {
                { "post", sender },
                { "isSocialPost", true }
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
            NewPostsLoading = true;
            
            _dataSourcesManager.GetLatestPostsSinceLastUpdateAsync()
                               .ContinueWith(task =>
                               {
                                   var newPosts = task.Result;
                                   var posts = new List<NewsArticlePost>(newPosts);

                                   if (posts.Count == 0)
                                       return;

                                   InvokeOnUi(() =>
                                   {
                                       using (ArticlePosts.DeferRefresh())
                                       {
                                           foreach (var viewModel in posts.Select(post => new NewsArticlePostViewModel(post)))
                                           {
                                               RegisterEvents(viewModel);

                                               _articlePosts.Add(viewModel);
                                           }
                                       }

                                       NewPostsLoading = false;
                                   });
                               });
        }

        private bool CanReloadArticlesManually() => !NewPostsLoading;

        private void DataSourcesManager_OnNewsArticlePostReceived(object sender, NewsArticlePost newsArticlePost)
        {
            InvokeOnUi(() =>
            {
                var viewModel = new NewsArticlePostViewModel(newsArticlePost);

                RegisterEvents(viewModel);

                _articlePosts.Add(viewModel);
                
                // show notification
                _notificationsManager.ShowNotification(newsArticlePost);
            });

        }
    }
}