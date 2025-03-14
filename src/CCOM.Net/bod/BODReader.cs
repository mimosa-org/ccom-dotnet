using System;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Linq;
using Oagis;
using Ccom.Xml.Serialization;
namespace CommonBOD;

public class BODReader
{
    private readonly BODReaderSettings _settings;

    private XDocument bodXml { get; init; }
    private string bodText { get; init; } // keeps the original text if the BOD is invalid
    private BusinessObjectDocumentType? bod { get; set; }

    public bool IsValid { get; private set; } = true;

    public IEnumerable<ValidationResult> ValidationErrors { get; init; } = new List<ValidationResult>();

    public string SimpleName
    {
        get
        {
            return bodXml.Root?.Name?.LocalName ?? "";
        }
    }

    public XName Name
    {
        get
        {
            return bodXml.Root?.Name ?? "";
        }
    }

    public string ReleaseID
    {
        get
        {
            return bodXml.Root?.Attribute("releaseID")?.Value ?? "";
        }
    }

    public string? VersionID
    {
        get
        {
            return bodXml.Root?.Attribute("versionID")?.Value;
        }
    }

    public string SystemEnvironmentCode
    {
        get
        {
            return bodXml.Root?.Attribute("systemEnvironmentCode")?.Value ?? Oagis.CodeLists.SystemEnvironmentCodeEnumerationType.Production.ToString();
        }
    }

    public string LanguageCode
    {
        get
        {
            return bodXml.Root?.Attribute("languageCode")?.Value ?? "en-US";
        }
    }

    private ApplicationAreaType? _applicationArea = null;
    public ApplicationAreaType ApplicationArea
    {
        get
        {
            return _applicationArea ??= bodXml.Root is null ? tryRecoverApplicationArea() : readApplicationArea();            
        }
    }

    public ResponseCodeType.ResponseCodeEnum RequiresConfirmation
    {
        get
        {
            return ApplicationArea.Sender?.ConfirmationCode?.ValueAsEnum()
                    ?? ResponseCodeType.ResponseCodeEnum.Never;
        }
    }

    private VerbType? _verb = null;
    public VerbType Verb
    {
        get
        {
            return _verb ??= readVerb();
        }
    }

    public IEnumerable<XElement> Nouns
    {
        get
        {
            return dataAreaElement()?.Elements()?.Skip(1) ?? XElement.EmptySequence;
        }
    }

    public BODReader(string fileUri, string schemaUri, BODReaderSettings settings)
        : this(new StreamReader(fileUri), schemaUri, settings)
    { }

    public BODReader(TextReader input, string schemaUri, BODReaderSettings settings)
    {
        _settings = settings;

        // FIXME: this double handling will be a problem for large messages
        var allContent = input.ReadToEnd();
        input.Close();
        var newInput = new StringReader(allContent);

        XmlReaderSettings xmlSettings = new XmlReaderSettings();
        xmlSettings.ValidationType = _settings.PerformValidation switch
        {
           BODReaderSettings.ValidationSetting.Off => ValidationType.None,
           _ => ValidationType.Schema 
        };
        if (_settings.PerformValidation == BODReaderSettings.ValidationSetting.ErrorsAsWarnings)
        {
            // Will always be valid with errors passed on as Warnings
            IsValid = true;
            xmlSettings.ValidationEventHandler += (object? o, ValidationEventArgs e)
             => ((List<ValidationResult>)ValidationErrors).Add(ValidationResult.FromValidationEventArgs(e) with { Severity = XmlSeverityType.Warning });
            xmlSettings.ValidationEventHandler += (object? o, ValidationEventArgs e)
             => Console.WriteLine("{0} (Converted to warning): {1}", e.Severity.ToString(), e.Message);
        }
        else
        {
            // Standard validation, errors are passed on as errors and the IsValid == false if an error occurs
            xmlSettings.ValidationEventHandler += (object? o, ValidationEventArgs e) => IsValid = IsValid && (e.Severity != XmlSeverityType.Error);
            xmlSettings.ValidationEventHandler += (object? o, ValidationEventArgs e) => ((List<ValidationResult>)ValidationErrors).Add(ValidationResult.FromValidationEventArgs(e));
            xmlSettings.ValidationEventHandler += (object? o, ValidationEventArgs e) => Console.WriteLine("{0}: {1}", e.Severity.ToString(), e.Message);
        }

        xmlSettings.Schemas.Add(Ccom.Cct.Namespace.URI, $"{_settings.SchemaPath}/CoreComponentType_2p0.xsd");
        xmlSettings.Schemas.Add(Ccom.Namespace.URI, $"{_settings.SchemaPath}/CCOM.xsd");
        xmlSettings.Schemas.Add(Ccom.Namespace.URI, $"{_settings.SchemaPath}/BOD/Messages/CCOMQuery.xsd");
        xmlSettings.Schemas.Add(Ccom.Namespace.URI, $"{_settings.SchemaPath}/BOD/Messages/CCOMElements.xsd");
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
        
        if (!String.IsNullOrWhiteSpace(schemaUri))
        {
            xmlSettings.Schemas.Add(Ccom.Namespace.URI, schemaUri);
        }

        XmlReader reader = XmlReader.Create(newInput, xmlSettings);

        try
        {
            bodXml = XDocument.Load(reader, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo);
            bodText = "";
        }
        catch (XmlException e)
        {
            IsValid = false;
            // We prepend syntax errors as schema validation errors may have already been added before the syntax error was reached.
            ((List<ValidationResult>)ValidationErrors).Insert(0, 
                new ValidationResult(XmlSeverityType.Error, e.Message, e.LineNumber, e.LinePosition)
            );
            bodXml = new XDocument();
            bodText = allContent;
            bod = new BusinessObjectDocumentType();
        }
    }

