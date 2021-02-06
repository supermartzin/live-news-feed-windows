using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
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

        protected virtual string GetLocalizedString(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var resourceLoader = ResourceLoader.GetForViewIndependentUse();

            return resourceLoader.GetString(key);
        }
    }
}