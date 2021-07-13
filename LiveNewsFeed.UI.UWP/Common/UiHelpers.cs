using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Html;
using Windows.System;

using LiveNewsFeed.Models;

namespace LiveNewsFeed.UI.UWP.Common
{
    public static class UiHelpers
    {
        public static void ShareArticleViaSystemUI(NewsArticlePost articlePost)
        {
            if (articlePost == null)
                throw new ArgumentNullException(nameof(articlePost));

            Helpers.InvokeOnUiAsync(() =>
            {
                DataTransferManager.GetForCurrentView().DataRequested += OnDataRequested;
                DataTransferManager.ShowShareUI();
            });

            void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs eventArgs)
            {
                DataTransferManager.GetForCurrentView().DataRequested -= OnDataRequested;

                // set shared content
                eventArgs.Request.Data.SetWebLink(articlePost.FullArticleUrl);
                eventArgs.Request.Data.Properties.ContentSourceWebLink = articlePost.FullArticleUrl;
                eventArgs.Request.Data.Properties.Title = articlePost.Title;
                eventArgs.Request.Data.Properties.Description = GetContentForSharing(HtmlUtilities.ConvertToText(articlePost.Content).Trim());
                eventArgs.Request.Data.Properties.ApplicationName = Package.Current.DisplayName;
            }
        }

        public static void ShareLinkViaClipboard(Uri url)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            var dataPackage = new DataPackage();
            dataPackage.SetText(url.AbsoluteUri);

            Helpers.InvokeOnUiAsync(() => Clipboard.SetContent(dataPackage));
        }

        public static async Task OpenInDefaultBrowser(Uri? uri) => await Launcher.LaunchUriAsync(uri);

        public static bool Not(bool? value) => value != true;

        public static bool IsNull(object? value) => value is null;

        public static bool IsNotNull(object? value) => value is not null;


        private static string GetContentForSharing(string content) => content.Length > 100
            ? content.Substring(0, 100) + "..."
            : content;
    }
}