    /// <summary>
    /// Returns the read BOD as the deserialized BOD type (or null if wrong type or not valid).
    /// </summary>
    /// <remarks>
    /// If the BODReader reports IsValid == false, this method will return null.
    /// </remarks>
    /// <typeparam name="T">The expected BOD type</typeparam>
    /// <returns>The deserialized BOD, or null</returns>
    public T? AsBod<T>() where T : BusinessObjectDocumentType, new()
    {
        if (!IsValid) return null;
        if (bod is not null) return bod as T;

        if (typeof(T).IsAssignableTo(typeof(GenericBodType)) 
            && (typeof(T).Name == "GenericBodType" // avoid doing constructor lookup twice if simple case
                || typeof(T).GetConstructor(new[] { typeof(XDocument), typeof(string), typeof(string), typeof(string), typeof(string) }) is not null))
        {
            bod = Activator.CreateInstance(typeof(T), bodXml, SimpleName, Ccom.Namespace.URI, "ccom", Nouns.FirstOrDefault()?.Name.LocalName) as BusinessObjectDocumentType;
            return (T?)bod;
        }
        else
        {
            return (T?)(bod = BusinessObjectDocumentType.Deserialize<T>(bodXml));
        }
    }

    public ConfirmBODType GenerateConfirmBOD(MessageType? msg = null)
    {
        var confirmBOD = new ConfirmBODType()
        {
            languageCode = "en-US",
            releaseID = "9.0",
            systemEnvironmentCode = SystemEnvironmentCode,
            ApplicationArea = new ApplicationAreaType()
            {
                BODID = new IdentifierType()
                {
                    Value = Guid.NewGuid().ToString()
                },
                CreationDateTime = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'"),
                // TODO: sender ID, parameter or config?
            },
            DataArea = new ConfirmBODDataAreaType()
            {
                Confirm = new ConfirmType()
                {
                    OriginalApplicationArea = ApplicationArea
                },
                BOD = new BODType[] { new BODType() }
            },
        };

        if (IsValid)
        {
            confirmBOD.DataArea.BOD[0].BODSuccessMessage = new BODSuccessMessageType();
            if (ValidationErrors.Any())
            {
                confirmBOD.DataArea.BOD[0].BODSuccessMessage.WarningProcessMessage = ValidationErrors.Select(r => r.ToOagisMessage()).ToArray();
            }
            SuccessMessageType successMsg = new()
            {
                ProcessMessage = msg == null ? null : new MessageType[] { msg }
            };
            confirmBOD.DataArea.BOD[0].BODSuccessMessage.NounSuccessMessage = new SuccessMessageType[] { successMsg };
        }
        else
        {
            confirmBOD.DataArea.BOD[0].BODFailureMessage = new BODFailureMessageType()
            {
                ErrorProcessMessage = (from error in ValidationErrors
                                       where error.IsError()
                                       select error.ToOagisMessage()).ToArray(),
                WarningProcessMessage = (from warning in ValidationErrors
                                         where warning.IsWarning()
                                         select warning.ToOagisMessage()).ToArray()
            };
        }

        return confirmBOD;
    }

