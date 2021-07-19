using System;
using System.Text.RegularExpressions;
using Windows.Data.Html;

namespace LiveNewsFeed.UI.UWP.Common
{
    public static class StringExtensions
    {
        public static string SanitizeHtmlContent(this string htmlContent)
        {
            if (htmlContent == null)
                throw new ArgumentNullException(nameof(htmlContent));

            // replace bullet list occurrences
            var content = htmlContent.Replace("<li>", "<li> • ");

            // replace original newlines
            content = content.Replace("\r\n", "($nl$)")
                             .Replace("\n", "($nl$)");

            // remove HTML tags
            content = HtmlUtilities.ConvertToText(content);
            
            // replace newly converted newlines
            content = content.Replace("\r\n", "($nl$)")
                             .Replace("\n", "($nl$)");

            // put original and new newlines back
            content = content.Replace("($nl$)($nl$)($nl$)($nl$)", Environment.NewLine + Environment.NewLine)
                             .Replace("($nl$)($nl$)($nl$)", Environment.NewLine + Environment.NewLine)
                             .Replace("($nl$)($nl$)", Environment.NewLine + Environment.NewLine)
                             .Replace("($nl$)", Environment.NewLine);

            content = content.Replace(Environment.NewLine + Environment.NewLine + Environment.NewLine,
                                      Environment.NewLine + Environment.NewLine);

            content = content.Replace($"{Environment.NewLine}•", "•");

            return content.Trim();
        }

        public static string SanitizeHtmlForTextBlock(this string htmlContent)
        {
            if (htmlContent == null)
                throw new ArgumentNullException(nameof(htmlContent));

            // replace bold tags
            htmlContent = htmlContent.Replace("<strong>", "(bold)")
                                     .Replace("</strong>", "(.bold)");

            // replace links
            var expression = new Regex(@"<a\W{1}.*?href=\""(.*?)\"".*?>(.*?)<\/a>");
            foreach (Match result in expression.Matches(htmlContent))
            {
                if (result.Groups.Count != 3)
                    continue;

                // create custom link elements
                var linkTag = $"(link={result.Groups[1].Value}){result.Groups[2].Value}(.link)";

                // replace old link tag for the new one
                htmlContent = htmlContent.Replace(result.Value, linkTag);
            }

            return htmlContent.Trim();
        }
    }
}