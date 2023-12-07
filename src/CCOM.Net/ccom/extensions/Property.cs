using System.Xml.Serialization;

namespace Ccom;

public partial class Property : ICompositionChild<PropertyGroup>, ICompositionChild<PropertySet>, ICompositionChild
{
    public bool IsBinaryData => ValueContent?.Item is BinaryData;
    public bool IsBinaryObject => ValueContent?.Item is BinaryObject;
    public bool IsBoolean => ValueContent?.Item is bool;
    public bool IsCoordinate => ValueContent?.Item is Coordinate;
    public bool IsEnumerationItem => ValueContent?.Item is EnumerationItem;
    public bool IsMeasure => ValueContent?.Item is Measure;
    public bool IsMultiParameter => ValueContent?.Item is MultiParameter;
    public bool IsNumber => ValueContent?.Item?.GetType() == typeof(NumericType); // exclude the subclasses
    public bool IsPercentage => ValueContent?.Item is Percentage;
    public bool IsProbability => ValueContent?.Item is Probability;
    public bool IsText => ValueContent?.Item is TextType;
    public bool IsURI => ValueContent?.Item is URI;
    public bool IsUTCDateTime => ValueContent?.Item is UTCDateTime;
    public bool IsUUID => ValueContent?.Item is UUID;
    public bool IsXML => ValueContent?.Item is XML;

    /// <summary>
    /// Returns the value (i.e., ValueContent.Item) of the Property as the
    /// specified type or null if the type does not match (applying conversion
    /// operators if available, such as TextType -> string).
    /// </summary>
    /// <typeparam name="T">
    /// The type to be returned (as defined by the ValueContent.Item and possible
    /// conversion operators)
    /// </typeparam>
    public T? GetValue<T>()
    {
        if (ValueContent is null) return default;
        return ValueContent.As<T>();
    }

    /// <summary>
    /// Returns the parent PropertyGroup or PropertySet. (Not serialized)
    /// </summary>
    [XmlIgnore]
    public Entity? Parent { get; set; }

    [XmlIgnore]
    public InfoSource? IndirectInfoSource => Parent is ICompositionChild asChild ? Parent.InfoSource ?? asChild.IndirectInfoSource : Parent?.InfoSource;

    [XmlIgnore]
    PropertyGroup? ICompositionChild<PropertyGroup>.Parent => Parent as PropertyGroup;

    [XmlIgnore]
    PropertySet? ICompositionChild<PropertySet>.Parent => Parent as PropertySet;

    [XmlIgnore]
    public Entity? Root => Parent is ICompositionChild asChild ? asChild.Root : Parent;

    // TODO Add additional parameters, such as description, etc. to make it easy
    // to create a more specified property object.

    /// <summary>
    /// Contructs a Property with the given parameters sans value. This provides a simpler
    /// mechanism to construct a property, particularly if only one ShortName,
    /// etc., are necessary.
    /// </summary>
    /// <param name="shortName">The ShortName of the Property</param>
    public static Property Create(string shortName)
    {
        return Create(shortName, new ValueContent());
    }

    /// <summary>
    /// Contructs a Property with the given parameters. This provides a simpler
    /// mechanism to construct a property, particularly if only one ShortName,
    /// etc., are necessary.
    /// It is recommended to use keyword parameters when calling to provide the
    /// specific parameters that are desired.
    /// </summary>
    /// <remarks>
    /// Implicit conversions defined for ValueContent apply, so a lot of
    /// convenience can be had by passing the more primitive values unless
    /// more control is needed over the structure.
    /// </remarks>
    /// <param name="shortName">The ShortName of the Property</param>
    /// <param name="valueContent">The ValueContent for the Property (implicit conversions apply)</param>
    /// <param name="uuid">(optional)Specific UUID of the Property</param>
    /// <param name="type">(optional) The PropertyType for the property, mutually exclusive with defintion</param>
    /// <param name="definition">(optional) The PropertyDefinition for the property, mutually exclusive with type</param>
    /// <param name="parentGroup">(optional) The PropertyGroup that is the parent of the property</param>
    /// <param name="parentSet">(optional) The PropertySet that is the parent of the property</param>
    /// <param name="implicitInfoSource">(optional) Whether the property definition's info source is to be implicitly inherited. Default: true</param>
    public static Property Create(string shortName, ValueContent valueContent, UUID? uuid = null,
        PropertyType? type = null, PropertyDefinition? definition = null,
        PropertyGroup? parentGroup = null, PropertySet? parentSet = null,
        bool implicitInfoSource = true)
    {
        var parentInfoSource = parentGroup?.InfoSource ?? parentSet?.InfoSource;

        // TODO: throw exception if the type/definiiton does not match the ValueContent?
        return new Property
        {
            UUID = uuid ?? UUID.Create(),
            InfoSource = implicitInfoSource ? null : parentInfoSource,
            ShortName = new TextType[]
            {
                shortName
            },
            ValueContent = valueContent,
            TypeOrDefinition = definition?.ToReference(parentInfoSource: parentInfoSource ?? new InfoSource())
                                ?? type?.ToReference(parentInfoSource: parentInfoSource ?? new InfoSource()) as Entity,
            Parent = parentGroup ?? parentSet as Entity,
        };
    }
}