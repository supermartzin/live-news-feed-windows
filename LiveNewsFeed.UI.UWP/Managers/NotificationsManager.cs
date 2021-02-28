﻿using System;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Data.Html;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Toolkit.Uwp.Notifications;

using LiveNewsFeed.Models;

using LiveNewsFeed.UI.UWP.Common;

namespace LiveNewsFeed.UI.UWP.Managers
{
    public class NotificationsManager : INotificationsManager
    {
        private static readonly ResourceLoader Localization = ResourceLoader.GetForViewIndependentUse();

        public NotificationSettings Settings { get; } = new();
        
        public void ShowNotification(NewsArticlePost articlePost)
        {
            if (articlePost == null)
                throw new ArgumentNullException(nameof(articlePost));

            if (!Settings.NotificationsAllowed)
                return;
            if (Settings.OnlyImportantPosts && !articlePost.IsImportant)
                return;

            var notificationBuilder = new ToastContentBuilder().AddAppLogoOverride(GetNewsFeedLogo(articlePost))
                                                               .AddText(articlePost.Title)
                                                               .AddCustomTimeStamp(articlePost.PublishTime)
                                                               .AddButton(Localization.GetString("Notification_Buttons_OpenInBrowser"),
                                                                          ToastActivationType.Protocol,
                                                                          articlePost.FullArticleUrl.AbsoluteUri)
                                                               .AddButton(new ToastButtonDismiss(Localization.GetString("Notification_Buttons_Dismiss")));

            // build Categories and Tags text
            var categoryTagText = string.Empty;
            if (articlePost.Categories.Count > 0)
                categoryTagText += string.Join(" | ", articlePost.Categories.Select(category => Localization.GetString($"CategoryEnum_{category}"))) + " | ";
            if (articlePost.Tags.Count > 0)
                categoryTagText += string.Join(" | ", articlePost.Tags.Select(tag => tag.Name));
            if (!string.IsNullOrWhiteSpace(categoryTagText))
                notificationBuilder.AddText(categoryTagText.Trim('|', ' '), hintMaxLines: 1);

            if (articlePost.Image != null)
                notificationBuilder.AddHeroImage(articlePost.Image.Url);

            // add Content text
            notificationBuilder.AddText(HtmlUtilities.ConvertToText(articlePost.Content).Trim(),
                                        hintWrap: true,
                                        hintAlign: AdaptiveTextAlign.Left);

            var notification = notificationBuilder.GetToastContent();

            ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(notification.GetXml()));
        }


        private static Uri? GetNewsFeedLogo(NewsArticlePost articlePost)
        {
            var imageSource = (BitmapImage) Helpers.GetLogoForNewsFeed(articlePost.NewsFeedName)!;

            return imageSource?.UriSource;
        }
    }
}