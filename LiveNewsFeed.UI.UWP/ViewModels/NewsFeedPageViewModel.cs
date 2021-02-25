using System;
using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
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
                                     QuickSettingsViewModel quickSettingsViewModel)
        {
            _dataSourcesManager = dataSourcesManager ?? throw new ArgumentNullException(nameof(dataSourcesManager));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _quickSettings = quickSettingsViewModel ?? throw new ArgumentNullException(nameof(quickSettingsViewModel));

            ArticlePosts = new AdvancedCollectionView(new List<NewsArticlePostViewModel>(), true);
            ArticlePosts.SortDescriptions.Add(new SortDescription(nameof(NewsArticlePostViewModel.PublishTime), SortDirection.Descending));

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

                                   using (ArticlePosts.DeferRefresh())
                                   {
                                       foreach (var post in posts)
                                       {
                                           ArticlePosts.Add(post);
                                       }
                                   }

                                   AllPostsLoading = false;
                               }));
        }

        private void RegisterEvents(IEnumerable<NewsArticlePostViewModel> posts)
        {
            foreach (var post in posts)
            {
                post.ShowImagePreviewRequested += NewsArticlePost_OnShowImagePreviewRequested;
                post.HideImagePreviewRequested += NewsArticlePost_OnHideImagePreviewRequested;
                post.OpenArticlePreviewRequested += NewsArticlePost_OnOpenArticlePreviewRequested;
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
                               .ContinueWith(task => InvokeOnUi(() =>
                               {
                                   var newPosts = task.Result
                                                                                 .Select(Helpers.ToViewModel)
                                                                                 .ToList();
                                   
                                   RegisterEvents(newPosts);

                                   if (newPosts.Count > 0)
                                   {
                                       using (ArticlePosts.DeferRefresh())
                                       {
                                           foreach (var post in newPosts)
                                           {
                                               ArticlePosts.Add(post);
                                           }
                                       }
                                   }

                                   NewPostsLoading = false;
                               }));
        }

        private bool CanReloadArticlesManually() => !NewPostsLoading;
    }
}