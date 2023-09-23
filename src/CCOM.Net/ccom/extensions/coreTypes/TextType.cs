using Ccom;

namespace Ccom;

public partial class TextType
{
    public static implicit operator TextType(string value)
    {
        return new TextType() { Value = value };
    }
}