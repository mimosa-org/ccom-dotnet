namespace Ccom;

public partial class PropertyType
{
    /// <summary>
    /// Returns a default value content based on the ValueClass of this type.
    /// If no ValueClass is set, returns null.
    /// </summary>
    /// <returns>An appropriate default value content for the set ValueClass or null.</returns>
    public ValueContent? DefaultValue()
    {
        switch (ValueClass)
        {
            case ValueClass.BinaryData:
                return new ValueContent() { Item = new BinaryData { Data = new BinaryObjectType() } };
            case ValueClass.BinaryObject:
                return new ValueContent() { Item = new BinaryObject { Value = new BinaryObjectType() } };
            case ValueClass.Boolean:
                return false;
            case ValueClass.Coordinate:
                return null;
            case ValueClass.EnumerationItem:
                return null;
            case ValueClass.Measure:
                return Measure.Create(0, new UnitOfMeasure {
                    UUID = UUID.Create("4d0d4ff6-9577-4f32-8592-a89c950a8d5b"),
                    ShortName = new TextType[] { "Undetermined" },
                    InfoSource = new InfoSource { UUID = UUID.Create("cf3f3a8a-1e42-4f15-9288-9cf2241e163d") }
                });
            case ValueClass.MultiParameter:
                return null;
            case ValueClass.Number:
                return 0;
            case ValueClass.Percentage:
                return 0;
            case ValueClass.Probability:
                return 0;
            case ValueClass.Text:
                return "";
            case ValueClass.URI:
                return null;
            case ValueClass.UTCDateTime:
                return DateTimeOffset.UtcNow;
            case ValueClass.UUID:
                return UUID.Create();
            case ValueClass.XML:
                return null;
            default:
                return null;
        }
    }
}