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
            await GenerateAsync(projectPath, template, output, (compilation, data) => data);
        }

        public static async Task GenerateAsync(string projectPath, TextReader template, TextWriter output,
            Func<Compilation, TemplateData, object> processor)
        {
            var ws = MSBuildWorkspace.Create();
            var project = await ws.OpenProjectAsync(projectPath);
            var compilation = await project.GetCompilationAsync();
            var types = GetNamedTypes(compilation);
            var templator = CreateTemplator(template);

            var data = new TemplateData
            {
                Title = project.AssemblyName,
                NamedTypes = types.OrderBy(x => x.Class.Node.Identifier.Value),
            };

            templator(output, processor(compilation, data));
        }

        private static IEnumerable<TypeMetadata> GetNamedTypes(Compilation compilation)
        {
            var formatter = new DeclarationFormatter();
            var types = ClassVisitor.GetPublicClasses(compilation);

            foreach (var t in types)
            {
                var model = compilation.GetSemanticModel(t.SyntaxTree);
                yield return new TypeMetadata
                {
                    Class = new Declaration<ClassDeclarationSyntax>(
                        t, model.GetDeclaredSymbol(t), t.Identifier, formatter.GetClassString(t)),
                    Members = MemberVisitor.GetPublicMembers(model, formatter, t)
                };
            }
        }

        private static Action<TextWriter, object> CreateTemplator(TextReader template)
        {
            Handlebars.RegisterHelper("camelcase", (writer, context, parameters) =>
            {
                if (parameters.Length == 0)
                {
                    return;
                }

                writer.WriteSafeString(ConvertToCamelCase(parameters[0] as string));
            });

            Handlebars.Configuration.TextEncoder = new NopEncoder();

            return Handlebars.Compile(template);
        }

        private static string ConvertToCamelCase(string s)
        {
            if (string.IsNullOrEmpty(s) || s.Length <= 1 || s.All(char.IsUpper))
            {
                return s;
            }

            return char.ToLowerInvariant(s[0]) + s.Substring(1);
        }
    }
}
