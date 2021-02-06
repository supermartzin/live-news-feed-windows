using Windows.UI;
using Windows.UI.Xaml.Media;

using LiveNewsFeed.Models;

using LiveNewsFeed.UI.UWP.Common;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public sealed class CategoryViewModel : ViewModelBase
    {
        public string Name { get; }

        public string IconGlyph { get; }

        public FontFamily FontFamily { get; }

        public Color Color { get; }
        
        public CategoryViewModel(Category category,
                                 Color color,
                                 string iconGlyph,
                                 FontFamily fontFamily)
        {
            Name = GetLocalizedString($"CategoryEnum_{category}");
            Color = color;
            IconGlyph = iconGlyph;
            FontFamily = fontFamily;
        }
    }
}