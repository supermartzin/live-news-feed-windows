using System;
using Windows.Data.Html;

using LiveNewsFeed.Models;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public class ArticleImageViewModel : ViewModelBase
    {
        public Image OriginalImage { get; }

        public Uri NormalImageUrl => OriginalImage.Url;

        public Uri? LargeImageUrl => OriginalImage?.LargeSizeUrl;

        public string? Title { get; }

        public ArticleImageViewModel(Image image)
        {
            OriginalImage = image ?? throw new ArgumentNullException(nameof(image));

            Title = SanitizeTitleText(image.Title);
        }

        private static string? SanitizeTitleText(string? imageTitle) => imageTitle != null ? HtmlUtilities.ConvertToText(imageTitle).Trim() : default;
    }

}