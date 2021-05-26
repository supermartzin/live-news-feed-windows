using Windows.UI;

namespace LiveNewsFeed.UI.UWP.Managers
{
    public class ColorChangedEventArgs : System.EventArgs
    {
        public Color Color { get; }

        public ColorChangedEventArgs(Color color)
        {
            Color = color;
        }
    }
}