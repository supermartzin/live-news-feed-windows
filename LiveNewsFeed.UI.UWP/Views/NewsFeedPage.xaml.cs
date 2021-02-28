﻿using System;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using LiveNewsFeed.UI.UWP.Common;
using LiveNewsFeed.UI.UWP.ViewModels;

namespace LiveNewsFeed.UI.UWP.Views
{
    public sealed partial class NewsFeedPage : BasePage
    {
        private const float DefaultImageZoomFactor = 0.9f;

        public NewsFeedPageViewModel ViewModel { get; }
        
        public NewsFeedPage()
        {
            ViewModel = ServiceLocator.Container.GetRequiredService<NewsFeedPageViewModel>();

            InitializeComponent();

            SetTitleBarButtonColors();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Window.Current.SetTitleBar(NewsFeedTitleBar);

            base.OnNavigatedTo(e);
        }

        protected override void OnApplicationThemeChanged(ApplicationTheme theme)
        {
            SetTitleBarButtonColors();
        }
        

        private void ImagePreviewOpened()
        {
            Window.Current.SetTitleBar(ImagePreviewTitleBar);
        }

        private void ImagePreviewClosed()
        {
            BitmapImage.UriSource = null;

            SetImagePreviewZoom(DefaultImageZoomFactor);

            Window.Current.SetTitleBar(NewsFeedTitleBar);
        }

        private void SetImagePreviewZoom(float zoomFactor)
        {
            ZoomPanel.ChangeView(0, 0, zoomFactor);
        }

        private void SetTitleBarButtonColors()
        {
            // set buttons foreground
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonForegroundColor = titleBar.ButtonHoverForegroundColor = (Color?) Application.Current.Resources["TitleBarButtonsForegroundColor"];
        }

        #region Event handlers
        
        private async void Expander_OnExpanded(object sender, EventArgs e)
        {
            if (sender is Expander expander && expander?.Content is WebView webView)
            {
                await AdjustSocialPostElementHeight(webView).ConfigureAwait(true);
            }
        }

        private void Expander_OnCollapsed(object sender, EventArgs e)
        {
            var expander = sender as Expander;

            if (expander?.Content is WebView webView)
            {
                webView.Refresh();
            }
        }

        private void CloseImagePreviewButton_OnClick(object sender, RoutedEventArgs e)
        {
            ImagePreviewClosed();
        }
        
        private void ZoomPanel_OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            SetImagePreviewZoom(DefaultImageZoomFactor);
        }

        private void ArticlePostImageThumbnail_OnPointerPressed(object sender, PointerRoutedEventArgs eventArgs)
        {
            ImagePreviewOpened();
        }

        private void BitmapImage_OnImageOpened(object sender, RoutedEventArgs e)
        {
            SetImagePreviewZoom(DefaultImageZoomFactor);
        }

        private void BitmapImage_OnImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            var logger = ServiceLocator.Container.GetService<ILogger<NewsFeedPage>>();
            
            logger.LogError($"Error loading Article image: {e.ErrorMessage}");
        }

        #endregion
        
        private static async Task AdjustSocialPostElementHeight(WebView webView)
        {
            await Task.Delay(400).ConfigureAwait(true);

            var result = await webView.InvokeScriptAsync("eval", new[] { "document.body.scrollHeight.toString()" });

            if (result != null && int.TryParse(result, out var height))
            {
                webView.Height = height;
                webView.MaxHeight = height;
                webView.MinHeight = height;
            }
        }
    }
}