using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

using LiveNewsFeed.Models;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public sealed class CategoryViewModel : ViewModelBase
    {
        public string Name { get; }

        public string IconGlyph { get; }

        public FontFamily FontFamily { get; }

        private readonly string _colorResourceKey;
        public Color Color => (Color) Application.Current.Resources[_colorResourceKey];
        
        public CategoryViewModel(Category category,
                                 string colorResourceKey,
                                 string iconGlyph,
                                 FontFamily fontFamily)
        {
            Name = GetLocalizedString($"CategoryEnum_{category}");
            IconGlyph = iconGlyph;
            FontFamily = fontFamily;

            _colorResourceKey = colorResourceKey;
        }

        protected override void OnApplicationThemeChanged(ApplicationTheme theme)
        {
            // force reevaluation of Color property
            RaisePropertyChanged(nameof(Color));
        }
    }
}