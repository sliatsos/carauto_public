using CarAuto.ExceptionInterceptor.CustomExceptions;
using CarAuto.ExceptionInterceptor.Models;
using CorrelationId.Abstractions;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.Json;

namespace CarAuto.ExceptionInterceptor;

public class ExceptionInterceptor : Interceptor
{
    private static readonly Dictionary<Type, StatusCode> ExceptionToStatusCodeMapping = new Dictionary<Type, StatusCode>
    {
        { typeof(Exception), StatusCode.Internal },
        { typeof(ApplicationException), StatusCode.Internal },
        { typeof(SystemException), StatusCode.Internal },
        { typeof(RpcException), StatusCode.Internal },
        { typeof(NullReferenceException), StatusCode.Internal },
        { typeof(AbandonedMutexException), StatusCode.Internal },
        { typeof(InvalidDataException), StatusCode.Internal },
        { typeof(FormatException), StatusCode.Internal },
        { typeof(InvalidOperationException), StatusCode.Internal },
        { typeof(TimeoutException), StatusCode.Internal },
        { typeof(AccessViolationException), StatusCode.Internal },
        { typeof(ArgumentException), StatusCode.InvalidArgument },
        { typeof(ArgumentNullException), StatusCode.InvalidArgument },
        { typeof(ValidationException), StatusCode.InvalidArgument },
        { typeof(ArgumentOutOfRangeException), StatusCode.OutOfRange },
        { typeof(DivideByZeroException), StatusCode.OutOfRange },
        { typeof(IndexOutOfRangeException), StatusCode.OutOfRange },
        { typeof(NotImplementedException), StatusCode.Unimplemented },
        { typeof(UnauthorizedAccessException), StatusCode.PermissionDenied },
        { typeof(DuplicateNameException), StatusCode.AlreadyExists },
        { typeof(AlreadyExistsException), StatusCode.AlreadyExists },
        { typeof(NotFoundException), StatusCode.NotFound },
    };

    private readonly ILogger<ExceptionInterceptor> _logger;
    private readonly ICorrelationContextAccessor _correlationContextAccessor;

    /// <summary>
    /// Default constructor for ExceptionInterceptor.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="correlationContextAccessor"></param>
    public ExceptionInterceptor(ILogger<ExceptionInterceptor> logger, ICorrelationContextAccessor correlationContextAccessor)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _correlationContextAccessor = correlationContextAccessor ?? throw new ArgumentNullException(nameof(correlationContextAccessor));
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            _logger.LogInformation($"Executing {context.Method} with request object '{JsonSerializer.Serialize(request)}'");
            var response = await continuation(request, context).ConfigureAwait(false);
            _logger.LogInformation($"Executed {context.Method}. Returning response object '{JsonSerializer.Serialize(response)}'");
            return response;
        }
        catch (Exception exception)
        {
            var statusCode = TranslateExceptionTypeToStatusCode(exception);
            var errorDetails = GetErrorDetails(exception, statusCode, _correlationContextAccessor);
            _logger.LogError(exception, errorDetails);
            throw new RpcException(new Status(statusCode, errorDetails));
        }
    }

    private static StatusCode TranslateExceptionTypeToStatusCode(Exception exception)
    {
        if (!ExceptionToStatusCodeMapping.TryGetValue(exception.GetType(), out var status))
        {
            status = StatusCode.Unknown;
        }

        return status;
    }

    private static string GetErrorDetails(Exception exception, StatusCode status, ICorrelationContextAccessor accessor)
    {
        return new ErrorDetails
        {
            ExceptionType = exception.GetType().ToString(),
            Message = exception.Message,
            StackTrace = exception.StackTrace,
            StatusCode = status.ToString(),
            CorrelationId = accessor?.CorrelationContext?.CorrelationId,
        }.ToString();
    }
}