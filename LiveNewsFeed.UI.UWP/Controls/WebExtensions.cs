using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LiveNewsFeed.UI.UWP.Controls
{
    public class WebExtensions : DependencyObject
    {
        public static readonly DependencyProperty HtmlStringProperty = DependencyProperty.RegisterAttached("HtmlString", typeof(string), typeof(WebExtensions), new PropertyMetadata("", OnHtmlStringChanged));

        public static string GetHtmlString(DependencyObject obj) => (string) obj.GetValue(HtmlStringProperty);

        public static void SetHtmlString(DependencyObject obj, string value) => obj.SetValue(HtmlStringProperty, value);

        private static void OnHtmlStringChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (dependencyObject is WebView webView && eventArgs.NewValue != null)
            {
                webView.NavigateToString((string) eventArgs.NewValue);
            }
        }
    }
}