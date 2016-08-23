using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Sharpdown
{
    internal class ClassVisitor : CSharpSyntaxWalker
    {
        private readonly List<ClassDeclarationSyntax> classes = new List<ClassDeclarationSyntax>();

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            base.VisitClassDeclaration(node);
            classes.Add(node);
        }

        public static IEnumerable<ClassDeclarationSyntax> GetPublicClassDeclarations(Compilation compilation)
        {
            foreach (var tree in compilation.SyntaxTrees)
            {
                var model = compilation.GetSemanticModel(tree);
                var visitor = new ClassVisitor();

                visitor.Visit(tree.GetRoot());

                foreach (var cls in visitor.classes)
                {
                    var symbol = model.GetDeclaredSymbol(cls);
                    if (symbol.DeclaredAccessibility == Accessibility.Public)
                    {
                        yield return cls;
                    }
                }
            }
        }
    }
}
