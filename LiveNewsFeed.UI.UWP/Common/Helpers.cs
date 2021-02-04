using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Media;

namespace LiveNewsFeed.UI.UWP.Common
{
    internal static class Helpers
    {
        private static readonly Dictionary<string, ImageBrush> NewsFeedLogos = new();

        public static ImageBrush? GetLogoForNewsFeed(string newsFeedName)
        {
            if (newsFeedName == null)
                throw new ArgumentNullException(nameof(newsFeedName));

            return NewsFeedLogos.ContainsKey(newsFeedName) ? NewsFeedLogos[newsFeedName] : default;
        }

        public static void RegisterLogoForNewsFeed(string newsFeedName, ImageBrush logo)
        {
            if (newsFeedName == null)
                throw new ArgumentNullException(nameof(newsFeedName));

            NewsFeedLogos[newsFeedName] = logo ?? throw new ArgumentNullException(nameof(logo));
        }
    }
}