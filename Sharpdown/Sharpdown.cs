using HandlebarsDotNet;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Sharpdown.Output;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sharpdown
{
    public class TemplateData
    {
        public string Title { get; set; }
        public IEnumerable<TypeMetadata> NamedTypes { get; set; }
    }

    public class Sharpdown
    {
        public static async Task GenerateAsync(string projectPath, TextReader template, TextWriter output)
        {
            await GenerateAsync(projectPath, template, output, data => data);
        }

        public static async Task GenerateAsync(string projectPath, TextReader template, TextWriter output, Func<TemplateData, object> processor)
        {
            var ws = MSBuildWorkspace.Create();
            var project = await ws.OpenProjectAsync(projectPath);
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

            Handlebars.Configuration.TextEncoder = new NopEncoder();
            var templator = Handlebars.Compile(template);

            var data = new TemplateData
            {
                Title = project.Name,
                NamedTypes = classes.OrderBy(x => x.Class.Node.Identifier.Value),
            };

            templator(output, processor(data));
        }
    }
}
