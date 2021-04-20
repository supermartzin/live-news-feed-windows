﻿using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

using LiveNewsFeed.DataSource.DenikNcz;
using LiveNewsFeed.DataSource.DennikNsk;

using LiveNewsFeed.UI.UWP.Managers;
using LiveNewsFeed.UI.UWP.Services;
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
            serviceCollection.AddSingleton<INavigationService, NavigationService>();
            serviceCollection.AddSingleton<INotificationsManager, NotificationsManager>();
            serviceCollection.AddSingleton<ISettingsManager, LocalSettingsManager>();
            serviceCollection.AddSingleton<IAutomaticUpdater, AutomaticUpdater>();
        }

        private static void AddViewModels(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<NewsFeedPageViewModel>();
            serviceCollection.AddTransient<QuickPanelSettingsViewModel>();
            serviceCollection.AddTransient<SettingsMenuViewModel>();
            serviceCollection.AddTransient<ArticlePreviewPageViewModel>();
        }

        private static void AddLogging(IServiceCollection serviceCollection)
        {
            serviceCollection.AddLogging(builder =>
            {
                builder.AddNLog();
                builder.AddSentry(options =>
                {
                    options.Dsn = "https://0f8a0d0187f9497ebc608603ec352c88@o504575.ingest.sentry.io/5709957";
                    options.MinimumEventLevel = LogLevel.Warning;
                    options.InitializeSdk = true;
                });

                // set Minimum log level based on variable in NLog.config --> default == INFO
                var minLevelVariable = LogManager.Configuration.Variables["minLogLevel"].OriginalText;
                if (Enum.TryParse(minLevelVariable, true, out LogLevel minLevel))
                    builder.SetMinimumLevel(minLevel);
            });
        }
    }
}