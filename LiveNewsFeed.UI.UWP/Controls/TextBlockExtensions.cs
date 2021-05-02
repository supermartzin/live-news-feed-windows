using System;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;

namespace LiveNewsFeed.UI.UWP.Controls
{
    [Bindable]
    public class TextBlockExtensions : DependencyObject
    {
        public static readonly DependencyProperty FormattedTextProperty = DependencyProperty.Register("FormattedText",
                                                                                                      typeof(string),
                                                                                                      typeof(TextBlock),
                                                                                                      PropertyMetadata.Create(string.Empty, OnFormattedTextChanged));

        public static string GetFormattedString(DependencyObject obj) => (string) obj.GetValue(FormattedTextProperty);
        
        public static void SetFormattedString(DependencyObject obj, string value) => obj.SetValue(FormattedTextProperty, value);
        

        private static void OnFormattedTextChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (dependencyObject is not TextBlock textBlock)
                return;
            if (eventArgs.NewValue is not string text)
                return;

            textBlock.Inlines.Clear();
            var results = Regex.Matches(text, "\\(bold\\)(.*?)\\(\\.bold\\)|\\(link=(.*?)\\)(.*?)\\(\\.link\\)", RegexOptions.Singleline);

            var matchNumber = 0;
            var fullyProcessed = false;
            while (!fullyProcessed)
            {
                // simple text with no formatting OR last part of the unprocessed text OR end of the text
                if (matchNumber == results.Count)
                {
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        textBlock.Inlines.Add(new Run
                        {
                            Text = text
                        });
                    }
                    fullyProcessed = true;
                    continue;
                }

                var wholeMatch = results[matchNumber].Value;
                var groups = results[matchNumber].Groups.Where(group => group.Success).ToArray();
                var index = text.IndexOf(wholeMatch, StringComparison.Ordinal);
                if (index > 0)
                {
                    // create Run from text before the match
                    textBlock.Inlines.Add(new Run
                    {
                        Text = text.Substring(0, index)
                    });
                }

                switch (groups.Length)
                {
                    case 2:
                        // bold text
                        textBlock.Inlines.Add(new Run
                        {
                            Text = groups[1].Value,
                            FontWeight = FontWeights.SemiBold
                        });
                        break;
                    case 3:
                    {
                        // link
                        var link = new Hyperlink
                        {
                            NavigateUri = new Uri(groups[1].Value),
                            TextDecorations = TextDecorations.None
                        };
                        link.Inlines.Add(new Run
                        {
                            Text = groups[2].Value.Trim(),
                            FontWeight = FontWeights.SemiBold
                        });

                        // set url as tooltip
                        ToolTipService.SetToolTip(link, new ToolTip
                        {
                            Content = groups[1].Value
                        });

                        textBlock.Inlines.Add(link);
                        break;
                    }
                }

                // trim text of processed parts
                text = text.Substring(index + wholeMatch.Length);

                matchNumber++;
            }
        }
    }
}