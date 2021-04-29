namespace LiveNewsFeed.UI.UWP.Managers
{
    public class ThemeChangedEventArgs : System.EventArgs
    {
        public Theme Theme { get; }

        public ThemeChangedEventArgs(Theme theme)
        {
            Theme = theme;
        }
    }
}