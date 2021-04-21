using System;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;

using LiveNewsFeed.Models;

using LiveNewsFeed.UI.UWP.Common;

namespace LiveNewsFeed.UI.UWP.Services
{
    public class LiveTileService : ILiveTileService
    {
        private const int MaximumTileQueueSize = 5;
     
        private readonly TileUpdater _tileUpdater;

        private int _tileQueueCount;

        public LiveTileService()
        {
            _tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
            _tileUpdater.EnableNotificationQueue(true);
            _tileUpdater.Clear();

            _tileQueueCount = 0;
        }

        public void UpdateLiveTile(NewsArticlePost newsArticlePost, bool skipIfQueueIsFull = false)
        {
            if (skipIfQueueIsFull && _tileQueueCount == MaximumTileQueueSize)
                return;

            var builder = new TileContentBuilder();

            // set medium tile
            builder.AddTile(TileSize.Medium)
                   .SetBranding(TileBranding.NameAndLogo);
            if (newsArticlePost.Image != null)
                builder.SetPeekImage(newsArticlePost.Image!.Url, hintCrop: TilePeekImageCrop.Default);
            builder.SetTextStacking(TileTextStacking.Top)
                   .AddText(Helpers.SanitizeHtmlContent(newsArticlePost.Content),
                            hintStyle: AdaptiveTextStyle.Caption,
                            hintMaxLines: 4,
                            hintWrap: true)
                   .AddText(newsArticlePost.Title,
                            hintStyle: AdaptiveTextStyle.CaptionSubtle);

            // update Live tile
            _tileUpdater.Update(CreateTileNotification(builder));

            _tileQueueCount++;
        }


        private static TileNotification CreateTileNotification(TileContentBuilder builder) => new (builder.Content.GetXml())
        {
            ExpirationTime = DateTimeOffset.UtcNow.AddHours(4)
        };
    }
}