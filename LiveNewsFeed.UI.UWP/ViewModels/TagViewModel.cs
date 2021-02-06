using LiveNewsFeed.UI.UWP.Common;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public class TagViewModel : ViewModelBase
    {
        public string Name { get; }

        public TagViewModel(string name)
        {
            Name = name;
        }
    }
}