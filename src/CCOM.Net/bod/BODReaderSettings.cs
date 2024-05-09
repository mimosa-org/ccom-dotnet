namespace CommonBOD;

public class BODReaderSettings
{
    /// <summary>
    /// The 'XSD' folder at the location of the binaries or the working directory.
    /// </summary>
    public static readonly string DefaultSchemaPath = $"{Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()?.Location) ?? "."}/XSD";

    /// <summary>
    /// Configure the default validation setting (starts as On). Be careful as changes
    /// to this setting are application wide.
    /// </summary>
    public static ValidationSetting DefaultPerformValidation = ValidationSetting.On;

    /// <summary>
    /// Path to the folder containing the CCOM and OAGIS XML schemas.
    /// </summary>
    public string SchemaPath { get; set; } = DefaultSchemaPath;

    /// <summary>
    /// Whether schema validation should be performed at all and the severity of
    /// the errors.
    /// </summary>
    public ValidationSetting PerformValidation { get; set; } = DefaultPerformValidation;

    public enum ValidationSetting
    {
        /// <summary>
        /// No validation will be peformed by BODReader.
        /// </summary>
        Off,

        /// <summary>
        /// Schema validation will be performed, but the result will still be
        /// considered valid in the even of errors with the errors being reported
        /// as warnings.
        /// </summary>
        ErrorsAsWarnings,

        /// <summary>
        /// Perform normal Schema validation: errors will cause the BOD to be
        /// considered invalid and errors will be reported as such.
        /// </summary>
        On
    }
}