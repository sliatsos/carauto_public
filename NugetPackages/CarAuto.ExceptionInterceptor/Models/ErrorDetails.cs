using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CarAuto.ExceptionInterceptor.Models;

public class ErrorDetails
{
    public string StatusCode { get; set; }

    public string Message { get; set; }

    public string StackTrace { get; set; }

    public string CorrelationId { get; set; }

    public string ExceptionType { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
