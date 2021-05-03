using System;
using Windows.ApplicationModel;

namespace LiveNewsFeed.UI.UWP
{
    public static class ApplicationInfo
    {
        public static string ApplicationName => AppInfo.Current.DisplayInfo.DisplayName;

        public static string AppVersion => GetVersion();

        public static string Author => Package.Current.Id.Publisher.Remove(0, 3);

        public static DateTime BuildDate => new(2021, 5, 3);

        private static string GetVersion()
        {
            var version = Package.Current.Id.Version;

            return $"v{version.Major}.{version.Minor}.{version.Build}";
        }
    }
}