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

    public partial class Entity {
        

    }
}