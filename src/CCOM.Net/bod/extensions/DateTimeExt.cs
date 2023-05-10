
namespace CommonBOD;

public static class DateTimeExt
{
    public static string ToXsDateTimeString(this DateTime self)
    {
        return self.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");
    }
}