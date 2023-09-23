using Ccom;

namespace Ccom;

/// <summary>
/// A URI in the CCOM representation. Has an optional 'resourceName' as metadata.
/// </summary>
/// <remarks>
/// Conversion from string is explicit to avoid ambiguity with TextType (the more
/// common) target of the desired implicit conversion.
/// 
/// Absolute and relative URLs are handled and fully escaped in the implicit conversions.
/// If more control is required, construct a Ccom.URI and/or a System.Uri manually.
/// </remarks>
public partial class URI
{
    /// <summary>
    /// Create CCOM URI object with a correctly formatted and escaped URI string.
    /// </summary>
    /// <throws>
    /// UriFormatException if the Uri is not a valid
    /// </throws>
    /// <remarks>
    /// Note that a successfully instantiated Uri may throw a UriFormatException
    /// if it was created as a relative Uri, but is not well formed.
    /// Almost anything goes for a relative Uri, but the System.Uri class is a
    /// little too lenient.
    /// </remarks>
    public static URI Create(System.Uri uri)
    {
        if (!uri.IsAbsoluteUri && !uri.IsWellFormedOriginalString())
            throw new UriFormatException("Relative URI is invalid. This may be the result of an invalid absolute URI being interpreted as a relvative URI");

        return new Ccom.URI()
        {
            Value = uri.IsAbsoluteUri
                ? uri.GetComponents(UriComponents.AbsoluteUri, UriFormat.UriEscaped)
                : uri.ToString()
        };
    }

    /// <summary>
    /// Create CCOM URI object with a correctly formatted and escaped URI string.
    /// </summary>
    /// <throws>
    /// UriFormatException if the string is not a valid URI.
    /// </throws>
    public static URI Create(string uri)
    {
        return Create(new Uri(uri, UriKind.RelativeOrAbsolute));
    }

    public static implicit operator Ccom.URI(System.Uri uri) => Create(uri);
}