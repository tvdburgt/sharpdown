using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Sharpdown.Output
{
    public class TypeMetadata
    {
        public Declaration<ClassDeclarationSyntax> Class { get; set; }
        public IEnumerable<Declaration<VariableDeclaratorSyntax>> Fields { get; set; }
        public IEnumerable<Declaration<PropertyDeclarationSyntax>> Properties { get; set; }
        public IEnumerable<Declaration<MethodDeclarationSyntax>> Methods { get; set; }
    }
}
