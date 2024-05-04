using System.Diagnostics;
using Ccom;

namespace Ccom;

[DebuggerDisplay("'{Value}'")]
public partial class TextType
{
    public static implicit operator TextType(string? value)
    {
        return value is null ? null! : new TextType() { Value = value };
    }

    public static explicit operator string(TextType value) => value.Value;
}