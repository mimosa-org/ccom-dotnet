namespace Ccom {

    public partial class UUID {

        /// Summary:
        ///     Maintain default constructor for XML Serialization framework
        public UUID() {}

        /// Summary:
        ///     Add support for autogenerating the UUID value: specify true to auto-generate.
        public UUID(bool autogen) {
            if (autogen) {
                Value = Guid.NewGuid().ToString();
            }
        }

        /// Summary:
        ///     Convenience constructor for providing the UUID value.
        public UUID(String value) {
            Guid.Parse(value); // Raise exception if not a valid UUID.
            Value = value;
        }

    }

    public partial class Entity {
        

    }
}