namespace Ccom;

public interface ICompositionChild
{
    /// <summary>
    /// Returns the parent entity. (Not serialized)
    /// </summary>
    public Entity? Parent { get; }

    /// <summary>
    /// Returns the root of this compositional hiearchy.
    /// </summary>
    public Entity? Root { get; }

    /// <summary>
    /// Returns the InfoSource from the parent.
    /// Implementations may recurse up the hierarchy.
    /// </summary>
    public InfoSource? IndirectInfoSource { get; }
}

public interface ICompositionChild<T> where T : Entity, ICompositionParent, IEntity<T>, new()
{
    /// <summary>
    /// Returns the parent entity of type T. (Not serialized)
    /// </summary>
    public T? Parent { get; }

    /// <summary>
    /// Returns the root of this compositional hiearchy.
    /// </summary>
    public Entity? Root { get; }

    /// <summary>
    /// Returns the InfoSource from the parent.
    /// Implementations may recurse up the hierarchy.
    /// </summary>
    public InfoSource? IndirectInfoSource { get; }
}