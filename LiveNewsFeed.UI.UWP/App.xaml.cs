using System;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;

using LiveNewsFeed.DataSource.Common;
using LiveNewsFeed.DataSource.DenikNcz;
using LiveNewsFeed.DataSource.DennikNsk;

using LiveNewsFeed.UI.UWP.Common;
using LiveNewsFeed.UI.UWP.Managers;
using LiveNewsFeed.UI.UWP.Views;
using UnhandledExceptionEventArgs = Windows.UI.Xaml.UnhandledExceptionEventArgs;

namespace LiveNewsFeed.UI.UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            ServiceLocator.Initialize();

            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("sk-SK");

            LoadDataSources();

            Suspending += OnSuspending;
            UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedException;
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Debugger.Break();
        }

        private void OnUnobservedException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Debugger.Break();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
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

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(NewsFeedPage), e.Arguments);
                }

                // Ensure the current window is active
                Window.Current.Activate();
            }
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
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private static void LoadDataSources()
        {
            var manager = ServiceLocator.Container.GetRequiredService<IDataSourcesManager>();
            var httpClient = ServiceLocator.Container.GetRequiredService<HttpClient>();

            manager.RegisterDataSource(new NewsFeedDataSource(new DennikNskNewsFeed(httpClient), new Uri("ms-appx:///Assets/Logos/denniknsk-logo.png")));
            manager.RegisterDataSource(new NewsFeedDataSource(new DenikNczNewsFeed(httpClient), new Uri("ms-appx:///Assets/Logos/denikncz-logo.jpg")));
        }
    }
}
