using System;
using Windows.UI.Xaml.Controls;

namespace LiveNewsFeed.UI.UWP.Controls
{
    public class CustomGridView : GridView
    {
        private ScrollViewer? _scrollViewer;

        public event EventHandler<ScrollViewerViewChangedEventArgs>? ScrollViewChanged;

        public CustomGridView()
        {
            DefaultStyleKey = typeof(GridView);
        }

        protected override void OnApplyTemplate()
        {
            _scrollViewer = GetTemplateChild("ScrollViewer") as ScrollViewer;
            if (_scrollViewer != null)
                _scrollViewer.ViewChanged += (sender, args) => ScrollViewChanged?.Invoke(sender, args);

            base.OnApplyTemplate();
        }
    }
}