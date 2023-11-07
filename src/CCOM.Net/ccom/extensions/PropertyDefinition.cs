
using System.Xml.Serialization;

namespace Ccom;

public partial class PropertyDefinition : ICompositionChild<PropertyGroupDefinition>, ICompositionChild<PropertySetDefinition>, ICompositionChild
{
    public bool IsBinaryData => Type?.ValueClass == ValueClass.BinaryData;
    public bool IsBinaryObject => Type?.ValueClass == ValueClass.BinaryObject;
    public bool IsBoolean => Type?.ValueClass == ValueClass.Boolean;
    public bool IsCoordinate => Type?.ValueClass == ValueClass.Coordinate;
    public bool IsEnumerationItem => Type?.ValueClass == ValueClass.EnumerationItem;
    public bool IsMeasure => Type?.ValueClass == ValueClass.Measure;
    public bool IsMultiParameter => Type?.ValueClass == ValueClass.MultiParameter;
    public bool IsNumber => Type?.ValueClass == ValueClass.Number;
    public bool IsPercentage => Type?.ValueClass == ValueClass.Percentage;
    public bool IsProbability => Type?.ValueClass == ValueClass.Probability;
    public bool IsText => Type?.ValueClass == ValueClass.Text;
    public bool IsURI => Type?.ValueClass == ValueClass.URI;
    public bool IsUTCDateTime => Type?.ValueClass == ValueClass.UTCDateTime;
    public bool IsUUID => Type?.ValueClass == ValueClass.UUID;
    public bool IsXML => Type?.ValueClass == ValueClass.XML;

    /// <summary>
    /// Returns the parent PropertyGroupDefinition or PropertySetDefinition. (Not serialized)
    /// </summary>
    [XmlIgnore]
    public Entity? Parent { get; set; }

    [XmlIgnore]
    public InfoSource? IndirectInfoSource => Parent is ICompositionChild asChild ? Parent.InfoSource ?? asChild.IndirectInfoSource : Parent?.InfoSource;

    [XmlIgnore]
    PropertyGroupDefinition? ICompositionChild<PropertyGroupDefinition>.Parent => Parent as PropertyGroupDefinition;

    [XmlIgnore]
    PropertySetDefinition? ICompositionChild<PropertySetDefinition>.Parent => Parent as PropertySetDefinition;

    [XmlIgnore]
    public Entity? Root => Parent is ICompositionChild asChild ? asChild.Root : Parent;

    // TODO Add additional parameters, such as description, etc. to make it easy
    // to create a more specified property object.

    /// <summary>
    /// Contructs a PropertyDefinition with the given parameters. Provides a simpler
    /// mechanism to construct a property, particularly if only one ShortName,
    /// etc., are necessary.
    /// It is recommended to use keyword parameters when calling to provide the
    /// specific parameters that are desired.
    /// </summary>
    /// <remarks>
    /// When the info source is implicitly inherited, the InfoSource object will not
    /// be set on the returned PropertyDefinition, so as to not be serialized to XML.
    /// However, the info source can be retrieved via the IndirectInfoSource property:
    /// the parent group or parent set must be provided for this to provide a value.
    /// </remarks>
    /// <param name="shortName">The ShortName of the PropertyDefinition</param>
    /// <param name="type">The PropertyType for the property</param>
    /// <param name="uuid">(optional) Specific UUID of the PropertyDefinition</param>
    /// <param name="parentGroup">(optional) The PropertyGroupDefinition that is the parent of the property definiiton</param>
    /// <param name="parentSet">(optional) The PropertySetDefinition that is the parent of the property definiiton</param>
    /// <param name="implicitInfoSource">(optional) Whether the property definition's info source is to be implicitly inherited. Default: true</param>
    public static PropertyDefinition Create(string shortName, PropertyType type,
        UUID? uuid = null,
        PropertyGroupDefinition? parentGroup = null,
        PropertySetDefinition? parentSet = null,
        bool implicitInfoSource = true)
    {
        var parentInfoSource = parentGroup?.InfoSource ?? parentSet?.InfoSource;

        return new PropertyDefinition
        {
            UUID = uuid ?? UUID.Create(),
            InfoSource = implicitInfoSource ? null : parentInfoSource,
            ShortName = new TextType[]
            {
                shortName
            },
            Type = type.ToReference(parentInfoSource: parentInfoSource ?? new InfoSource()),
            Parent = parentGroup ?? parentSet as Entity
        };
    }
}
