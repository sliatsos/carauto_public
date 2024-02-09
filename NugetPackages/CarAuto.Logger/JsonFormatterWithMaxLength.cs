using System;
using System.IO;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Json;

namespace CarAuto.Logger;
public class JsonFormatterWithMaxLength : ITextFormatter
{
    private readonly JsonValueFormatterWithMaxLength _valueFormatter = new JsonValueFormatterWithMaxLength();

    public void Format(LogEvent logEvent, TextWriter output)
    {
        _ = logEvent ?? throw new ArgumentNullException(nameof(logEvent));
        _ = output ?? throw new ArgumentNullException(nameof(output));

        FormatEvent(logEvent, output, _valueFormatter);
        output.WriteLine();
    }

    private static void FormatEvent(LogEvent logEvent, TextWriter output, JsonValueFormatter valueFormatter)
    {
        output.Write("{");

        output.Write("\"Timestamp\":\"");
        output.Write(logEvent.Timestamp.UtcDateTime.ToString("O"));
        output.Write("\",");

        output.Write("\"MessageTemplate\":");
        JsonValueFormatterWithMaxLength.WriteQuotedJsonString(logEvent.MessageTemplate.Text, output);
        output.Write(",");

        output.Write("\"RenderedMessage\":");
        var renderedMessage = logEvent.MessageTemplate.Render(logEvent.Properties);
        JsonValueFormatterWithMaxLength.WriteQuotedJsonString(renderedMessage, output);
        output.Write(",");

        output.Write("\"Level\":\"");
        output.Write(logEvent.Level);
        output.Write("\"");

        if (logEvent.Exception != null)
        {
            output.Write(",");
            output.Write("\"Exception\":");
            JsonValueFormatterWithMaxLength.WriteQuotedJsonString(logEvent.Exception.ToString(), output);
        }

        if (logEvent.Properties.Count > 0)
        {
            output.Write(",");
            output.Write("\"Properties\":{");
            var first = true;
            foreach (var property in logEvent.Properties)
            {
                var name = property.Key;
                if (name.Length > 0 && name[0] == '@')
                {
                    name = '@' + name;
                }

                if (!first)
                {
                    output.Write(",");
                }

                first = false;
                JsonValueFormatter.WriteQuotedJsonString(name, output);
                output.Write(':');
                valueFormatter.Format(property.Value, output);
            }

            output.Write("}");
        }

        output.Write('}');
    }
}
