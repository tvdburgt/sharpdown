using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

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

        public string GetVariableString(FieldDeclarationSyntax field, VariableDeclaratorSyntax variable)
        {
            return $"{field.Modifiers} {field.Declaration.Type} {variable}";
        }

        public string GetMethodString(MethodDeclarationSyntax method)
        {
            return $"{method.Modifiers} {method.ReturnType} {method.Identifier}{method.ParameterList}";
        }
    }
}
