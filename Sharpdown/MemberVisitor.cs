using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sharpdown.Output;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Sharpdown
{
    internal class MemberVisitor : CSharpSyntaxWalker
    {
        private readonly SemanticModel model;
        private readonly List<Declaration<CSharpSyntaxNode>> members = new List<Declaration<CSharpSyntaxNode>>();

        private readonly DeclarationFormatter formatter = new DeclarationFormatter();

        private MemberVisitor(SemanticModel model)
        {
            this.model = model;
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            members.Add(Declaration<CSharpSyntaxNode>.FromModel(model, node, formatter.GetString(node)));
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            members.Add(Declaration<CSharpSyntaxNode>.FromModel(model, node, formatter.GetString(node)));
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            var variables = node.Declaration.Variables.Select(
                v => Declaration<CSharpSyntaxNode>.FromModel(model, v, formatter.GetString(node, v)));

            members.AddRange(variables);
        }

        public static IEnumerable<Declaration<CSharpSyntaxNode>> GetPublicMembers(SemanticModel model, ClassDeclarationSyntax cls)
        {
            var visitor = new MemberVisitor(model);
            visitor.Visit(cls);

            return visitor.members
                .Where(m => m.Symbol.DeclaredAccessibility == Accessibility.Public)
                .OrderBy(m => m.Node.Kind());
        }
    }
}