    private ApplicationAreaType tryRecoverApplicationArea()
    {
        // Attempt to recover BOD header information by looking at at the text in the event of a syntactic error.
        // TODO: how flexible should we be in trying to recover this info?
        // TODO: what else should we attempt to recover?
        var applicationArea = new ApplicationAreaType();
        Match match;
        var options = RegexOptions.ExplicitCapture;

        match = Regex.Match(bodText, @"ApplicationArea(.|\n)+BODID[^>]*>(?<value>[^<]+)<([^:]+:)?BODID(.|\n)+ApplicationArea", options);
        applicationArea.BODID = match.Success ? new IdentifierType() { Value = match.Groups[1].Value } : null;
        
        match = Regex.Match(bodText, @"ApplicationArea(.|\n)+CreationDateTime[^>]*>(?<value>[^<]+)<([^:]+:)?CreationDateTime(.|\n)+ApplicationArea", options);
        applicationArea.CreationDateTime = match.Success ? match.Groups[1].Value : null;

        match = Regex.Match(bodText, "ApplicationArea(.|\n)+Sender(.|\n)+LogicalID[^>]*>(?<value>[^<]+)<([^:]+:)?LogicalID(.|\n)+Sender(.|\n)+ApplicationArea", options);
        if (match.Success)
        {
            applicationArea.Sender ??= new SenderType();
            applicationArea.Sender.LogicalID = new IdentifierType() { Value = match.Groups[1].Value };
        }

        match = Regex.Match(bodText, "ApplicationArea(.|\n)+Sender(.|\n)+ConfirmationCode[^>]*>(?<value>[^<]+)<([^:]+:)?ConfirmationCode(.|\n)+Sender(.|\n)+ApplicationArea", options);
        if (match.Success)
        {
            applicationArea.Sender ??= new SenderType();
            applicationArea.Sender.ConfirmationCode = new ConfirmationResponseCodeType() { Value = match.Groups[1].Value };
        }

        return applicationArea;
    }

    private ApplicationAreaType readApplicationArea()
    {
        try
        {
            return deserializeOrDefault<ApplicationAreaType, ApplicationAreaType>(bodXml.Root!.Elements().First().CreateReader());
        }
        catch (InvalidOperationException e)
        {
            Console.Error.WriteLine("No ApplicationArea when BOD expected: {0}", e);
            // Valid XML but no ApplicationArea; should already be marked invalid, so return empty application area.
            return _applicationArea ??= new ApplicationAreaType();
        }
    }

    private XElement? dataAreaElement()
    {
        return bodXml.Root?.Elements().FirstOrDefault(x => x.Name.LocalName == "DataArea");
    }

    private VerbType readVerb()
    {
        var verbElement = dataAreaElement()?.Elements()?.FirstOrDefault();

        var verbType = verbElement?.Name?.LocalName switch
        {
            "ResponseVerb" => typeof(ResponseVerbType),
            "Show" => typeof(ShowType),
            "Respond" => typeof(RespondType),
            "Confirm" => typeof(ConfirmType),
            "Acknowledge" => typeof(AcknowledgeType),
            "RequestVerb" => typeof(RequestVerbType),
            "Get" => typeof(GetType),
            "ActionVerb" => typeof(ActionVerbType),
            "Update" => typeof(UpdateType),
            "Sync" => typeof(SyncType),
            "Process" => typeof(ProcessType),
            "Post" => typeof(PostType),
            "Notify" => typeof(NotifyType),
            "Load" => typeof(LoadType),
            "Change" => typeof(ChangeType),
            "Cancel" => typeof(CancelType),
            _ => typeof(UnknownVerbType)
        };

        return deserializeOrDefault<VerbType, UnknownVerbType>(verbElement?.CreateReader(), verbType);
    }

    private T deserializeOrDefault<T, U>(XmlReader? reader, Type? t = null) where T : class where U : T, new()
    {
        if (reader is null) return new U();
        return new XmlCallbackSerializer( t ?? typeof(T)).Deserialize(reader) as T ?? new U();
    }

    private class UnknownVerbType : VerbType
    {
        // A placeholder class to be returned when a BOD has no known VerbType.
    }
}