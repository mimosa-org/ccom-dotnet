using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace CommonBOD;

public class BODReader
{
    private readonly BODReaderSettings _settings;

    private XElement bod { get; init; }

    public bool IsValid { get; private set; } = true;

    public IEnumerable<ValidationEventArgs> ValidationErrors { get; init; } = new List<ValidationEventArgs>();

    public string SimpleName
    {
        get
        {
            return bod.Name.LocalName;
        }
    }

    public XName Name
    {
        get
        {
            return bod.Name;
        }
    }

    public BODReader(string fileUri, string schemaUri, BODReaderSettings settings)
    {
        _settings = settings;

        XmlReaderSettings xmlSettings = new XmlReaderSettings();
        xmlSettings.ValidationType = ValidationType.Schema;
        xmlSettings.ValidationEventHandler += (object? o, ValidationEventArgs e) => IsValid = IsValid && (e.Severity != XmlSeverityType.Error);
        xmlSettings.ValidationEventHandler += (object? o, ValidationEventArgs e) => ((List<ValidationEventArgs>)ValidationErrors).Add(e);
        xmlSettings.ValidationEventHandler += (object? o, ValidationEventArgs e) => Console.WriteLine("{0}: {1}", e.Severity.ToString(), e.Message);

        xmlSettings.Schemas.Add(Ccom.Cct.Namespace.URI, $"{_settings.SchemaPath}/CoreComponentType_2p0.xsd");
        xmlSettings.Schemas.Add(Ccom.Namespace.URI, $"{_settings.SchemaPath}/CCOM.xsd");
        xmlSettings.Schemas.Add(Oagis.Namespace.URI, $"{_settings.SchemaPath}/BOD/OAGIS/Meta.xsd");
        xmlSettings.Schemas.Add(Oagis.Namespace.URI, $"{_settings.SchemaPath}/BOD/OAGIS/Fields.xsd");
        xmlSettings.Schemas.Add(Oagis.Namespace.URI, $"{_settings.SchemaPath}/BOD/OAGIS/BOD.xsd");
        xmlSettings.Schemas.Add(Oagis.Namespace.URI, $"{_settings.SchemaPath}/BOD/OAGIS/ConfirmBOD.xsd");
        xmlSettings.Schemas.Add(Oagis.CodeLists.Namespace.URI, $"{_settings.SchemaPath}/BOD/OAGIS/CodeLists.xsd");
        xmlSettings.Schemas.Add(Oagis.QualifiedDataTypes.Namespace.URI, $"{_settings.SchemaPath}/BOD/OAGIS/QualifiedDataTypes.xsd");
        xmlSettings.Schemas.Add(Oagis.UnqualifiedDataTypes.Namespace.URI, $"{_settings.SchemaPath}/BOD/OAGIS/UnqualifiedDataTypes.xsd");
        xmlSettings.Schemas.Add(Oagis.CurrencyCode.Namespace.URI, $"{_settings.SchemaPath}/BOD/OAGIS/CodeList_CurrencyCode_ISO_7_04.xsd");
        xmlSettings.Schemas.Add(Oagis.LanguageCode.Namespace.URI, $"{_settings.SchemaPath}/BOD/OAGIS/CodeList_LanguageCode_ISO_7_04.xsd");
        xmlSettings.Schemas.Add(Oagis.IanaMimeMediaTypes.Namespace.URI, $"{_settings.SchemaPath}/BOD/OAGIS/CodeList_MIMEMediaTypeCode_IANA_7_04.xsd");
        xmlSettings.Schemas.Add(Oagis.UnitCode.Namespace.URI, $"{_settings.SchemaPath}/BOD/OAGIS/CodeList_UnitCode_UNECE_7_04.xsd");
        
        xmlSettings.Schemas.Add(Ccom.Namespace.URI, schemaUri);

        XmlReader reader = XmlReader.Create(fileUri, xmlSettings);

        bod = XElement.Load(reader, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
    }
}