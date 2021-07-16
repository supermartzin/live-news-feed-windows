using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public class ApplicationInfoViewModel : ViewModelBase
    {
        private string _applicationName;
        public string ApplicationName
        {
            get => _applicationName;
            set => SetProperty(ref _applicationName, value);
        }

        private string _version;
        public string Version
        {
            get => _version;
            set => SetProperty(ref _version, value);
        }

        private string _creator;
        public string Creator
        {
            get => _creator;
            set => SetProperty(ref _creator, value);
        }

        private DateTime _buildDate;
        public DateTime BuildDate
        {
            get => _buildDate;
            set => SetProperty(ref _buildDate, value);
        }

        public ApplicationInfoViewModel()
        {
            LoadInfo();
        }


        private async Task LoadInfo()
        {
            ApplicationName = AppInfo.Current.DisplayInfo.DisplayName;

            var version = Package.Current.Id.Version;
            Version = $"v{version.Major}.{version.Minor}.{version.Build}";

            Creator = Package.Current.Id.Publisher.Remove(0, 3);

            var buildDate = await GetBuildTimestampAsync().ConfigureAwait(false);
            await InvokeOnUiAsync(() => BuildDate = buildDate);
        }
        
        private static async Task<DateTime> GetBuildTimestampAsync()
        {
            var manifest = await Package.Current.InstalledLocation.GetFileAsync("AppxManifest.xml");
            var properties = await manifest.GetBasicPropertiesAsync();

            return properties.DateModified.DateTime;
        }
    }
}