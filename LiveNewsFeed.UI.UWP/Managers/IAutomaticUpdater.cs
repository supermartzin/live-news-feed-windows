using System;

using LiveNewsFeed.UI.UWP.Managers.Settings;

namespace LiveNewsFeed.UI.UWP.Managers
{
    public interface IAutomaticUpdater
    {
        AutomaticUpdateSettings Settings { get; }

        event EventHandler AutomaticUpdateRequested;

        void Start();

        void Stop();
    }
}