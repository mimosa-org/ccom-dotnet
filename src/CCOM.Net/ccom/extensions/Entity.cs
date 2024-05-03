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
        /// <summary>
        /// Whether the 'EffectiveStatus' of the Entity is considered to be 'Active'.
        /// Returns `true` if: an EffectiveStatusType exists and is the 'Active' type
        /// from the MIMOSA CCOM Reference data; or (if the entity is a child entity)
        /// the parent is considered to be 'Active'; or if no EffectiveStatus info
        /// can be found. I.e., it defaults to `true`.
        /// </summary>
        public virtual bool IsActive => 
            EffectiveStatus?.OfType<EffectiveStatusType>().Any(e => e.UUID.Value == "db4bf287-2374-4e9c-bd4b-fadaada24b99")
            ?? (this is ICompositionChild c && (c.Parent?.IsActive ?? EffectiveStatus is null)) || EffectiveStatus is null;

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
        /// <param name="keepProperties">Optional collection of property names that should be kept in the reference</param>
        /// <returns>A copy of the entity with minimalistic information that can be serialised as a reference</returns>
        public static T ToReference<T>(this T entity,
                                       bool minimal = false,
                                       InfoSource? parentInfoSource = null,
                                       IEnumerable<string>? keepProperties = null) where T : Entity, new()
        {
            var entityInfoSource = entity.InfoSource ?? (entity is ICompositionChild cc ? cc.IndirectInfoSource : null);

            T copy = new()
            {
                UUID = entity.UUID,
                IDInInfoSource = minimal ? null : entity.IDInInfoSource,
                InfoSource = parentInfoSource is not null && parentInfoSource.UUID?.Value != entityInfoSource?.UUID?.Value
                    ? entityInfoSource?.ToReference(minimal: true)
                    : minimal || entity.IDInInfoSource is null ? null : entityInfoSource?.ToReference(minimal: true),
            };
            if (entity is INameable e && copy is INameable c && e.ShortName is not null)
            {
                c.ShortName = e.ShortName.Take(1).ToArray();
            }
            Type type = copy.GetType();
            foreach (var p in keepProperties ?? Array.Empty<string>())
            {
                var prop = type.GetProperty(p);
                var value = prop?.GetValue(entity);
                if (value is Entity ve)
                {
                    value = ve.ToReference(minimal: true, parentInfoSource: copy.InfoSource);
                }
                prop?.SetValue(copy, value);                
            }
            return copy;
        }

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
        /// <remarks>
        /// This version of the method uses reflection to construct the copy and is intended for when
        /// creating references recursively, e.g., via specifying `keepProperties`.
        /// </remarks>
        /// <param name="entity">The entity being copied</param>
        /// <param name="minimal">True to ensure only necessary information is included</param>
        /// <param name="parentInfoSource">Used to decide whether the InfoSource of entity can be excluded</param>
        /// <param name="keepProperties">Optional collection of property names that should be kept in the reference</param>
        /// <returns>A copy of the entity with minimalistic information that can be serialised as a reference</returns>
        internal static Entity ToReference(this Entity entity,
                                           bool minimal = false,
                                           InfoSource? parentInfoSource = null,
                                           IEnumerable<string>? keepProperties = null)
        {
            var entityInfoSource = entity.InfoSource ?? (entity is ICompositionChild cc ? cc.IndirectInfoSource : null);

            Entity copy = (Entity)entity.GetType().GetConstructor(Array.Empty<Type>())?.Invoke(null)!;
            copy.UUID = entity.UUID;
            copy.IDInInfoSource = minimal ? null : entity.IDInInfoSource;
            copy.InfoSource = parentInfoSource is not null && parentInfoSource.UUID?.Value != entityInfoSource?.UUID?.Value
                    ? entityInfoSource?.ToReference(minimal: true)
                    : minimal || entity.IDInInfoSource is null ? null : entityInfoSource?.ToReference(minimal: true);
            if (entity is INameable e && copy is INameable c && e.ShortName is not null)
            {
                c.ShortName = e.ShortName.Take(1).ToArray();
            }
            Type type = copy.GetType();
            foreach (var p in keepProperties ?? Array.Empty<string>())
            {
                var prop = type.GetProperty(p);
                var value = prop?.GetValue(entity);
                if (value is Entity ve)
                {
                    value = ve.ToReference(minimal: true, parentInfoSource: copy.InfoSource);
                }
                prop?.SetValue(copy, value);                
            }
            return copy;
        }

        /// <summary>
        /// Overload of ToReference<T> for PriorityLevelType that always includes the PriorityScale property.
        /// </summary>
        /// <param name="entity">The entity being copied</param>
        /// <param name="minimal">True to ensure only necessary information is included</param>
        /// <param name="parentInfoSource">Used to decide whether the InfoSource of entity can be excluded</param>
        /// <param name="keepProperties">Optional collection of property names that should be kept in the reference</param>
        /// <returns>A copy of the entity with minimalistic information that can be serialised as a reference</returns>
        public static PriorityLevelType ToReference(this PriorityLevelType entity, bool minimal = false,
                                                    InfoSource? parentInfoSource = null,
                                                    IEnumerable<string>? keepProperties = null)
        {
            var reference = ToReference<PriorityLevelType>(entity, minimal, parentInfoSource, keepProperties);
            reference.PriorityScale = (int)entity.PriorityScale;
            return reference;
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