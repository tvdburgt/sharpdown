using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using System.Xml.Linq;

namespace Sharpdown
{
    public static class SymbolExtensions
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
            var summary = doc.Root.Element("summary")?.Value.Trim();

            return summary ?? member.Value.Trim();
        }

        public static bool Inherits(this ISymbol symbol, Compilation compilation, string name)
        {
            if (!(symbol is INamedTypeSymbol))
            {
                throw new ArgumentException("Symbol is not a named type", nameof(symbol));
            }

            var t = compilation.GetTypeByMetadataName(name);
            if (t == null)
            {
                throw new InvalidOperationException($"Named type '{name}' does not exist in compilation");
            }

            return ((INamedTypeSymbol)symbol).Inherits(t);
        }


        public static bool Inherits(this INamedTypeSymbol symbol, INamedTypeSymbol baseSymbol)
        {
            while (symbol.BaseType != null)
            {
                if (symbol.BaseType.Equals(baseSymbol))
                {
                    return true;
                }
                symbol = symbol.BaseType;
            }

            return false;
        }

        public static bool Implements(this INamedTypeSymbol symbol, INamedTypeSymbol iface)
        {
            return symbol.AllInterfaces.Contains(iface);
        }
    }
}
