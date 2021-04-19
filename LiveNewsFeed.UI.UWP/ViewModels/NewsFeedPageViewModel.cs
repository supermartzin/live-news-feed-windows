using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Mvvm.Input;

using LiveNewsFeed.Models;

using LiveNewsFeed.UI.UWP.Managers;
using LiveNewsFeed.UI.UWP.Services;
using LiveNewsFeed.UI.UWP.Views;
using Microsoft.Extensions.Logging;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public class NewsFeedPageViewModel : ViewModelBase
    {
        private readonly ILogger<NewsFeedPageViewModel>? _logger;

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
                var changed = SetProperty(ref _allPostsLoading, value);

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
                var changed = SetProperty(ref _newPostsLoading, value);

                if (changed)
                    ReevaluateCommands();
            }
        }
        
        public QuickSettingsViewModel QuickSettings { get; }

        private NewsArticlePostViewModel? _selectedPost;
        public NewsArticlePostViewModel? SelectedPost
        {
            get => _selectedPost;
            set => SetProperty(ref _selectedPost, value);
        }

        public AdvancedCollectionView ArticlePosts { get; }
        
        public IAsyncRelayCommand RefreshNewsFeedCommand { get; private set; }

        public NewsFeedPageViewModel(IDataSourcesManager dataSourcesManager,
                                     INavigationService navigationService,
                                     INotificationsManager notificationsManager,
                                     IAutomaticUpdater automaticUpdater,
                                     QuickSettingsViewModel quickSettingsViewModel,
                                     ILogger<NewsFeedPageViewModel>? logger = default)
        {
            _dataSourcesManager = dataSourcesManager ?? throw new ArgumentNullException(nameof(dataSourcesManager));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _notificationsManager = notificationsManager ?? throw new ArgumentNullException(nameof(notificationsManager));
            _automaticUpdater = automaticUpdater ?? throw new ArgumentNullException(nameof(automaticUpdater));
            QuickSettings = quickSettingsViewModel ?? throw new ArgumentNullException(nameof(quickSettingsViewModel));
            _logger = logger;

            _articlePosts = new ObservableCollection<NewsArticlePostViewModel>();
            ArticlePosts = new AdvancedCollectionView(_articlePosts);
            ArticlePosts.SortDescriptions.Add(new SortDescription(nameof(NewsArticlePostViewModel.PublishTime), SortDirection.Descending));

            _dataSourcesManager.NewsArticlePostReceived += DataSourcesManager_OnNewsArticlePostReceived;
           
            InitializeCommands();
            LoadPosts();
            StartUpdater();
        }
        

        private async Task LoadPosts()
        {
            AllPostsLoading = true;

            var posts = await _dataSourcesManager.GetLatestPostsFromAllAsync();

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
        }

        private void StartUpdater()
        {
            _automaticUpdater.AutomaticUpdateRequested += (_, _) =>
            {
                _logger?.LogDebug($"Automatic news feeds update initiated...");

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
            _navigationService.NavigateTo<ArticlePreviewPage>(new NavigationParameters(new Dictionary<string, object>
            {
                { "post", sender },
                { "isSocialPost", false }
            }));
        }

        private void NewsArticlePost_OnOpenSocialPostPreviewRequested(object sender, EventArgs e)
        {
            _navigationService.NavigateTo<ArticlePreviewPage>(new NavigationParameters(new Dictionary<string, object>
            {
                { "post", sender },
                { "isSocialPost", true }
            }));
        }

        private void InitializeCommands()
        {
            RefreshNewsFeedCommand = new AsyncRelayCommand(ReloadArticlesManually, CanReloadArticlesManually);
        }

        private void ReevaluateCommands()
        {
            RefreshNewsFeedCommand.NotifyCanExecuteChanged();
        }

        private async Task ReloadArticlesManually()
        {
            NewPostsLoading = true;

            var posts = await _dataSourcesManager.GetLatestPostsSinceLastUpdateAsync();

            foreach (var viewModel in posts.Select(post => new NewsArticlePostViewModel(post)))
            {
                RegisterEvents(viewModel);

                _articlePosts.Add(viewModel);
            }

            NewPostsLoading = false;
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