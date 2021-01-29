using System;
using System.Net.Http;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using LiveNewsFeed.DataSource.DennikNsk;

namespace LiveNewsFeed.UI.UWP
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var dataSource = new SkDennikNDataSource(new HttpClient());

            await dataSource.GetPostsAsync(important: true).ConfigureAwait(false);

            Console.WriteLine("Done.");
        }
    }
}
