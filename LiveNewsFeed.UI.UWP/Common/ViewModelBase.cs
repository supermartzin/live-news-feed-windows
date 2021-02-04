using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace LiveNewsFeed.UI.UWP.Common
{
    public class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase
    {
        protected virtual async Task InvokeOnUi(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var dispatcher = Window.Current?.Dispatcher ?? CoreApplication.MainView.Dispatcher;

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, action.Invoke);
        }
    }
}