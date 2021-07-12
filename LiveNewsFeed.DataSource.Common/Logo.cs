using System;

namespace LiveNewsFeed.DataSource.Common
{
    public class Logo
    {
        public Uri LightThemeUrl { get; }

        public Uri DarkThemeUrl { get; }

        public Logo(Uri lightThemeUrl, Uri darkThemeUrl)
        {
            LightThemeUrl = lightThemeUrl ?? throw new ArgumentNullException(nameof(lightThemeUrl));
            DarkThemeUrl = darkThemeUrl ?? throw new ArgumentNullException(nameof(darkThemeUrl));
        }
    }
}