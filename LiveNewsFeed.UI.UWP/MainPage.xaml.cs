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
            var dataSource = new SkDennikNDataSource("https://dennikn.sk/api/minute", new HttpClient());

            var posts = await dataSource.GetLatestPostsAsync().ConfigureAwait(false);

            Console.WriteLine("Done.");
        }
    }
}
