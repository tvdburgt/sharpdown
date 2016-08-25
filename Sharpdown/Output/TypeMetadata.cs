using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sharpdown.Output
{
    public class TypeMetadata
    {
        public Declaration<ClassDeclarationSyntax> Class { get; set; }
        public IEnumerable<Declaration<CSharpSyntaxNode>> Members { get; set; }
    }
}
