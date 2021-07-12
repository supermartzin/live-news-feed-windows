using System;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Data.Html;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Microsoft.Toolkit.Uwp.Notifications;

using LiveNewsFeed.Models;

using LiveNewsFeed.UI.UWP.Common;
using LiveNewsFeed.UI.UWP.Managers.Settings;

namespace LiveNewsFeed.UI.UWP.Managers
{
    public class NotificationsManager : INotificationsManager
    {
        private static readonly ResourceLoader Localization = ResourceLoader.GetForViewIndependentUse();

        private readonly ISettingsManager _settingsManager;
        private readonly IThemeManager _themeManager;

        public NotificationSettings Settings => _settingsManager.NotificationSettings;

        public Dictionary<string, NewsArticlePost> NotifiedPosts { get; }

        public NotificationsManager(ISettingsManager settingsManager,
                                    IThemeManager themeManager)
        {
            _settingsManager = settingsManager ?? throw new ArgumentNullException(nameof(settingsManager));
            _themeManager = themeManager ?? throw new ArgumentNullException(nameof(themeManager));
            
            NotifiedPosts = new Dictionary<string, NewsArticlePost>();
        }

        public void ShowNotification(NewsArticlePost articlePost)
        {
            if (articlePost == null)
                throw new ArgumentNullException(nameof(articlePost));

            if (!Settings.NotificationsAllowed)
                return;
            if (Settings.NotifyOnlyOnImportantPosts && !articlePost.IsImportant)
                return;

            var notificationId = Guid.NewGuid().ToString();

            var notificationBuilder = new ToastContentBuilder().AddAppLogoOverride(GetNewsFeedLogo(articlePost))
                                                               .AddToastActivationInfo($"action=showPost&notifId={notificationId}", ToastActivationType.Foreground)
                                                               .AddText(articlePost.Title)
                                                               .AddCustomTimeStamp(articlePost.PublishTime);

            // add buttons
            notificationBuilder.AddButton(Localization.GetString("Notification_Buttons_OpenInBrowser"),
                                          ToastActivationType.Protocol,
                                          articlePost.FullArticleUrl.AbsoluteUri,
                                          GetButtonIcon("Assets/NotificationIcons/OpenExternalIcon.png"));
            notificationBuilder.AddButton(Localization.GetString("Notification_Buttons_CopyLinkToClipboard"),
                                          ToastActivationType.Foreground,
                                          $"action=copyLink&notifId={notificationId}",
                                          GetButtonIcon("Assets/NotificationIcons/CopyIcon.png"));
            notificationBuilder.AddButton(Localization.GetString("Notification_Buttons_SharePost"),
                                          ToastActivationType.Foreground,
                                          $"action=sharePost&notifId={notificationId}",
                                          GetButtonIcon("Assets/NotificationIcons/ShareIcon.png"));
            notificationBuilder.AddButton(new ToastButtonDismiss(Localization.GetString("Notification_Buttons_Dismiss"))
                                          {
                                              ImageUri = GetButtonIcon("Assets/NotificationIcons/DismissIcon.png").OriginalString
                                          });


            // build Categories and Tags text
            var categoryTagText = string.Empty;
            if (articlePost.Categories.Count > 0)
                categoryTagText += string.Join(" | ", articlePost.Categories.Select(category => Localization.GetString($"CategoryEnum_{category}"))) + " | ";
            if (articlePost.Tags.Count > 0)
                categoryTagText += string.Join(" | ", articlePost.Tags.Select(tag => tag.Name));
            if (!string.IsNullOrWhiteSpace(categoryTagText))
            {
                if (categoryTagText.Length >= 35)
                    categoryTagText = categoryTagText.Substring(0, 30) + "...";

                notificationBuilder.AddText(categoryTagText.Trim('|', ' '), hintMaxLines: 1);
            }

            if (articlePost.Image != null)
                notificationBuilder.AddHeroImage(articlePost.Image.Url);

            // add Content text
            notificationBuilder.AddText(HtmlUtilities.ConvertToText(articlePost.Content).Trim(),
                                        hintWrap: true,
                                        hintAlign: AdaptiveTextAlign.Left);

            var notification = notificationBuilder.GetToastContent();

            // add to notified posts collection
            NotifiedPosts[notificationId] = articlePost;

            ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(notification.GetXml()));
        }


        private Uri? GetNewsFeedLogo(NewsArticlePost articlePost) => Helpers.GetLogoForNewsFeed(articlePost.NewsFeedName, _themeManager.CurrentApplicationTheme);

        private static Uri GetButtonIcon(string iconPath)
        {
            var themeExtension = Application.Current.RequestedTheme == ApplicationTheme.Dark ? "-Light" : "-Dark";
            var index = iconPath.IndexOf(".png", StringComparison.InvariantCulture);

            return new Uri(iconPath.Substring(0, index) + themeExtension + ".png", UriKind.Relative);
        }
    }
}