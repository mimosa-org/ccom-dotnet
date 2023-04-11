using System.Xml.Schema;
using Oagis;

namespace CommonBOD;

public record class ValidationResult(
    XmlSeverityType Severity = XmlSeverityType.Error, 
    string Message = "", 
    int LineNumber = 0, 
    int LinePosition = 0)
{
    public static ValidationResult FromValidationEventArgs(ValidationEventArgs e)
    {
        return new ValidationResult(
            e.Severity,
            e.Message,
            e.Exception.LineNumber,
            e.Exception.LinePosition
        );
    }

    public bool IsError()
    {
        return Severity == XmlSeverityType.Error;
    }

    public bool IsWarning()
    {
        return Severity == XmlSeverityType.Warning;
    }

    public MessageType ToOagisMessage()
    {
        return new MessageType()
        {
            Description = new DescriptionType[]
            {
                new DescriptionType()
                {
                    languageID = "en-US", // TODO: suppoer internationalisation
                    Value = $"{Message} at Line: {LineNumber} Position: {LinePosition}"
                }
            }
        };
    }
}