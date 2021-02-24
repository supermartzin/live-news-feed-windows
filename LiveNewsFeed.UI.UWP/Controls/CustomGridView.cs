using Windows.UI.Xaml.Controls;

namespace LiveNewsFeed.UI.UWP.Controls
{
    public class CustomGridView : GridView
    {
        private ScrollViewer? _scrollViewer;

        public CustomGridView()
        {
            DefaultStyleKey = typeof(GridView);
        }

        public void ScrollToTop()
        {
            _scrollViewer?.ChangeView(0, 0, _scrollViewer?.ZoomFactor);
        }

        protected override void OnApplyTemplate()
        {
            _scrollViewer = GetTemplateChild("ScrollViewer") as ScrollViewer;
            if (_scrollViewer != null)
                _scrollViewer.ViewChanged += ScrollViewer_OnViewChanged;

            base.OnApplyTemplate();
        }


        private void ScrollViewer_OnViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            SetValue(ScrollViewerExtensions.VerticalScrollOffsetProperty, _scrollViewer?.VerticalOffset);
        }
    }
}