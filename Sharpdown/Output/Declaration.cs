using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;
using System.IO;

namespace Sharpdown.Output
{
    public class Declaration<T> where T : CSharpSyntaxNode
    {
        public string Representation { get; set; }
        public string Summary { get; set; }

        public SyntaxToken Identifier { get; set; }
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

                using (var reader = new StringReader(Summary))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        yield return line.Trim();
                    }
                }
            }
        }

        public Declaration(T node, ISymbol symbol, SyntaxToken identifier, string representation)
        {
            Symbol = symbol;
            Node = node;
            Summary = symbol.GetSummary();
            Representation = representation;
            Identifier = identifier;
        }

        public override string ToString()
        {
            return Representation;
        }
    }
}
