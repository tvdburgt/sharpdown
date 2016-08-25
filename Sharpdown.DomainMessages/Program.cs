using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Sharpdown.Output;

namespace Sharpdown.DomainMessages
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var template = File.OpenText(@"doc.hbs"))
            using (var output = File.CreateText(@"doc.md"))
            {
                Sharpdown.GenerateAsync(@"..\..\..\examples\SimpleCQRS\SimpleCQRS.csproj", template, output, ProcessTemplate)
                    .GetAwaiter().GetResult();
            }
        }

        private static object ProcessTemplate(Compilation compilation, TemplateData data)
        {
            return new
            {
                Title = data.Title,
                Commands = GetDomainMessages(compilation, data, "SimpleCQRS.Command"),
                Events = GetDomainMessages(compilation, data, "SimpleCQRS.Event"),
            };
        }

        private static IEnumerable<TypeMetadata> GetDomainMessages(Compilation compilation, TemplateData data, string baseType)
        {
            var types = data.NamedTypes
                .Where(t => t.Class.Symbol.Inherits(compilation, baseType));

            foreach (var t in types)
            {
                t.Members = t.Members.Where(m => m.Node.IsKind(SyntaxKind.FieldDeclaration));
            }

            return types;
        }
    }
}
