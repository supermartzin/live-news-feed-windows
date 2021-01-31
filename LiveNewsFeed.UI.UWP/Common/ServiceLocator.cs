using System;
using Microsoft.Extensions.DependencyInjection;

using LiveNewsFeed.DataSource.Contract;
using LiveNewsFeed.DataSource.DennikNsk;

using LiveNewsFeed.UI.UWP.ViewModels;

namespace LiveNewsFeed.UI.UWP.Common
{
    public static class ServiceLocator
    {
        private static bool _isInitialized;

        public static IServiceProvider Container { get; private set; }

        public static void Initialize()
        {
            if (_isInitialized)
                return;

            var serviceCollection = new ServiceCollection();

            AddServices(serviceCollection);
            AddViewModels(serviceCollection);

            Container = serviceCollection.BuildServiceProvider();

            _isInitialized = true;
        }


        private static void AddServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<INewsFeedDataSource, SkDennikNDataSource>();
        }

        private static void AddViewModels(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<MainPageViewModel>();
        }
    }
}