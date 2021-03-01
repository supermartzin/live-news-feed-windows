using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;

using LiveNewsFeed.UI.UWP.Common;
using LiveNewsFeed.UI.UWP.Managers;
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
            if (eventArgs.Parameter is NavigationParameters parameters)
            {
                ViewModel.IsSocialPostPreview = parameters.GetValue<bool>("isSocialPost")!;
                ViewModel.NewsArticlePost = parameters.GetValue<NewsArticlePostViewModel>("post")!;
            }
            
            Window.Current.SetTitleBar(ArticlePreviewTopBar);

            base.OnNavigatedTo(eventArgs);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            StartDisposingWebView();

            base.OnNavigatedFrom(e);
        }


        private void StartDisposingWebView()
        {
            var count = 0;
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(20) };
            timer.Tick += (_, _) =>
            {
                if (count++ == 10)
                {
                    timer.Stop();
                }

                WebView.Source = new Uri("about:blank");
            };

            timer.Start();
        }
    }
}
