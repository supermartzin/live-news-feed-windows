using System;
using System.IO;
using System.Reflection;
using Windows.ApplicationModel;

namespace LiveNewsFeed.UI.UWP
{
    public static class ApplicationInfo
    {
        public static string ApplicationName => AppInfo.Current.DisplayInfo.DisplayName;

        public static string AppVersion => GetVersion();

        public static string Author => Package.Current.Id.Publisher.Remove(0, 3);

        public static DateTime BuildDate => GetBuildDate();
        
        
        private static string GetVersion()
        {
            var version = Package.Current.Id.Version;

            return $"v{version.Major}.{version.Minor}.{version.Build}";
        }

        private static DateTime GetBuildDate()
        {
            var filePath = Assembly.GetExecutingAssembly().Location;
            const int cPeHeaderOffset = 60;
            const int cLinkerTimestampOffset = 8;

            var buffer = new byte[2048];

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                stream.Read(buffer, 0, 2048);

            var offset = BitConverter.ToInt32(buffer, cPeHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(buffer, offset + cLinkerTimestampOffset);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var linkTimeUtc = epoch.AddSeconds(secondsSince1970);
            
            return TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, TimeZoneInfo.Local);
        }
    }
}