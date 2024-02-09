using System.IO;
using Serilog.Formatting.Json;

namespace CarAuto.Logger;
public class JsonValueFormatterWithMaxLength : JsonValueFormatter
{
    private const int MaxStringLength = 10000;

    public static new void WriteQuotedJsonString(string str, TextWriter output)
    {
        if (str.Length > MaxStringLength)
        {
            str = $"{str.Substring(0, MaxStringLength)}[..]";
        }

        JsonValueFormatter.WriteQuotedJsonString(str, output);
    }

    protected override void FormatLiteralValue(object value, TextWriter output)
    {
        if (value is string valueAsString)
        {
            if (valueAsString.Length > MaxStringLength)
            {
                value = $"{valueAsString.Substring(0, MaxStringLength)}[..]";
            }
        }

        base.FormatLiteralValue(value, output);
    }
}
