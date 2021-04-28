using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LiveNewsFeed.UI.UWP.Services
{
    public class NavigationService : INavigationService
    {
        private Frame? _currentFrame;

        public Type? CurrentPage => CurrentFrame.Content?.GetType();

        public Frame CurrentFrame
        {
            get => _currentFrame ??= (Frame) Window.Current.Content;
            set => _currentFrame = value;
        }

        public virtual bool CanGoBack => CurrentFrame.CanGoBack;
        
        public virtual void GoBack()
        {
            if (CanGoBack)
                CurrentFrame.GoBack();
        }
        
        public virtual void NavigateTo<T>(object? parameter = default, bool tempDisableCache = false)
        {
            if (!typeof(Page).IsAssignableFrom(typeof(T)))
                throw new ArgumentOutOfRangeException(nameof(T), $@"Provided type '{typeof(T).Name}' is not assignable to root Page type.");

            var originalCacheSize = CurrentFrame.CacheSize;

            if (tempDisableCache)
                CurrentFrame.CacheSize = 0;

            CurrentFrame.Navigate(typeof(T), parameter);

            if (tempDisableCache)
                CurrentFrame.CacheSize = originalCacheSize;
        }
    }
}