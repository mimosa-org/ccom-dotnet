
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Ccom;

public partial class PropertyGroup
    : ICompositionParent<Property>, ICompositionParent<PropertyGroup>, ICompositionParent,
        ICompositionChild<PropertyGroup>, ICompositionChild<PropertySet>, ICompositionChild
{
    [XmlIgnore]
    public Entity? this[int index]
    {
        get => index >= Count ? null : index >= (SetProperties?.Length ?? 0) ? Group[index - (SetProperties?.Length ?? 0)] : SetProperties?[index];
        set
        {
            if (index >= (SetProperties?.Length ?? 0))
            {
                Group[index - (SetProperties?.Length ?? 0)] = (PropertyGroup)value!;
            }
            else
            {
                SetProperties![index] = (Property)value!;
            }
        }
    }
    [XmlIgnore]
    Property ICompositionParent<Property>.this[int index]
    {
        get => SetProperties[index]; set => SetProperties[index] = value;
    }
    [XmlIgnore]
    PropertyGroup ICompositionParent<PropertyGroup>.this[int index]
    {
        get => Group[index]; set => Group[index] = value;
    }

    [XmlIgnore]
    public int Count => (SetProperties?.Length ?? 0) + (Group?.Length ?? 0);

    /// <summary>
    /// Returns the parent PropertyGroup or PropertySet. (Not serialized)
    /// </summary>
    [XmlIgnore]
    public Entity? Parent { get; set; }

    [XmlIgnore]
    PropertyGroup? ICompositionChild<PropertyGroup>.Parent => Parent as PropertyGroup;

    [XmlIgnore]
    PropertySet? ICompositionChild<PropertySet>.Parent => Parent as PropertySet;

    [XmlIgnore]
    public InfoSource? IndirectInfoSource => Parent is ICompositionChild asChild ? Parent.InfoSource ?? asChild.IndirectInfoSource : Parent?.InfoSource;

    [XmlIgnore]
    public Entity? Root => Parent is ICompositionChild asChild ? asChild.Root : Parent;

    public IEnumerable<Entity> GetChildren()
    {
        return SetProperties.Cast<Entity>().Concat(Group);
    }

    [OnDeserialized]
    public void RepairChildren(StreamingContext streamingContext)
    {
        _ = SetProperties.Select(p => p.Parent = this);
        _ = Group.Select(g => g.Parent = this);
    }

    IEnumerable<PropertyGroup> ICompositionParent<PropertyGroup>.GetChildren() => Group;

    IEnumerable<Property> ICompositionParent<Property>.GetChildren() => SetProperties;

    public void Add(Property newChild)
    {
        var oldSize = SetProperties.Length;
        Array.Resize(ref itemsField, oldSize + 1);
        SetProperties[oldSize] = newChild;
        Array.Resize(ref itemsElementNameField, oldSize + 1);
        SetPropertiesName[oldSize] = SetPropertiesName.FirstOrDefault(ItemsChoiceType7.SetProperty);
    }

    public void AddAll(IEnumerable<Property> newChildren)
    {
        SetProperties = SetProperties.Concat(newChildren).ToArray();
        var choiceType = SetPropertiesName.FirstOrDefault(ItemsChoiceType7.SetProperty));
        SetPropertiesName = new ItemsChoiceType7[SetProperties.Length];
        Array.Fill(SetPropertiesName, choiceType);
    }

    public void Add(PropertyGroup newChild)
    {
        var oldSize = Group.Length;
        Array.Resize(ref groupField, oldSize + 1);
        Group[oldSize] = newChild;
    }

    public void AddAll(IEnumerable<PropertyGroup> newChildren)
    {
        Group = Group.Concat(newChildren).ToArray();
    }

    // TODO Add additional parameters, such as description, etc. to make it easy
    // to create a more specified property object.

    /// <summary>
    /// Contructs a PropertyGroup with the given parameters. Provides a simpler
    /// mechanism to construct a property group , particularly if only one ShortName,
    /// etc., are necessary.
    /// It is recommended to use keyword parameters when calling to provide the
    /// specific parameters that are desired.
    /// </summary>
    /// <remarks>
    /// When the info source is implicitly inherited, the InfoSource object will not
    /// be set on the returned PropertyGroup, so as to not be serialized to XML.
    /// However, the info source can be retrieved via the IndirectInfoSource property:
    /// the parent group or parent set must be provided for this to provide a value.
    /// </remarks>
    /// <param name="shortName">The ShortName of the PropertyGroup</param>
    /// <param name="uuid">(optional) Specific UUID of the PropertyGroup</param>
    /// <param name="definition">(optional) The PropertyGroupDefinition for the property group</param>
    /// <param name="parentGroup">(optional) The PropertyGroup that is the parent of the property group definiiton</param>
    /// <param name="parentSet">(optional) The PropertySet that is the parent of the property group definiiton</param>
    /// <param name="implicitInfoSource">(optional) Whether the property group 's info source is to be implicitly inherited. Default: true</param>
    public static PropertyGroup Create(string shortName,
        UUID? uuid = null,
        PropertyGroupDefinition? definition = null,
        PropertyGroup? parentGroup = null,
        PropertySet? parentSet = null,
        bool implicitInfoSource = true)
    {
        var parentInfoSource = parentGroup?.InfoSource ?? parentSet?.InfoSource;

        return new PropertyGroup
        {
            UUID = uuid ?? UUID.Create(),
            InfoSource = implicitInfoSource ? null : parentInfoSource,
            ShortName = new TextType[]
            {
                shortName
            },
            Definition = definition?.ToReference(parentInfoSource: parentInfoSource ?? new InfoSource()),
            Parent = parentGroup ?? parentSet as Entity
        };
    }
}