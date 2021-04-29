using Windows.UI;
using Windows.UI.Xaml.Media;

using LiveNewsFeed.Models;

using LiveNewsFeed.UI.UWP.Managers;
using LiveNewsFeed.UI.UWP.Resources;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public sealed class CategoryViewModel : ViewModelBase
    {
        private readonly Category _category;
        
        public string Name => GetLocalizedString($"CategoryEnum_{_category}");

        public string IconGlyph { get; }

        public FontFamily FontFamily { get; }
        
        public Color Color => ThemeResources.GetAs<Color>($"Category{_category}Color");
        
        public CategoryViewModel(Category category,
                                 string iconGlyph,
                                 FontFamily fontFamily)
        {
            _category = category;
            IconGlyph = iconGlyph;
            FontFamily = fontFamily;
        }
        
        protected override void OnApplicationThemeChanged(Theme theme)
        {
            // force reevaluation of Color property
            OnPropertyChanged(nameof(Color));
        }
    }
}