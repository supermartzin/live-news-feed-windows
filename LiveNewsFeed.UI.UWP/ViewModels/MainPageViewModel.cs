using GalaSoft.MvvmLight;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public string Message { get; set; }

        public MainPageViewModel()
        {
            Message = "Hello world!";
        }
    }
}