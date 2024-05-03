
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Ccom;

public partial class PropertyGroupDefinition
    : ICompositionParent<PropertyDefinition>, ICompositionParent<PropertyGroupDefinition>, ICompositionParent,
        ICompositionChild<PropertyGroupDefinition>, ICompositionChild<PropertySetDefinition>, ICompositionChild
{
    [XmlIgnore]
    public Entity? this[int index]
    {
        get => index >= Count ? null : index >= (PropertyDefinitions?.Length ?? 0) ? Group[index - (PropertyDefinitions?.Length ?? 0)] : PropertyDefinitions?[index];
        set
        {
            if (index >= (PropertyDefinitions?.Length ?? 0))
            {
                Group[index - (PropertyDefinitions?.Length ?? 0)] = (PropertyGroupDefinition)value!;
            }
            else
            {
                PropertyDefinitions![index] = (PropertyDefinition)value!;
            }
        }
    }
    [XmlIgnore]
    PropertyDefinition ICompositionParent<PropertyDefinition>.this[int index]
    {
        get => PropertyDefinitions[index]; set => PropertyDefinitions[index] = value;
    }
    [XmlIgnore]
    PropertyGroupDefinition ICompositionParent<PropertyGroupDefinition>.this[int index]
    {
        get => Group[index]; set => Group[index] = value;
    }

    [XmlIgnore]
    public int Count => (PropertyDefinitions?.Length ?? 0) + (Group?.Length ?? 0);

    /// <summary>
    /// Returns the parent PropertyGroupDefinition or PropertySetDefinition. (Not serialized)
    /// </summary>
    [XmlIgnore]
    public Entity? Parent { get; set; }

    [XmlIgnore]
    PropertyGroupDefinition? ICompositionChild<PropertyGroupDefinition>.Parent => Parent as PropertyGroupDefinition;

    [XmlIgnore]
    PropertySetDefinition? ICompositionChild<PropertySetDefinition>.Parent => Parent as PropertySetDefinition;

    [XmlIgnore]
    public InfoSource? IndirectInfoSource => Parent is ICompositionChild asChild ? Parent.InfoSource ?? asChild.IndirectInfoSource : Parent?.InfoSource;

    [XmlIgnore]
    public Entity? Root => Parent is ICompositionChild asChild ? asChild.Root : Parent;

    public IEnumerable<Entity> GetChildren()
    {
        return PropertyDefinitions.Cast<Entity>().Concat(Group);
    }

    [OnDeserialized]
    public void RepairChildren(StreamingContext streamingContext)
    {
        foreach (var p in PropertyDefinitions ?? Array.Empty<PropertyDefinition>()) p.Parent = this;
        foreach (var g in Group ?? Array.Empty<PropertyGroupDefinition>()) g.Parent = this;
    }

    IEnumerable<PropertyGroupDefinition> ICompositionParent<PropertyGroupDefinition>.GetChildren() => Group;

    IEnumerable<PropertyDefinition> ICompositionParent<PropertyDefinition>.GetChildren() => PropertyDefinitions;

    public void Add(PropertyDefinition newChild)
    {
        var oldSize = PropertyDefinitions.Length;
        Array.Resize(ref itemsField, oldSize + 1);
        PropertyDefinitions[oldSize] = newChild;
        Array.Resize(ref itemsElementNameField, oldSize + 1);
        PropertyDefinitionsName[oldSize] = PropertyDefinitionsName.LastOrDefault(ItemsChoiceType2.PropertyDefinition);
    }

    public void AddAll(IEnumerable<PropertyDefinition> newChildren)
    {
        PropertyDefinitions = PropertyDefinitions.Concat(newChildren).ToArray();
        PropertyDefinitionsName = new ItemsChoiceType2[PropertyDefinitions.Length];
        Array.Fill(PropertyDefinitionsName, PropertyDefinitionsName.LastOrDefault(ItemsChoiceType2.PropertyDefinition));
    }

    public void Add(PropertyGroupDefinition newChild)
    {
        var oldSize = Group.Length;
        Array.Resize(ref groupField, oldSize + 1);
        Group[oldSize] = newChild;
    }

    public void AddAll(IEnumerable<PropertyGroupDefinition> newChildren)
    {
        Group = Group.Concat(newChildren).ToArray();
    }

    // TODO Add additional parameters, such as description, etc. to make it easy
    // to create a more specified property object.

    /// <summary>
    /// Contructs a PropertyGroupDefinition with the given parameters. Provides a simpler
    /// mechanism to construct a property group definition, particularly if only one ShortName,
    /// etc., are necessary.
    /// It is recommended to use keyword parameters when calling to provide the
    /// specific parameters that are desired.
    /// </summary>
    /// <remarks>
    /// When the info source is implicitly inherited, the InfoSource object will not
    /// be set on the returned PropertyGroupDefinition, so as to not be serialized to XML.
    /// However, the info source can be retrieved via the IndirectInfoSource property:
    /// the parent group or parent set must be provided for this to provide a value.
    /// </remarks>
    /// <param name="shortName">The ShortName of the PropertyGroupDefinition</param>
    /// <param name="uuid">(optional) Specific UUID of the PropertyGroupDefinition</param>
    /// <param name="parentGroup">(optional) The PropertyGroupDefinition that is the parent of the property group definiiton</param>
    /// <param name="parentSet">(optional) The PropertySetDefinition that is the parent of the property group definiiton</param>
    /// <param name="implicitInfoSource">(optional) Whether the property group definition's info source is to be implicitly inherited. Default: true</param>
    public static PropertyGroupDefinition Create(string shortName,
        UUID? uuid = null,
        PropertyGroupDefinition? parentGroup = null,
        PropertySetDefinition? parentSet = null,
        bool implicitInfoSource = true)
    {
        var parentInfoSource = parentGroup?.InfoSource ?? parentSet?.InfoSource;

        return new PropertyGroupDefinition
        {
            UUID = uuid ?? UUID.Create(),
            InfoSource = implicitInfoSource ? null : parentInfoSource,
            ShortName = new TextType[]
            {
                shortName
            },
            Parent = parentGroup ?? parentSet as Entity
        };
    }

    public PropertyGroup InstantiateGroup(UUID? uuid = null, PropertyGroup? parentGroup = null, PropertySet? parentSet = null,
        UUIDProvider? uuidProvider = null, GroupUUIDProvider? groupUUIDProvider = null, PropertyDefinition.ValueProvider? valueProvider = null)
    {
        uuidProvider ??= DefaultUUIDProvider;
        groupUUIDProvider ??= DefaultGroupUUIDProvider;

        var group = PropertyGroup.Create(
            ShortName.FirstOrDefault("<unknown>").Value,
            uuid: uuid,
            definition: this,
            parentGroup: parentGroup,
            parentSet: parentSet
        );
        group.Order = Order is null ? null : new() { format = Order.format, Value = Order.Value };

        group.SetProperties = PropertyDefinitions?.Select(definition => definition.IsActive ?
            definition.InstantiateProperty(uuid: uuidProvider(definition, group), parentGroup: group, parentSet: parentSet, valueProvider: valueProvider)
            : null
        )
        .OfType<Property>() // exclude the nulls
        .ToArray() ?? Array.Empty<Property>();
        group.SetPropertiesName = new ItemsChoiceType7[group.SetProperties.Length];
        Array.Fill(group.SetPropertiesName, ItemsChoiceType7.SetProperty);

        group.Group = Group?.Select(definition => definition.IsActive ?
            definition.InstantiateGroup(uuid: groupUUIDProvider(definition, group), parentGroup: group, parentSet: parentSet, 
                    uuidProvider: uuidProvider, groupUUIDProvider: groupUUIDProvider, valueProvider: valueProvider)
            : null
        )
        .OfType<PropertyGroup>() // exclude the nulls
        .ToArray() ?? Array.Empty<PropertyGroup>();

        return group;
    }

    public delegate UUID UUIDProvider(PropertyDefinition definition, Entity? parent);
    public delegate UUID GroupUUIDProvider(PropertyGroupDefinition definition, Entity? parent);

    public static readonly UUIDProvider DefaultUUIDProvider = (_, _) => UUID.Create();
    public static readonly GroupUUIDProvider DefaultGroupUUIDProvider = (_, _) => UUID.Create();
}