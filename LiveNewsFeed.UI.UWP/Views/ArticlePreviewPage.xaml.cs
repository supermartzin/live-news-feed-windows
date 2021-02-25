using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;

using LiveNewsFeed.UI.UWP.Common;
using LiveNewsFeed.UI.UWP.ViewModels;

namespace LiveNewsFeed.UI.UWP.Views
{
    public sealed partial class ArticlePreviewPage : BasePage
    {
        public ArticlePreviewPageViewModel ViewModel { get; }

        public ArticlePreviewPage()
        {
            ViewModel = ServiceLocator.Container.GetRequiredService<ArticlePreviewPageViewModel>();

            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs eventArgs)
        {
            if (eventArgs.Parameter is NewsArticlePostViewModel post)
            {
                ViewModel.NewsArticlePost = post;
            }
            
            Window.Current.SetTitleBar(ArticlePreviewTopBar);

            base.OnNavigatedTo(eventArgs);
        }
    }
}
