
using System.Globalization;

namespace CommonBOD;

public static class DateTimeExt
{
    public static string ToXsDateTimeString(this DateTime self, int precision = 7)
    {
        // Treat unspecified DateTime as UTC for consistency.
        // Unfortunately DateTime's are lossy as they have no offset/timezone info.
        return (self.Kind == DateTimeKind.Unspecified ? self : self.ToUniversalTime())
            .ToString($"yyyy'-'MM'-'dd'T'HH':'mm':'ss{(precision > 0 ? $".{new string('f', Math.Min(precision, 7))}" : "")}'Z'", CultureInfo.InvariantCulture);
    }

    public static string ToXsDateTimeString(this DateTimeOffset self, int precision = 7)
    {
        return self.ToUniversalTime().ToString($"yyyy'-'MM'-'dd'T'HH':'mm':'ss{(precision > 0 ? $".{new string('f', Math.Min(precision, 7))}" : "")}'Z'", CultureInfo.InvariantCulture);
    }
}