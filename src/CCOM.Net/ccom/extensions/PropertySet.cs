

using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Ccom;

public partial class PropertySet : ICompositionParent<Property>, ICompositionParent<PropertyGroup>, ICompositionParent
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

    public void Add(Property newChild)
    {
        var oldSize = SetProperties.Length;
        Array.Resize(ref itemsField, oldSize + 1);
        SetProperties[oldSize] = newChild;
        Array.Resize(ref itemsElementNameField, oldSize + 1);
        SetPropertiesName[oldSize] = SetPropertiesName.LastOrDefault(ItemsChoiceType6.SetProperty);
    }

    public void Add(PropertyGroup newChild)
    {
        var oldSize = SetProperties.Length;
        Array.Resize(ref groupField, oldSize + 1);
        Group[oldSize] = newChild;
    }

    public void AddAll(IEnumerable<Property> newChildren)
    {
        SetProperties = SetProperties.Concat(newChildren).ToArray();
        SetPropertiesName = new ItemsChoiceType6[SetProperties.Length];
        Array.Fill(SetPropertiesName, SetPropertiesName.LastOrDefault(ItemsChoiceType6.SetProperty));
    }

    public void AddAll(IEnumerable<PropertyGroup> newChildren)
    {
        Group = Group.Concat(newChildren).ToArray();
    }

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

    IEnumerable<Property> ICompositionParent<Property>.GetChildren() => SetProperties;

    IEnumerable<PropertyGroup> ICompositionParent<PropertyGroup>.GetChildren() => Group;

    // TODO Add additional parameters, such as description, etc. to make it easy
    // to create a more specified property object.

    /// <summary>
    /// Contructs a PropertySet with the given parameters. Provides a simpler
    /// mechanism to construct a property set , particularly if only one ShortName,
    /// etc., are necessary.
    /// It is recommended to use keyword parameters when calling to provide the
    /// specific parameters that are desired.
    /// </summary>
    /// <param name="shortName">The ShortName of the PropertySet</param>
    /// <param name="type">The PropertySetType for the property set </param>
    /// <param name="uuid">(optional) Specific UUID of the property set </param>
    /// <param name="infoSource">(optional) The InfoSource for the property set : recommended to be set</param>
    /// <param name="definition">(optional) The PropertySetDefinition to which this proerty set conforms</param>
    public static PropertySet Create(string shortName, PropertySetType type,
        UUID? uuid = null,
        InfoSource? infoSource = null,
        PropertySetDefinition? definition = null)
    {
        return new PropertySet
        {
            UUID = uuid ?? UUID.Create(),
            InfoSource = infoSource,
            ShortName = new TextType[]
            {
                shortName
            },
            Definition = definition?.ToReference(parentInfoSource: infoSource ?? new InfoSource()),
            Type = type?.ToReference(parentInfoSource: infoSource ?? new InfoSource()),
        };
    }
}