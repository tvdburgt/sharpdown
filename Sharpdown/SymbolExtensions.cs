using Microsoft.CodeAnalysis;
using System.Xml.Linq;

namespace Sharpdown
{
    internal static class SymbolExtensions
    {
        public static string GetSummary(this ISymbol symbol)
        {
            var xml = symbol.GetDocumentationCommentXml();
            if (string.IsNullOrEmpty(xml))
            {
                return null;
            }

            var doc = XDocument.Parse(xml);
            var member = doc.Element("member");
            var summary = doc.Root.Element("summary")?.Value?.Trim();

            return summary ?? member.Value.Trim();
        }
    }
}
