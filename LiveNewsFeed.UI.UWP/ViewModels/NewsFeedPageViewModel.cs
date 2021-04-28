using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;
using Windows.Globalization;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

using LiveNewsFeed.Models;

using LiveNewsFeed.UI.UWP.Managers;
using LiveNewsFeed.UI.UWP.Managers.Settings;
using LiveNewsFeed.UI.UWP.Services;
using LiveNewsFeed.UI.UWP.Views;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public class NewsFeedPageViewModel : ViewModelBase
    {
        private static class ViewFilters
        {
            public static readonly Predicate<NewsArticlePostViewModel> ShowOnlyImportantPostsFilter = viewModel => viewModel.IsImportant;
        }

        private readonly ILogger<NewsFeedPageViewModel>? _logger;

        private readonly IDataSourcesManager _dataSourcesManager;
        private readonly INavigationService _navigationService;
        private readonly ISettingsManager _settingsManager;
        private readonly ILiveTileService _liveTileService;
        private readonly INotificationsManager _notificationsManager;
        private readonly IAutomaticUpdater _automaticUpdater;

        private readonly ObservableCollection<NewsArticlePostViewModel> _articlePosts;
        private readonly ISet<Predicate<NewsArticlePostViewModel>> _articlePostsViewFilters;

        private bool _arePostsLoadingAutomatically;
        public bool ArePostsLoadingAutomatically
        {
            get => _arePostsLoadingAutomatically;
            set
            {
                var changed = SetProperty(ref _arePostsLoadingAutomatically, value);

                if (changed)
                    ReevaluateCommands();
            }
        }

        private bool _arePostsLoadingManually;
        public bool ArePostsLoadingManually
        {
            get => _arePostsLoadingManually;
            set
            {
                var changed = SetProperty(ref _arePostsLoadingManually, value);

                if (changed)
                    ReevaluateCommands();
            }
        }
        
        public SettingsMenuViewModel SettingsMenu { get; }

        private NewsArticlePostViewModel? _selectedPost;
        public NewsArticlePostViewModel? SelectedPost
        {
            get => _selectedPost;
            set => SetProperty(ref _selectedPost, value);
        }

        public AdvancedCollectionView ArticlePosts { get; private set; }

        public IAsyncRelayCommand RefreshNewsFeedCommand { get; private set; }

        public IAsyncRelayCommand LoadOlderPostsCommand { get; private set; }

        public NewsFeedPageViewModel(IDataSourcesManager dataSourcesManager,
                                     INavigationService navigationService,
                                     INotificationsManager notificationsManager,
                                     ISettingsManager settingsManager,
                                     ILiveTileService liveTileService,
                                     IAutomaticUpdater automaticUpdater,
                                     SettingsMenuViewModel settingsMenuViewModel,
                                     ILogger<NewsFeedPageViewModel>? logger = default)
        {
            _dataSourcesManager = dataSourcesManager ?? throw new ArgumentNullException(nameof(dataSourcesManager));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _notificationsManager = notificationsManager ?? throw new ArgumentNullException(nameof(notificationsManager));
            _settingsManager = settingsManager ?? throw new ArgumentNullException(nameof(settingsManager));
            _liveTileService = liveTileService ?? throw new ArgumentNullException(nameof(liveTileService));
            _automaticUpdater = automaticUpdater ?? throw new ArgumentNullException(nameof(automaticUpdater));
            SettingsMenu = settingsMenuViewModel ?? throw new ArgumentNullException(nameof(settingsMenuViewModel));
            _logger = logger;

            _articlePosts = new ObservableCollection<NewsArticlePostViewModel>();
            _articlePostsViewFilters = new HashSet<Predicate<NewsArticlePostViewModel>>();

            SetArticlePostsView();
            RegisterEventHandlers();
            InitializeCommands();
            LoadPosts();
            StartUpdater();
        }


        private void SetArticlePostsView()
        {
            ArticlePosts = new AdvancedCollectionView(_articlePosts);
            // set sorting description
            ArticlePosts.SortDescriptions.Add(new SortDescription(nameof(NewsArticlePostViewModel.PublishTime), SortDirection.Descending));
            // set filter conditions
            ArticlePosts.Filter = obj =>
            {
                if (obj is not NewsArticlePostViewModel articlePost)
                    return true;

                return _articlePostsViewFilters.Count == 0 || _articlePostsViewFilters.Aggregate(true,
                    (current, filter) => current && filter.Invoke(articlePost));
            };

            if (_settingsManager.NewsFeedDisplaySettings.ShowOnlyImportantPosts)
                _articlePostsViewFilters.Add(ViewFilters.ShowOnlyImportantPostsFilter);
        }

        private void RegisterEventHandlers()
        {
            _dataSourcesManager.NewsArticlePostReceived += DataSourcesManager_OnNewsArticlePostReceived;
            _settingsManager.ApplicationSettings.SettingChanged += Settings_OnChanged;
            _settingsManager.NewsFeedDisplaySettings.SettingChanged += Settings_OnChanged;
        }

        private async Task LoadPosts()
        {
            ArePostsLoadingAutomatically = true;

            var posts = await _dataSourcesManager.GetLatestPostsFromAllAsync(GetCurrentOptions());

            using (ArticlePosts.DeferRefresh())
            {
                foreach (var post in posts)
                {
                    var viewModel = new NewsArticlePostViewModel(post);

                    if (_articlePosts.Contains(viewModel)) 
                        continue;

                    RegisterEvents(viewModel);

                    _articlePosts.Add(viewModel);

                    if (post.Image != null)
                        _liveTileService.UpdateLiveTile(post, true);
                }
            }

            ArePostsLoadingAutomatically = false;
        }

        private void StartUpdater()
        {
            _automaticUpdater.AutomaticUpdateRequested += (_, _) =>
            {
                _logger?.LogDebug("Automatic news feeds update initiated...");

                _dataSourcesManager.LoadLatestPostsSinceLastUpdateAsync(GetCurrentOptions());
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
            LoadOlderPostsCommand = new AsyncRelayCommand(LoadOlderPosts, CanLoadOlderPosts);
        }
        
        private void ReevaluateCommands()
        {
            RefreshNewsFeedCommand.NotifyCanExecuteChanged();
            LoadOlderPostsCommand.NotifyCanExecuteChanged();
        }

        private async Task ReloadArticlesManually()
        {
            _logger?.LogDebug("Manual News Feed refresh requested.");

            ArePostsLoadingManually = true;

            var posts = await _dataSourcesManager.GetLatestPostsSinceLastUpdateAsync(GetCurrentOptions());

            foreach (var post in posts)
            {
                var viewModel = new NewsArticlePostViewModel(post);

                if (_articlePosts.Contains(viewModel))
                    continue;
                
                RegisterEvents(viewModel);

                _articlePosts.Add(viewModel);

                // show notification
                _notificationsManager.ShowNotification(post);

                if (post.Image != null)
                    _liveTileService.UpdateLiveTile(post, true);
            }

            ArePostsLoadingManually = false;
        }

        private bool CanReloadArticlesManually() => !ArePostsLoadingManually;

        private async Task LoadOlderPosts()
        {
            _logger?.LogDebug("Loading of older posts requested.");

            ArePostsLoadingManually = true;

            var posts = await _dataSourcesManager.GetOlderPostsFromAllAsync(GetCurrentOptions());

            using (ArticlePosts.DeferRefresh())
            {
                foreach (var post in posts)
                {
                    var viewModel = new NewsArticlePostViewModel(post);

                    if (_articlePosts.Contains(viewModel))
                        continue;

                    RegisterEvents(viewModel);

                    _articlePosts.Add(viewModel);
                }
            }

            ArePostsLoadingManually = false;
        }

        private bool CanLoadOlderPosts() => !ArePostsLoadingManually;

        private void DataSourcesManager_OnNewsArticlePostReceived(object sender, NewsArticlePost newsArticlePost)
        {
            InvokeOnUi(() =>
            {
                var viewModel = new NewsArticlePostViewModel(newsArticlePost);

                if (_articlePosts.Contains(viewModel))
                    return;

                RegisterEvents(viewModel);

                _articlePosts.Add(viewModel);
                
                // show notification
                _notificationsManager.ShowNotification(newsArticlePost);

                // update live tile
                if (newsArticlePost.Image != null)
                    _liveTileService.UpdateLiveTile(newsArticlePost);
            });
        }

        private void Settings_OnChanged(object sender, SettingChangedEventArgs eventArgs)
        {
            switch (eventArgs.SettingName)
            {
                case nameof(ApplicationSettings.DisplayLanguageCode):
                    ChangeLanguage(eventArgs.GetNewValue<string>());
                    break;

                case nameof(NewsFeedDisplaySettings.ShowOnlyImportantPosts):
                    switch (eventArgs.GetNewValue<bool>())
                    {
                        case true:
                            _articlePostsViewFilters.Add(ViewFilters.ShowOnlyImportantPostsFilter);
                            ArticlePosts.RefreshFilter();
                            break;

                        case false:
                            _articlePostsViewFilters.Remove(ViewFilters.ShowOnlyImportantPostsFilter);
                            ArticlePosts.RefreshFilter();
                            break;
                    }
                    break;
            }
        }

        private void ChangeLanguage(string? languageCode)
        {
            if (languageCode == null)
                return;

            ApplicationLanguages.PrimaryLanguageOverride = languageCode;
            ResourceContext.GetForCurrentView().Reset();
            ResourceContext.GetForViewIndependentUse().Reset();

            _logger?.LogInformation($"App language switched to {CultureInfo.GetCultureInfo(languageCode).EnglishName}");
            
            _navigationService.NavigateTo<NewsFeedPage>(tempDisableCache: true);
        }

        private DataSourceUpdateOptions GetCurrentOptions() => new()
        {
            Important = _settingsManager.NewsFeedDisplaySettings.ShowOnlyImportantPosts,
            Count = 20
        };
    }
}