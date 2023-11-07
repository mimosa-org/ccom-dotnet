using System.Runtime.Serialization;

namespace Ccom;

public interface ICompositionParent
{
    public int Count { get; }
    public Entity? this[int index] { get; set; }
    public IEnumerable<Entity> GetChildren();

    /// <summary>
    /// Ensures the child entities have their parent set to this entity.
    /// </summary>
    /// <remarks>
    /// Implementers of this method must mark it with the [OnDeserialized]
    /// attribute to ensure it executes after deserialization from XML.
    /// </remarks>
    /// <param name="streamingContext"></param>
    [OnDeserialized]
    public void RepairChildren(StreamingContext streamingContext);
}

public interface ICompositionParent<T> where T : Entity, ICompositionChild, IEntity<T>, new()
{
    public int Count { get; }
    public T this[int index] { get; set; }

    public void Add(T newChild);
    public void AddAll(IEnumerable<T> newChildren);

    public IEnumerable<T> GetChildren();
}