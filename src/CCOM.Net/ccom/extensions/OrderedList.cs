using System.Numerics;
using Ccom;
using Microsoft.CSharp.RuntimeBinder;

namespace Ccom;

public partial class OrderedList
{
    public static OrderedEntityItem GetOrderedEntityItem(Entity entity, int order, UUID? uuid = null)
    {
        return new OrderedEntityItem() { 
            UUID = uuid ?? UUID.Create(),
            Item = entity,
            Order = (NumericType) order,
        };
    }

}