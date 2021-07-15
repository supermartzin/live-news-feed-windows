using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Microsoft.Extensions.DependencyInjection;

using LiveNewsFeed.UI.UWP.Common;
using LiveNewsFeed.UI.UWP.Managers;

namespace LiveNewsFeed.UI.UWP.Controls
{
    [Bindable]
    public class TextBlockExtensions : DependencyObject
    {
        private static readonly IThemeManager ThemeManager = ServiceLocator.Container.GetRequiredService<IThemeManager>();

        private static readonly IList<Hyperlink> Hyperlinks = new List<Hyperlink>();

        static TextBlockExtensions()
        {
            ThemeManager.SystemThemeChanged += async (_, _) => await ReloadHyperlinksColor().ConfigureAwait(false);
            ThemeManager.ApplicationThemeChanged += async (_, _) => await ReloadHyperlinksColor().ConfigureAwait(false);
            ThemeManager.SystemAccentColorChanged += async (_, _) => await ReloadHyperlinksColor().ConfigureAwait(false);
        }
        
        public static readonly DependencyProperty FormattedTextProperty = DependencyProperty.Register("FormattedText",
                                                                                                      typeof(string),
                                                                                                      typeof(TextBlock),
                                                                                                      PropertyMetadata.Create(string.Empty, OnFormattedTextChanged));

        public static string GetFormattedText(DependencyObject obj) => (string) obj.GetValue(FormattedTextProperty);
        
        public static void SetFormattedText(DependencyObject obj, string value) => obj.SetValue(FormattedTextProperty, value);
        

        private static void OnFormattedTextChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (dependencyObject is not TextBlock textBlock)
                return;
            if (eventArgs.NewValue is not string text)
            {
                textBlock.Inlines.Clear();
                return;
            }
            
            textBlock.Inlines.Clear();
            var results = Regex.Matches(text, @"\(bold\)(.*?)\(\.bold\)|\(link=(.*?)\)(.*?)\(\.link\)", RegexOptions.Singleline);

            var matchNumber = 0;
            var fullyProcessed = false;
            while (!fullyProcessed)
            {
                // simple text with no formatting OR last part of the unprocessed text OR end of the text
                if (matchNumber == results.Count)
                {
                    if (!string.IsNullOrWhiteSpace(text))
                        textBlock.Inlines.Add(CreateRun(text));

                    fullyProcessed = true;
                    continue;
                }

                var wholeMatch = results[matchNumber].Value;
                var groups = results[matchNumber].Groups.Where(group => group.Success).ToArray();
                var index = text.IndexOf(wholeMatch, StringComparison.Ordinal);
                if (index > 0)
                {
                    // create Run from text before the match
                    textBlock.Inlines.Add(CreateRun(text.Substring(0, index)));
                }

                switch (groups.Length)
                {
                    case 2:
                        // bold text
                        foreach (var inline in ProcessBoldText(groups[1].Value))
                        {
                            textBlock.Inlines.Add(inline);
                        }
                        break;
                    case 3:
                    {
                        // link
                        textBlock.Inlines.Add(CreateLink(groups[1].Value,
                                                         groups[2].Value.Replace("(bold)", string.Empty)
                                                                        .Replace("(.bold)", string.Empty)));
                        break;
                    }
                }

                // trim text of processed parts
                text = text.Substring(index + wholeMatch.Length);

                matchNumber++;
            }
        }

        private static IEnumerable<Inline> ProcessBoldText(string text)
        {
            var inlines = new List<Inline>();

            // check if does not contain link
            var linkResult = Regex.Match(text, @"\(link=(.*?)\)(.*?)\(\.link\)", RegexOptions.Singleline);
            if (!linkResult.Success)
            {
                // only bold text
                inlines.Add(CreateRun(text, FontWeights.SemiBold));
            }
            else
            {
                // bold text with link
                var index = text.IndexOf(linkResult.Value, StringComparison.Ordinal);
                if (index > 0)
                {
                    // process text before link
                    inlines.Add(CreateRun(text.Substring(0, index), FontWeights.SemiBold));
                    text = text.Substring(index);
                }

                // process link
                inlines.Add(CreateLink(linkResult.Groups[1].Value, linkResult.Groups[2].Value));
                text = text.Substring(linkResult.Value.Length);

                if (text.Length > 0)
                {
                    // process text after link
                    inlines.Add(CreateRun(text, FontWeights.SemiBold));
                }
            }

            return inlines;
        }

        private static Run CreateRun(string text, FontWeight? weight = null) => new()
        {
            Text = text,
            FontWeight = weight ?? FontWeights.Normal
        };

        private static Hyperlink CreateLink(string url, string text)
        {
            var link = new Hyperlink
            {
                NavigateUri = new Uri(url),
                TextDecorations = TextDecorations.None,
                Foreground = new SolidColorBrush(GetCurrentHyperlinkColor())
            };
            
            Hyperlinks.Add(link);

            // link text
            link.Inlines.Add(CreateRun(text, FontWeights.SemiBold));

            // url as tooltip
            ToolTipService.SetToolTip(link, new ToolTip
            {
                Content = url
            });

            return link;
        }

        private static Color GetCurrentHyperlinkColor() => ThemeManager.CurrentApplicationTheme switch
        {
            ApplicationTheme.Dark => ThemeManager.GetSystemColor(UIColorType.AccentLight1),
            ApplicationTheme.Light => ThemeManager.GetSystemColor(UIColorType.AccentDark1),
            _ => ThemeManager.GetSystemColor(UIColorType.Accent)
        };

        private static async Task ReloadHyperlinksColor()
        {
            await Helpers.InvokeOnUiAsync(() =>
            {
                foreach (var hyperlink in Hyperlinks)
                {
                    hyperlink.Foreground = new SolidColorBrush(GetCurrentHyperlinkColor());
                }
            });
        }
    }
}