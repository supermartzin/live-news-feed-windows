using System;
using System.Threading.Tasks;
using Sentry;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources.Core;
using Windows.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UnhandledExceptionEventArgs = Windows.UI.Xaml.UnhandledExceptionEventArgs;

using LiveNewsFeed.DataSource.Common;
using LiveNewsFeed.DataSource.DenikNcz;
using LiveNewsFeed.DataSource.DennikNsk;

using LiveNewsFeed.UI.UWP.Common;
using LiveNewsFeed.UI.UWP.Managers;
using LiveNewsFeed.UI.UWP.Views;

namespace LiveNewsFeed.UI.UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        private static readonly ILogger Logger = ServiceLocator.Container.GetRequiredService<ILogger<App>>();

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            SetEventHandlers();
            SetServices();
            InitializeSettings();
            LoadDataSources();
        }
        
        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (Window.Current.Content is not Frame rootFrame)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated)
                return;

            SetLanguage("sk");

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(NewsFeedPage), e.Arguments);
            }

            // Ensure the current window is active
            Window.Current.Activate();

            // extend title bar
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            Helpers.CreateCategoriesMap();
        }
        
        protected override void OnActivated(IActivatedEventArgs eventArgs)
        {
            if (eventArgs is ToastNotificationActivatedEventArgs notificationArgs)
            {
                var split = notificationArgs.Argument.Split('&');

                // get parsed arguments
                var action = split[0].Split('=')[1];
                var notificationId = split[1].Split('=')[1];

                var notificationsManager = ServiceLocator.Container.GetRequiredService<INotificationsManager>();

                var articlePost = notificationsManager.NotifiedPosts[notificationId];

                switch (action)
                {
                    case "showPost":
                        // TODO show post in Feed
                        break;
                    case "copyLink":
                        UiHelpers.ShareLinkViaClipboard(articlePost.FullArticleUrl);
                        break;
                    case "sharePost":
                        UiHelpers.ShareArticleViaSystemUI(articlePost);
                        break;
                }
            }
        }

        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            base.OnBackgroundActivated(args);
        }
        
        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            Logger.LogDebug("Application is being suspended.");

            // stop automatic updates
            var updater = ServiceLocator.Container.GetRequiredService<IAutomaticUpdater>();
            updater.Stop();

            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private static void OnResuming(object sender, object e)
        {
            Logger.LogDebug("Application is being resumed.");

            // manually refresh news feeds
            var dataSourcesManager = ServiceLocator.Container.GetRequiredService<IDataSourcesManager>();
            dataSourcesManager.LoadLatestPostsSinceLastUpdateAsync();

            // re-start automatic updates
            var updater = ServiceLocator.Container.GetRequiredService<IAutomaticUpdater>();
            updater.Start();
        }


        private void SetEventHandlers()
        {
            Suspending += OnSuspending;
            Current.Resuming += OnResuming;

            // unhandled exception
            UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedException;
        }

        private static void SetServices()
        {
            SentrySdk.Init("https://0f8a0d0187f9497ebc608603ec352c88@o504575.ingest.sentry.io/5709957");

            ServiceLocator.Initialize();

            ImageCache.Instance.MaxMemoryCacheCount = 50;
        }

        private static void SetLanguage(string languageCode)
        {
            ApplicationLanguages.PrimaryLanguageOverride = languageCode;
            ResourceContext.GetForCurrentView().Reset();
            ResourceContext.GetForViewIndependentUse().Reset();
        }

        private static void InitializeSettings()
        {
            var settingsManager = ServiceLocator.Container.GetRequiredService<ISettingsManager>();

            settingsManager.LoadSettingsAsync().ContinueWith(_ =>
            {
                settingsManager.NotificationSettings!.NotificationsAllowed = true;
                settingsManager.AutomaticUpdateSettings!.AutomaticUpdateAllowed = true;
                settingsManager.AutomaticUpdateSettings!.UpdateInterval = TimeSpan.FromSeconds(60);
            });
        }

        private static void LoadDataSources()
        {
            var manager = ServiceLocator.Container.GetRequiredService<IDataSourcesManager>();

            manager.RegisterDataSource(new NewsFeedDataSource(ServiceLocator.Container.GetRequiredService<DennikNskNewsFeed>(), new Uri("ms-appx:///Assets/Logos/denniknsk-logo.png")));
            manager.RegisterDataSource(new NewsFeedDataSource(ServiceLocator.Container.GetRequiredService<DenikNczNewsFeed>(), new Uri("ms-appx:///Assets/Logos/denikncz-logo.jpg")));
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.LogCritical(e.Exception, "Unhandled exception occurred.");
        }
        
        private static void OnUnobservedException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Logger.LogCritical(e.Exception, "Unhandled exception from background Task occurred.");
        }
    }
}
