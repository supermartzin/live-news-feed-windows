using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

using LiveNewsFeed.UI.UWP.Managers;
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

            AddCoreModules(serviceCollection);
            AddServices(serviceCollection);
            AddViewModels(serviceCollection);

            Container = serviceCollection.BuildServiceProvider();

            _isInitialized = true;
        }


        private static void AddCoreModules(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<HttpClient>();
        }

        private static void AddServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IDataSourcesManager, DataSourcesManager>();
        }

        private static void AddViewModels(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<NewsFeedPageViewModel>();
        }
    }
}