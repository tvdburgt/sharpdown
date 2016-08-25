using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sharpdown.Output;
using System.Collections.Generic;
using System.Linq;

namespace Sharpdown
{
    internal class MemberVisitor : CSharpSyntaxWalker
    {
        private readonly SemanticModel model;
        private readonly DeclarationFormatter formatter;
        private readonly List<Declaration<MemberDeclarationSyntax>> members = new List<Declaration<MemberDeclarationSyntax>>();

        private MemberVisitor(SemanticModel model, DeclarationFormatter formatter)
        {
            this.model = model;
            this.formatter = formatter;
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            members.Add(new Declaration<MemberDeclarationSyntax>(
                node, model.GetDeclaredSymbol(node), node.Identifier,
                formatter.GetPropertyString(node)));
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            members.Add(new Declaration<MemberDeclarationSyntax>(
                node, model.GetDeclaredSymbol(node), node.Identifier,
                formatter.GetMethodString(node)));
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            var variables = node.Declaration.Variables.Select(v =>
                new Declaration<MemberDeclarationSyntax>(
                    node, model.GetDeclaredSymbol(v), v.Identifier,
                    formatter.GetFieldString(node, v)));

            members.AddRange(variables);
        }

        public static IEnumerable<Declaration<MemberDeclarationSyntax>> GetPublicMembers(SemanticModel model,
            DeclarationFormatter formatter, ClassDeclarationSyntax cls)
        {
            var visitor = new MemberVisitor(model, formatter);
            visitor.Visit(cls);

            return visitor.members
                .Where(m => m.Symbol.DeclaredAccessibility == Accessibility.Public)
                .OrderBy(m => m.Node.Kind())
                .ThenBy(m => m.Identifier.Text);
        }
    }
}
