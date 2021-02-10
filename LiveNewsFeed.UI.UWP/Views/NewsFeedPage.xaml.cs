using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.Extensions.DependencyInjection;

using LiveNewsFeed.UI.UWP.Common;
using LiveNewsFeed.UI.UWP.ViewModels;

namespace LiveNewsFeed.UI.UWP.Views
{
    public sealed partial class NewsFeedPage : Page
    {
        public NewsFeedPageViewModel ViewModel => ServiceLocator.Container.GetRequiredService<NewsFeedPageViewModel>();

        public NewsFeedPage()
        {
            InitializeComponent();

            SetTitleBarProperties();
        }


        private void SetTitleBarProperties()
        {
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            Window.Current.SetTitleBar(NewsFeedTitleBar);

            // set buttons foreground
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonForegroundColor = (Color?) Application.Current.Resources["TitleBarButtonsForegroundColor"];
        }


        private async void Expander_OnExpanded(object sender, EventArgs e)
        {
            var expander = sender as Expander;

            if (expander?.Content is WebView webView)
            {
                await Task.Delay(200).ConfigureAwait(true);

                var result = await webView.InvokeScriptAsync("eval", new[] { "document.body.scrollHeight.toString()" });

                if (result != null && int.TryParse(result, out int height))
                {
                    webView.Height = height;
                    webView.MaxHeight = height;
                    webView.MinHeight = height;
                }
            }
        }
    }
}