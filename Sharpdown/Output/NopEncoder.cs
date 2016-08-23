using HandlebarsDotNet;

namespace Sharpdown.Output
{
    /// <summary>
    /// Pass trough Markdown encoder that doesn't escape HTML-like symbols.
    /// </summary>
    internal class NopEncoder : ITextEncoder
    {
        public string Encode(string value)
        {
            return value;
        }
    }
}
