using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.Helpers;

using LiveNewsFeed.UI.UWP.Common;
using LiveNewsFeed.UI.UWP.Managers;

namespace LiveNewsFeed.UI.UWP.Resources
{
    public static class ThemeResources
    {
        private static ApplicationTheme CurrentApplicationTheme => ServiceLocator.Container.GetRequiredService<IThemeManager>().CurrentApplicationTheme;

        private static readonly Dictionary<ApplicationTheme, Dictionary<string, object>> Dictionary = new()
        {
            {
                // Light theme resources
                ApplicationTheme.Light,
                new Dictionary<string, object>
                {
                    {"TitleBarButtonsForegroundColor", Colors.Black},
                    {"CategoryLocalColor", "#22769F".ToColor()},
                    {"CategoryWorldColor", "#915209".ToColor()},
                    {"CategoryEconomyColor", "#422E4D".ToColor()},
                    {"CategorySportColor", "#005627".ToColor()},
                    {"CategoryArtsColor", "#07618D".ToColor()},
                    {"CategoryScienceColor", "#B51675".ToColor()},
                    {"CategoryCommentaryColor", "#BF2134".ToColor()},
                    {"CategoryTravelingColor", "#779e0d".ToColor()}
                }
            },
            {
                // Dark theme resources
                ApplicationTheme.Dark,
                new Dictionary<string, object>
                {
                    {"TitleBarButtonsForegroundColor", Colors.White},
                    {"CategoryLocalColor", "#36BCFF".ToColor()},
                    {"CategoryWorldColor", "#FF9010".ToColor()},
                    {"CategoryEconomyColor", "#B17BCC".ToColor()},
                    {"CategorySportColor", "#00D660".ToColor()},
                    {"CategoryArtsColor", "#0A95D9".ToColor()},
                    {"CategoryScienceColor", "#FF1F86".ToColor()},
                    {"CategoryCommentaryColor", "#FF2C45".ToColor()},
                    {"CategoryTravelingColor", "#b3e036".ToColor()}
                }
            }
        };

        public static T GetAs<T>(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return (T) Dictionary[CurrentApplicationTheme][key];
        }
    }
}