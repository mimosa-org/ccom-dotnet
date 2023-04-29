namespace CommonBOD;

public class BODReaderSettings
{
    /// <summary>
    /// The 'XSD' folder at the location of the binaries or the working directory.
    /// </summary>
    public static readonly string DefaultSchemaPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()?.Location) ?? "XSD";

    /// <summary>
    /// Path to the folder containing the CCOM and OAGIS XML schemas.
    /// </summary>
    public string SchemaPath { get; set; } = DefaultSchemaPath;
}