namespace Ccom {

    public partial class UUID {

        /// Summary:
        ///     Maintain default constructor for XML Serialization framework
        public UUID() {}

        /// Summary:
        ///     Add support for autogenerating the UUID value: specify true to auto-generate.
        /// Deprecated
        public UUID(bool autogen) {
            if (autogen) {
                Value = Guid.NewGuid().ToString();
            }
        }

        /// Summary:
        ///     Convenience constructor for providing the UUID value.
        /// Deprecated
        public UUID(string value) {
            Guid.Parse(value); // Raise exception if not a valid UUID.
            Value = value;
        }

        /// <summary>
        /// Returns a new randomly generated UUID (type 4).
        /// </summary>
        public static UUID Create()
        {
            return new UUID() { Value = Guid.NewGuid().ToString() };
        }

        /// <summary>
        /// Returns a new UUID initialised from the given Guid.
        /// </summary>
        /// <param name="value"></param>
        public static UUID Create(Guid value)
        {
            return new UUID() { Value = value.ToString() };
        }

        /// <summary>
        /// Returns a new UUID initialised from the string representation.
        /// </summary>
        /// <param name="value">a valid UUID string representation</param>
        /// <exceptions>
        /// ArgumentNullException
        /// FormatException
        /// </exceptions>
        public static UUID Create(string value)
        {
            Guid.Parse(value); // Raise exception if not a valid UUID.
            return new UUID() { Value = value };
        }

        public static implicit operator UUID(Guid value) => Create(value);
        public static explicit operator Guid(UUID value) => Guid.Parse(value.Value);
    }

    public abstract partial class Entity
    {


    }

    public interface IEntity<T> where T : Entity, new()
    {
    }

    public static class EntityExtensions
    {
        /// <summary>
        /// Provides a copy of the Entity that is suitable to be used as a serializable reference.
        /// By default includes the UUID, IDInInfoSource, InfoSource (if there is an IDInInfoSource),
        /// and a single ShortName (if a nameable entity).
        /// 
        /// When minimal is true, includes only: UUID and ShortName (if a nameable entity)
        /// 
        /// When a parentInfoSource is provided, the InfoSource of the entity will be included if it
        /// differs from the parentInfoSource.
        /// </summary>
        /// <typeparam name="T">The entity's concrete type</typeparam>
        /// <param name="entity">The entity being copied</param>
        /// <param name="minimal">True to ensure only necessary information is included</param>
        /// <param name="parentInfoSource">Used to decide whether the InfoSource of entity can be excluded</param>
        /// <returns>A copy of the entity with minimalistic information that can be serialised as a reference</returns>
        public static T ToReference<T>(this T entity, bool minimal = false, InfoSource? parentInfoSource = null) where T : Entity, new()
        {
            T copy = new()
            {
                UUID = entity.UUID,
                IDInInfoSource = minimal ? null : entity.IDInInfoSource,
                InfoSource = parentInfoSource is not null && parentInfoSource.UUID.Value != entity.InfoSource?.UUID.Value
                    ? entity.InfoSource?.ToReference(minimal: true)
                    : minimal || entity.IDInInfoSource is null ? null : entity.InfoSource?.ToReference(minimal: true),
            };
            if (entity is INameable e && copy is INameable c && e.ShortName is not null)
            {
                c.ShortName = e.ShortName.Take(1).ToArray();
            }
            return copy;
        }

        public static bool HasShortName(this Entity? entity, string name)
        {
            if (entity is INameable nameable) return nameable.ShortName?.Any(sn => sn.Value == name) ?? false;
            else return false;
        }

        public static bool HasFullName(this Entity? entity, string name)
        {
            if (entity is INameable nameable) return nameable.FullName?.Any(fn => fn.Value == name) ?? false;
            else return false;
        }
    }

    public interface INameable
    {
        public abstract TextType[] ShortName { get; set; }
        public abstract TextType[] FullName { get; set; }
        public abstract TextType[] Description { get; set; }
    }

    public interface IRelationship
    {
        // Marker interface for the moment
    }
}