using System;
using GalaSoft.MvvmLight;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public class ImageViewModel : ViewModelBase
    {
        private Uri _normalImageUrl;
        public Uri NormalImageUrl
        {
            get => _normalImageUrl;
            set => Set(ref _normalImageUrl, value);
        }

        private Uri? _largeImageUrl;
        public Uri? LargeImageUrl
        {
            get => _largeImageUrl;
            set => Set(ref _largeImageUrl, value);
        }

        private string? _title;
        public string? Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        public ImageViewModel(Uri normalImageUrl,
                              string? title = null,
                              Uri? largeImageUrl = null)
        {
            _normalImageUrl = normalImageUrl;
            _title = title;
            _largeImageUrl = largeImageUrl ?? normalImageUrl;
        }
    }
}