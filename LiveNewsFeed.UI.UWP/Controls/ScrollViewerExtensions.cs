using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace LiveNewsFeed.UI.UWP.Controls
{
    [Bindable]
    public class ScrollViewerExtensions : DependencyObject
    {
        public static readonly DependencyProperty VerticalScrollOffsetProperty = DependencyProperty.RegisterAttached("VerticalScrollOffset", typeof(double), typeof(ScrollViewerExtensions), new PropertyMetadata(0.0));

        public static double GetVerticalScrollOffset(GridView gridView) => (double) gridView.GetValue(VerticalScrollOffsetProperty);

        public static void SetVerticalScrollOffset(GridView gridView, double value) => gridView.SetValue(VerticalScrollOffsetProperty, value);
    }
}