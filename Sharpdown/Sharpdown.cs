using HandlebarsDotNet;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Sharpdown.Output;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Sharpdown
{
    public class Sharpdown
    {
        public static async Task GenerateAsync(string projectPath, TextReader template, TextWriter output)
        {
            await GenerateAsync(projectPath, template, output, data => data);
        }

        public static async Task GenerateAsync(string projectPath, TextReader template, TextWriter output,
            Func<TemplateData, object> processor)
        {
            var ws = MSBuildWorkspace.Create();
            var project = await ws.OpenProjectAsync(projectPath);
            var compilation = await project.GetCompilationAsync();
            var types = GetNamedTypes(compilation);

            Handlebars.Configuration.TextEncoder = new NopEncoder();
            var templator = Handlebars.Compile(template);
            var data = new TemplateData
            {
                Title = project.AssemblyName,
                NamedTypes = types.OrderBy(x => x.Class.Node.Identifier.Value),
            };

            templator(output, processor(data));
        }

        private static IEnumerable<TypeMetadata> GetNamedTypes(Compilation compilation)
        {
            var types = ClassVisitor.GetPublicClasses(compilation);

            var declarationFormatter = new DeclarationFormatter();

            foreach (var t in types)
            {
                var model = compilation.GetSemanticModel(t.SyntaxTree);

                yield return new TypeMetadata
                {
                    Class = Declaration<ClassDeclarationSyntax>.FromModel(
                        model, t, declarationFormatter.GetString(t)),
                    Members = MemberVisitor.GetPublicMembers(model, t)
                };
            }
        }
    }
}
