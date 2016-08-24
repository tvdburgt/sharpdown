using System.IO;

namespace Sharpdown.Simple
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var template = File.OpenText(@"doc.md.hbs"))
            using (var output = File.CreateText(@"doc.md"))
            {
                Sharpdown.GenerateAsync(@"..\..\..\examples\SimpleCQRS\SimpleCQRS.csproj", template, output)
                    .GetAwaiter().GetResult();
            }
        }
    }
}
