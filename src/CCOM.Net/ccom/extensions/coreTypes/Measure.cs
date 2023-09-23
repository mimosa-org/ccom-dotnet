using Ccom;

namespace Ccom;

public partial class Measure
{
    public static Measure Create(NumericType value, UnitOfMeasure unit)
    {
        return new Measure() { Value = value, UnitOfMeasure = unit };
    }
}