using System.IO;

namespace Sharpdown.Simple
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var writer = File.CreateText("doc.md"))
            {
                Sharpdown.DocumentProjectAsync(@"..\..\..\examples\SimpleCQRS\SimpleCQRS.csproj", writer)
                    .GetAwaiter().GetResult();
            }
        }
    }
}
