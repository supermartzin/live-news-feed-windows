using System;
using System.Globalization;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources.Core;
using Windows.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using LiveNewsFeed.DataSource.AktualneCz;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UnhandledExceptionEventArgs = Windows.UI.Xaml.UnhandledExceptionEventArgs;

using LiveNewsFeed.DataSource.Common;
using LiveNewsFeed.DataSource.DenikNcz;
using LiveNewsFeed.DataSource.DennikNsk;
using LiveNewsFeed.DataSource.SeznamZpravyCzNewsFeed;
using LiveNewsFeed.UI.UWP.Common;
using LiveNewsFeed.UI.UWP.Managers;
using LiveNewsFeed.UI.UWP.Managers.Settings;
using LiveNewsFeed.UI.UWP.Services;
using LiveNewsFeed.UI.UWP.Views;

namespace LiveNewsFeed.UI.UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        private static readonly ILogger Logger = ServiceLocator.Container.GetRequiredService<ILogger<App>>();

        private readonly ISettingsManager _settingsManager;
        private readonly IThemeManager _themeManager;
        
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            SetGlobalEventHandlers();

            ImageCache.Instance.MaxMemoryCacheCount = 50;

            ServiceLocator.Initialize();
            _settingsManager = ServiceLocator.Container.GetRequiredService<ISettingsManager>();
            _themeManager = ServiceLocator.Container.GetRequiredService<IThemeManager>();

            InitializeSettings();
            LoadDataSources();
            SetInitialApplicationTheme();

            InitializeComponent();
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

            SetLanguage(_settingsManager.ApplicationSettings.DisplayLanguageCode);

            // set theme loaded from settings
            _themeManager.SetTheme(_settingsManager.ApplicationSettings.Theme);

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


        private void SetGlobalEventHandlers()
        {
            Suspending += OnSuspending;
            Current.Resuming += OnResuming;

            // unhandled exception
            UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedException;
        }

        private void InitializeSettings()
        {
            _settingsManager.LoadSettingsAsync().Wait();

            _settingsManager.ApplicationSettings.SettingChanged += ApplicationSettings_OnSettingChanged;
        }

        private void SetInitialApplicationTheme()
        {
            RequestedTheme = _settingsManager.ApplicationSettings.Theme switch
            {
                Theme.Light => ApplicationTheme.Light,
                Theme.Dark => ApplicationTheme.Dark,
                Theme.SystemDefault => _themeManager.CurrentSystemTheme,
                _ => _themeManager.CurrentSystemTheme
            };
        }

        private void SetLanguage(string? languageCode)
        {
            if (languageCode == null)
                return;

            ApplicationLanguages.PrimaryLanguageOverride = languageCode;
            ResourceContext.GetForCurrentView().Reset();
            ResourceContext.GetForViewIndependentUse().Reset();
            
            Logger.LogInformation($"App language set to '{CultureInfo.GetCultureInfo(languageCode)?.EnglishName ?? languageCode}'.");
        }

        private void ApplicationSettings_OnSettingChanged(object sender, SettingChangedEventArgs eventArgs)
        {
            switch (eventArgs.SettingName)
            {
                case nameof(ApplicationSettings.DisplayLanguageCode):
                    SetLanguage(eventArgs.GetNewValueAs<string>());

                    // reload UI
                    var navigationService = ServiceLocator.Container.GetRequiredService<INavigationService>();
                    navigationService.NavigateTo<NewsFeedPage>(tempDisableCache: true);
                    break;

                case nameof(ApplicationSettings.Theme):
                    _themeManager.SetTheme(eventArgs.GetNewValueAs<Theme>());
                    break;
            }
        }
        
        private static void LoadDataSources()
        {
            var manager = ServiceLocator.Container.GetRequiredService<IDataSourcesManager>();

            manager.RegisterDataSource(new NewsFeedDataSource(ServiceLocator.Container.GetRequiredService<DennikNskNewsFeed>(),
                                                              new Logo(new Uri("ms-appx:///Assets/Logos/denniknsk-logo.png"),
                                                                       new Uri("ms-appx:///Assets/Logos/denniknsk-logo.png"))));
            manager.RegisterDataSource(new NewsFeedDataSource(ServiceLocator.Container.GetRequiredService<DenikNczNewsFeed>(),
                                                              new Logo(new Uri("ms-appx:///Assets/Logos/denikncz-logo.jpg"),
                                                                       new Uri("ms-appx:///Assets/Logos/denikncz-logo.jpg"))));
            manager.RegisterDataSource(new NewsFeedDataSource(ServiceLocator.Container.GetRequiredService<AktualneCzNewsFeed>(),
                                                              new Logo(new Uri("ms-appx:///Assets/Logos/aktualnecz-logo-lighttheme.png"),
                                                                       new Uri("ms-appx:///Assets/Logos/aktualnecz-logo-darktheme.png"))));
            manager.RegisterDataSource(new NewsFeedDataSource(ServiceLocator.Container.GetRequiredService<SeznamZpravyCzNewsFeed>(),
                                                              new Logo(new Uri("ms-appx:///Assets/Logos/seznamzpravy-logo.png"),
                                                                       new Uri("ms-appx:///Assets/Logos/seznamzpravy-logo.png"))));
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
