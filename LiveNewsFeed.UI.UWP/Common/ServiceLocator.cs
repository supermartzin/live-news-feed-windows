using System;
using System.Net.Http;
using GalaSoft.MvvmLight.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

using LiveNewsFeed.DataSource.DenikNcz;
using LiveNewsFeed.DataSource.DennikNsk;

using LiveNewsFeed.UI.UWP.Managers;
using LiveNewsFeed.UI.UWP.ViewModels;
using LiveNewsFeed.UI.UWP.Views;

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

            AddLogging(serviceCollection);
            AddCoreModules(serviceCollection);
            AddServices(serviceCollection);
            AddViewModels(serviceCollection);

            Container = serviceCollection.BuildServiceProvider();

            _isInitialized = true;
        }


        private static void AddCoreModules(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<HttpClient>();
            serviceCollection.AddSingleton<DennikNskNewsFeed>();
            serviceCollection.AddSingleton<DenikNczNewsFeed>();
        }

        private static void AddServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IDataSourcesManager, DataSourcesManager>();

            var navigationService = new NavigationService();
            navigationService.Configure(nameof(NewsFeedPage), typeof(NewsFeedPage));
            navigationService.Configure(nameof(ArticlePreviewPage), typeof(ArticlePreviewPage));
            serviceCollection.AddSingleton<INavigationService>(navigationService);
            
            serviceCollection.AddSingleton<INotificationsManager, NotificationsManager>();
            serviceCollection.AddSingleton<ISettingsManager, UserFileSettingsManager>();
            serviceCollection.AddSingleton<IAutomaticUpdater, AutomaticUpdater>();
        }

        private static void AddViewModels(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<NewsFeedPageViewModel>();
            serviceCollection.AddTransient<QuickSettingsViewModel>();
            serviceCollection.AddTransient<ArticlePreviewPageViewModel>();
        }

        private static void AddLogging(IServiceCollection serviceCollection)
        {
            serviceCollection.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddNLog();
            });
        }
    }
}