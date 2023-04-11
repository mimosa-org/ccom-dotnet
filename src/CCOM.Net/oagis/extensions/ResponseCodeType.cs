namespace Oagis;

public partial class ResponseCodeType
{
    /// <summary>
    /// Enumeration of the 3 OAGIS defined values, plus one to indicate
    /// whether the value is a User Defined extension.
    /// </summary>
    public enum ResponseCodeEnum
    {
        UserDefined, Never, OnError, Always
    }

    public ResponseCodeEnum ValueAsEnum()
    {
        return Value switch
        {
            "Never" => ResponseCodeEnum.Never,
            "OnError" => ResponseCodeEnum.OnError,
            "Always" => ResponseCodeEnum.Always,
            _ => ResponseCodeEnum.UserDefined
        };
    }
}