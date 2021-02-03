using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Microsoft.Extensions.DependencyInjection;

using LiveNewsFeed.UI.UWP.Common;
using LiveNewsFeed.UI.UWP.ViewModels;

namespace LiveNewsFeed.UI.UWP.Views
{
    public sealed partial class NewsFeedPage : Page
    {
        public NewsFeedPageViewModel ViewModel => ServiceLocator.Container.GetRequiredService<NewsFeedPageViewModel>();

        public NewsFeedPage()
        {
            InitializeComponent();

            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;
        }
    }
}