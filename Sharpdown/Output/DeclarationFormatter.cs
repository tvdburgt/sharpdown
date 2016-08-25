using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace Sharpdown.Output
{
    public class DeclarationFormatter
    {
        public string GetString(ClassDeclarationSyntax cls)
        {
            return $"{cls.Modifiers} class {cls.Identifier}";
        }

        public string GetString(PropertyDeclarationSyntax property)
        {
            var accessors = property.AccessorList.Accessors
                .Select(a => a.Keyword + ";");
            return $"{property.Modifiers} {property.Type} {property.Identifier} {{ {string.Join(" ", accessors)} }}";
        }

        public string GetString(FieldDeclarationSyntax field, VariableDeclaratorSyntax variable)
        {
            return $"{field.Modifiers} {field.Declaration.Type} {variable}";
        }

        public string GetString(MethodDeclarationSyntax method)
        {
            return $"{method.Modifiers} {method.ReturnType} {method.Identifier}{method.ParameterList}";
        }
    }
}
