using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Sharpdown.Output
{
    public class DeclarationFormatter
    {
        public string GetClassString(ClassDeclarationSyntax cls)
        {
            return $"{cls.Modifiers} class {cls.Identifier}";
        }

        public string GetPropertyString(PropertyDeclarationSyntax property)
        {
            var accessors = property.AccessorList.Accessors
                .Select(a => a.Keyword + ";");
            return $"{property.Modifiers} {property.Type} {property.Identifier} {{ {string.Join(" ", accessors)} }}";
        }

        public string GetFieldString(FieldDeclarationSyntax field, VariableDeclaratorSyntax variable)
        {
            return $"{field.Modifiers} {field.Declaration.Type} {variable.Identifier}";
        }

        public string GetMethodString(MethodDeclarationSyntax method)
        {
            return $"{method.Modifiers} {method.ReturnType} {method.Identifier}{method.ParameterList}";
        }
    }
}
