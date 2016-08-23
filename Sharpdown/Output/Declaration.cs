using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.IO;

namespace Sharpdown.Output
{
    public class Declaration<T> where T : CSharpSyntaxNode
    {
        public string Value { get; set; }
        public string Summary { get; set; }
        public T Node { get; set; }
        public ISymbol Symbol { get; set; }

        public IEnumerable<string> SummaryLines
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Summary))
                {
                    yield break;
                }

                string line;
                using (var reader = new StringReader(Summary))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        yield return line.Trim();
                    }
                }
            }
        }

        public static Declaration<T> FromModel(SemanticModel model, T node, string value)
        {
            var symbol = model.GetDeclaredSymbol(node);
            return new Declaration<T>
            {
                Node = node,
                Symbol = symbol,
                Summary = symbol.GetSummary(),
                Value = value,
            };
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
