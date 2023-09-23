using Ccom;

namespace Ccom;

public partial class Property
{
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
    /// <param name="type">(optional) The PropertyType for the property, mutually exclusive with defintion</param>
    /// <param name="definition">(optional) The PropertyDefinition for the property, mutually exclusive with type</param>
    public static Property Create(string shortName, ValueContent valueContent,
        PropertyType? type = null, PropertyDefinition? definition = null)
    {
        // TODO: throw exception if the type/definiiton does not match the ValueContent?
        return new Property
        {
            UUID = UUID.Create(),
            ShortName = new TextType[]
            {
                shortName
            },
            ValueContent = valueContent,
            TypeOrDefinition = (Entity)definition! ?? type
        };
    }
}