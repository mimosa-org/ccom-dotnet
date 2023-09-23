using Ccom;
using CommonBOD;

namespace Ccom;

/// <summary>
/// Representation of date/time in UTC with optional metadata indicating the local
/// timezone offsets.
/// </summary>
/// <remarks>
/// When implicitly converting a DateTime with a DateTimeKind.Unspecified, we assume
/// UTC as we cannot tell what timezone it is supposed to be.
/// </remarks
public partial class UTCDateTime
{
    public static implicit operator UTCDateTime(DateTime value)
    {
        if (value.Kind == DateTimeKind.Utc || value.Kind == DateTimeKind.Unspecified)
        {
            // Assume that Unspecified is UTC, since we cannot tell what timezone it is in
            return new UTCDateTime() { Value = value.ToXsDateTimeString() };
        }
        else
        {
            // Local time, incorporate the offset
            return (UTCDateTime)new DateTimeOffset(value);
        }
    }

    public static implicit operator UTCDateTime(DateTimeOffset value)
    {
        return new UTCDateTime()
        {
            Value = value.ToXsDateTimeString(),
            locHrDeltaFromUTC = value.Offset.Hours,
            locHrDeltaFromUTCFieldSpecified = value.Offset > TimeSpan.Zero,
            locMinDeltaFromUTC = value.Offset > TimeSpan.Zero ? value.Offset.Minutes.ToString() : null
    };
    }
}