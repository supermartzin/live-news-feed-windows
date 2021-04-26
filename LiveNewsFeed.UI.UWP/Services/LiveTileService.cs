using System;
using System.Linq;
using Windows.ApplicationModel;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;

using LiveNewsFeed.Models;

using LiveNewsFeed.UI.UWP.Common;

namespace LiveNewsFeed.UI.UWP.Services
{
    public class LiveTileService : ILiveTileService
    {
        private const int MaxNumberOfTileContents = 5;
        private const int TilesExpirationTimeHours = 12;
     
        private readonly TileUpdater _tileUpdater;
        private readonly FixedSizeSortedQueue<NewsArticlePost> _postsOnTileQueue;
        
        public LiveTileService()
        {
            _tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
            _tileUpdater.EnableNotificationQueue(true);
            _tileUpdater.Clear();

            _postsOnTileQueue = new FixedSizeSortedQueue<NewsArticlePost>(MaxNumberOfTileContents);
        }

        public void UpdateLiveTile(NewsArticlePost newsArticlePost, bool skipIfQueueIsFull = false)
        {
            if (skipIfQueueIsFull && _postsOnTileQueue.IsFull())
                return;

            // add to collection
            _postsOnTileQueue.Enqueue(newsArticlePost);

            // build Live tiles content
            var builder = new TileContentBuilder();
            builder.AddTile(TileSize.Medium | TileSize.Wide | TileSize.Large)
                   .SetBranding(TileBranding.NameAndLogo)
                   .SetTextStacking(TileTextStacking.Top);

            SetMediumAndWideTileContent(builder, newsArticlePost);
            SetLargeTileContent(builder);
            
            // update Live tiles
            _tileUpdater.Update(CreateTileNotification(builder));
        }

        
        private void SetMediumAndWideTileContent(TileContentBuilder builder, NewsArticlePost newsArticlePost)
        {
            builder.AddTile(TileSize.Medium | TileSize.Wide)
                   .SetDisplayName($"{newsArticlePost.PublishTime:t} | {AppInfo.Current.DisplayInfo.DisplayName}", size: TileSize.Medium | TileSize.Wide)
                   .AddText(newsArticlePost.Title, hintStyle: AdaptiveTextStyle.Base, size: TileSize.Medium | TileSize.Wide)
                   .AddText(Helpers.SanitizeHtmlContent(newsArticlePost.Content),
                            hintStyle: AdaptiveTextStyle.Caption, hintMaxLines: 3, hintWrap: true, size: TileSize.Medium | TileSize.Wide);
            
            if (newsArticlePost.Image != null)
                builder.SetPeekImage(newsArticlePost.Image!.Url, TileSize.Medium | TileSize.Wide);
        }

        private void SetLargeTileContent(TileContentBuilder builder)
        {
            var numberOfCreatedTiles = 0;

            // create multiple posts in one tile
            if (_postsOnTileQueue.Count >= 3)
            {
                var content = builder.Content.Visual.TileLarge.Content as TileBindingContentAdaptive;
                for (var i = 0; i < 3; i++)
                {
                    var articlePost = _postsOnTileQueue.ElementAt(i);

                    content?.Children.Add(CreateLargeTileGroup(
                        header: $"{articlePost.PublishTime:t} | {articlePost.Title}",
                        content: Helpers.SanitizeHtmlContent(articlePost.Content)));

                    if (i == 0 && articlePost.Image != null)
                        builder.SetPeekImage(articlePost.Image!.Url, TileSize.Large);
                }

                numberOfCreatedTiles++;
            }

            // create single post tiles
            for (var postIndex = 0; numberOfCreatedTiles < MaxNumberOfTileContents; postIndex++)
            {
                var newsArticlePost = _postsOnTileQueue.ElementAtOrDefault(postIndex);
                if (newsArticlePost == null)
                    break;

                // create content
                builder.AddTile(TileSize.Large)
                    .AddText(newsArticlePost.Title, hintStyle: AdaptiveTextStyle.Subtitle, size: TileSize.Large)
                    .AddText($"{newsArticlePost.PublishTime:t} {newsArticlePost.PublishTime:d}", hintStyle: AdaptiveTextStyle.BaseSubtle, size: TileSize.Large)
                    .AddText(Helpers.SanitizeHtmlContent(newsArticlePost.Content), hintStyle: AdaptiveTextStyle.Default, hintWrap: true, size: TileSize.Large);

                if (newsArticlePost.Image != null)
                    builder.SetPeekImage(newsArticlePost.Image!.Url, TileSize.Large);

                numberOfCreatedTiles++;
            }
        }

        private static AdaptiveGroup CreateLargeTileGroup(string header, string content) => new()
        {
            Children =
            {
                new AdaptiveSubgroup
                {
                    Children =
                    {
                        new AdaptiveText
                        {
                            Text = header,
                            HintStyle = AdaptiveTextStyle.CaptionSubtle,
                            HintAlign = AdaptiveTextAlign.Left,
                            HintMaxLines = 1,
                            HintWrap = false
                        },
                        new AdaptiveText
                        {
                            Text = content,
                            HintStyle = AdaptiveTextStyle.Caption,
                            HintAlign = AdaptiveTextAlign.Left,
                            HintMaxLines = 2,
                            HintWrap = true
                        },
                        new AdaptiveText()
                    }
                }
            }
        };

        private static TileNotification CreateTileNotification(TileContentBuilder builder) => new (builder.Content.GetXml())
        {
            ExpirationTime = DateTimeOffset.UtcNow.AddHours(TilesExpirationTimeHours)
        };
    }
}