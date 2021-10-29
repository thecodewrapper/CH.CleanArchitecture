using System.IO;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;

namespace CH.CleanArchitecture.Presentation.Framework
{
    public static class HtmlExtensions
    {

        /// <summary>
        /// Convert IHtmlContent to string
        /// </summary>
        /// <param name="htmlContent">HTML content</param>
        /// <returns>Result</returns>
        public static string RenderHtmlContent(this IHtmlContent htmlContent) {
            using var writer = new StringWriter();
            htmlContent.WriteTo(writer, HtmlEncoder.Default);
            var htmlOutput = writer.ToString();
            return htmlOutput;
        }

        /// <summary>
        /// Convert IHtmlContent to string
        /// </summary>
        /// <param name="tag">Tag</param>
        /// <returns>String</returns>
        public static string ToHtmlString(this IHtmlContent tag) {
            using var writer = new StringWriter();
            tag.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }

    }
}
