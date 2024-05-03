using Ccom;

namespace Ccom;

public partial class IDType
{
    public static implicit operator IDType(string? value)
    {
        return value is null ? null! : new IDType() { Value = value };
    }

    public static implicit operator string(IDType? value) => value?.Value!;
}