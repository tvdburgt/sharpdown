using System.Collections.Generic;

namespace Sharpdown.Output
{
    public class TemplateData
    {
        public string Title { get; set; }
        public IEnumerable<TypeMetadata> NamedTypes { get; set; }
    }
}
