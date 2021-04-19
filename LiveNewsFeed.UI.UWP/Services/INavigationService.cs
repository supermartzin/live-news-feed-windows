using System;

namespace LiveNewsFeed.UI.UWP.Services
{
    public interface INavigationService
    {
        Type? CurrentPage { get; }

        bool CanGoBack { get; }

        void GoBack();

        void NavigateTo<T>(object? parameter = default);
    }
}