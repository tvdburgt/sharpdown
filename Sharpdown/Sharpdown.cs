using HandlebarsDotNet;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Sharpdown.Output;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sharpdown
{
    public class Sharpdown
    {
        public static async Task DocumentProjectAsync(string path, TextWriter output)
        {
            var ws = MSBuildWorkspace.Create();
            var project = await ws.OpenProjectAsync(path);
            var compilation = await project.GetCompilationAsync();

            var types = ClassVisitor.GetPublicClassDeclarations(compilation);

            var declarationFormatter = new DeclarationFormatter();
            var classes = new List<TypeMetadata>();

            foreach (var c in types)
            {
                var model = compilation.GetSemanticModel(c.SyntaxTree);

                var cls = new TypeMetadata
                {
                    Class = Declaration<ClassDeclarationSyntax>.FromModel(
                        model, c, declarationFormatter.GetClassString(c))
                };

                classes.Add(cls);

                cls.Properties = c.Members
                    .OfType<PropertyDeclarationSyntax>()
                    .Select(d => Declaration<PropertyDeclarationSyntax>.FromModel(model, d, declarationFormatter.GetPropertyString(d)))
                    .Where(d => d.Symbol.DeclaredAccessibility == Accessibility.Public)
                    .OrderBy(d => d.Node.Identifier.Value);

                cls.Methods = c.Members
                    .OfType<MethodDeclarationSyntax>()
                    .Select(d => Declaration<MethodDeclarationSyntax>.FromModel(model, d, declarationFormatter.GetMethodString(d)))
                    .Where(d => d.Symbol.DeclaredAccessibility == Accessibility.Public)
                    .OrderBy(d => d.Node.Identifier.Value);

                cls.Fields = c.Members
                    .OfType<FieldDeclarationSyntax>()
                    .SelectMany(d => d.Declaration.Variables.Select(v => Declaration<VariableDeclaratorSyntax>.FromModel(
                        model, v, declarationFormatter.GetVariableString(d, v))))
                    .Where(d => d.Symbol.DeclaredAccessibility == Accessibility.Public)
                    .OrderBy(d => d.Node.Identifier.Value);
            }

            using (var reader = File.OpenText(@"Output\doc.md.hbs"))
            {
                Handlebars.Configuration.TextEncoder = new NopEncoder();
                var template = Handlebars.Compile(reader);
                template(output, new
                {
                    Title = project.Name,
                    Classes = classes.OrderBy(x => x.Class.Node.Identifier.Value),
                });
            }
        }
    }
}
