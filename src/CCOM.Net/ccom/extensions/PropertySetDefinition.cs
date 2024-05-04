

using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Ccom;

public partial class PropertySetDefinition : ICompositionParent<PropertyDefinition>, ICompositionParent<PropertyGroupDefinition>, ICompositionParent
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

    public void Add(PropertyDefinition newChild)
    {
        var oldSize = PropertyDefinitions.Length;
        Array.Resize(ref itemsField, oldSize + 1);
        PropertyDefinitions[oldSize] = newChild;
        Array.Resize(ref itemsElementNameField, oldSize + 1);
        PropertyDefinitionsName[oldSize] = PropertyDefinitionsName.LastOrDefault(ItemsChoiceType1.PropertyDefinition);
    }

    public void Add(PropertyGroupDefinition newChild)
    {
        var oldSize = PropertyDefinitions.Length;
        Array.Resize(ref groupField, oldSize + 1);
        Group[oldSize] = newChild;
    }

    public void AddAll(IEnumerable<PropertyDefinition> newChildren)
    {
        PropertyDefinitions = PropertyDefinitions.Concat(newChildren).ToArray();
        PropertyDefinitionsName = new ItemsChoiceType1[PropertyDefinitions.Length];
        Array.Fill(PropertyDefinitionsName, PropertyDefinitionsName.LastOrDefault(ItemsChoiceType1.PropertyDefinition));
    }

    public void AddAll(IEnumerable<PropertyGroupDefinition> newChildren)
    {
        Group = Group.Concat(newChildren).ToArray();
    }

    public IEnumerable<Entity> GetChildren()
    {
        return (PropertyDefinitions?.Cast<Entity>() ?? Array.Empty<Entity>())
                .Concat(Group ?? Array.Empty<Entity>()).ToArray();
    }

    [OnDeserialized]
    public void RepairChildren(StreamingContext streamingContext)
    {
        foreach (var p in PropertyDefinitions ?? Array.Empty<PropertyDefinition>()) p.Parent = this;
        foreach (var g in Group ?? Array.Empty<PropertyGroupDefinition>()) g.Parent = this;
    }

    IEnumerable<PropertyDefinition> ICompositionParent<PropertyDefinition>.GetChildren() => PropertyDefinitions;

    IEnumerable<PropertyGroupDefinition> ICompositionParent<PropertyGroupDefinition>.GetChildren() => Group;

    // TODO Add additional parameters, such as description, etc. to make it easy
    // to create a more specified property object.

    /// <summary>
    /// Contructs a PropertySetDefinition with the given parameters. Provides a simpler
    /// mechanism to construct a property set definition, particularly if only one ShortName,
    /// etc., are necessary.
    /// It is recommended to use keyword parameters when calling to provide the
    /// specific parameters that are desired.
    /// </summary>
    /// <param name="shortName">The ShortName of the PropertySetDefinition</param>
    /// <param name="type">The PropertySetType for the property set definition</param>
    /// <param name="uuid">(optional) Specific UUID of the property set definition</param>
    /// <param name="infoSource">(optional) The InfoSource for the property set definition: recommended to be set</param>
    /// <param name="parentSet">(optional) The PropertySetDefinition that is the parent of the property set definiiton (inheritance/extension chains)</param>
    public static PropertySetDefinition Create(string shortName, PropertySetType type,
        UUID? uuid = null,
        InfoSource? infoSource = null,
        PropertySetDefinition? parentSet = null)
    {
        return new PropertySetDefinition
        {
            UUID = uuid ?? UUID.Create(),
            InfoSource = infoSource,
            ShortName = new TextType[]
            {
                shortName
            },
            Type = type.ToReference(parentInfoSource: infoSource ?? new InfoSource()),
            Parent = parentSet
        };
    }

    public PropertySet InstantiatePropertySet(UUID? uuid = null, InfoSource? infoSource = null,
        PropertyGroupDefinition.UUIDProvider? uuidProvider = null, 
        PropertyGroupDefinition.GroupUUIDProvider? groupUUIDProvider = null, 
        PropertyDefinition.ValueProvider? valueProvider = null,
        Func<Entity, bool>? includeIfPredicate = null)
    {
        uuidProvider ??= PropertyGroupDefinition.DefaultUUIDProvider;
        groupUUIDProvider ??= PropertyGroupDefinition.DefaultGroupUUIDProvider;
        includeIfPredicate ??= e => e.IsActive;

        var propertySet = PropertySet.Create(
            ShortName.FirstOrDefault("<unknown>").Value,
            Type,
            uuid: uuid,
            infoSource: infoSource,
            definition: this
        );

        propertySet.SetProperties = PropertyDefinitions?.Where(definition => includeIfPredicate(definition))
        .OrderBy(definition => (int?)definition.Order ?? int.MaxValue)
        .Select(definition =>
            definition.InstantiateProperty(uuid: uuidProvider(definition, propertySet), parentSet: propertySet, valueProvider: valueProvider)
        )
        .ToArray() ?? Array.Empty<Property>();
        propertySet.SetPropertiesName = new ItemsChoiceType6[propertySet.SetProperties.Length];
        Array.Fill(propertySet.SetPropertiesName, ItemsChoiceType6.SetProperty);

        propertySet.Group = Group?.Where(definition => includeIfPredicate(definition))
        .Select(definition =>
            definition.InstantiateGroup(uuid: groupUUIDProvider(definition, propertySet), parentSet: propertySet, 
                    uuidProvider: uuidProvider, groupUUIDProvider: groupUUIDProvider, valueProvider: valueProvider, 
                    includeIfPredicate: includeIfPredicate)
        )
        .ToArray() ?? Array.Empty<PropertyGroup>();

        // Instantiate the parent (if any) and merge
        if (Parent is not null)
        {
            var parentSet = Parent.InstantiatePropertySet(propertySet.UUID, infoSource, uuidProvider, groupUUIDProvider, valueProvider, includeIfPredicate);
            MergeSet(propertySet, parentSet);
        }

        // TODO: Instantiate included definitions (if any) and merge

        propertySet.RepairChildren(new StreamingContext()); // Update parent/child relations

        return propertySet;
    }

    private static PropertySet MergeSet(PropertySet target, PropertySet source)
    {
        target.SetProperties = (source.SetProperties ?? Array.Empty<Property>()).Concat(target.SetProperties ?? Array.Empty<Property>()).ToArray();
        target.SetPropertiesName = Enumerable.Repeat(ItemsChoiceType6.SetProperty, target.SetProperties.Length).ToArray();

        var groupsToMerge = source.Group?.Select(sourceG => (Source: sourceG, Target: target.Group.SingleOrDefault(targetG => targetG.ShortName.Any(sn => sourceG.HasShortName((string)sn))) ))
            .Where(pair => pair.Target is not null).ToArray();
        var sourceGroupsToAdd = source.Group?.Except(groupsToMerge?.Select(pair => pair.Source).ToArray() ?? Array.Empty<PropertyGroup>()) ?? Array.Empty<PropertyGroup>();
        
        target.Group = sourceGroupsToAdd.Concat(target.Group ?? Array.Empty<PropertyGroup>()).ToArray();

        foreach (var (Source, Target) in groupsToMerge ?? Array.Empty<(PropertyGroup, PropertyGroup)>())
        {
            MergeGroup(Target!, Source);
        }

        return target;
    }

    private static PropertyGroup MergeGroup(PropertyGroup target, PropertyGroup source)
    {
        target.SetProperties = (source.SetProperties ?? Array.Empty<Property>()).Concat(target.SetProperties ?? Array.Empty<Property>()).ToArray();
        target.SetPropertiesName = Enumerable.Repeat(ItemsChoiceType7.SetProperty, target.SetProperties.Length).ToArray();

        var groupsToMerge = source.Group?.Select(sourceG => (Source: sourceG, Target: target.Group.SingleOrDefault(targetG => targetG.ShortName.Any(sn => sourceG.HasShortName((string)sn))) ))
            .Where(pair => pair.Target is not null).ToArray()
            ?? Array.Empty<(PropertyGroup, PropertyGroup)>();
        var sourceGroupsToAdd = source.Group?.Except(groupsToMerge.Select(pair => pair.Source).ToArray())
            ?? Array.Empty<PropertyGroup>();

        target.Group = sourceGroupsToAdd.Concat(target.Group ?? Array.Empty<PropertyGroup>()).ToArray();

        foreach (var (Source, Target) in groupsToMerge ?? Array.Empty<(PropertyGroup, PropertyGroup)>())
        {
            MergeGroup(Target!, Source);
        }

        target.RepairChildren(new StreamingContext());

        return target;
    }
}