using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using CorrelationId;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CarAuto.GrpcClientWrapper
{
    public class GrpcClientWrapper : IGrpcClientWrapper
    {
        private readonly Uri _apiGatewayUrl;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<GrpcClientWrapper> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GrpcClientWrapper(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<GrpcClientWrapper> logger, IHttpContextAccessor httpContextAccessor)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            string apiGatewayUrl = configuration.GetValue<string>("ApiGateway:url") ?? throw new ArgumentNullException(
                $"ApiGateway configuration is missing in the {nameof(configuration)}.");
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

            _apiGatewayUrl = new Uri(apiGatewayUrl);
        }

        public bool Http2UnencryptedSupport { get; set; }

        public TResponse CallGrpcService<TClient, TInput, TResponse>(TInput parameters, string methodName, Metadata headers)
        {
            ValidateParameters(_apiGatewayUrl, parameters, methodName);
            return CallGrpcInternalAsync<TClient, TInput, TResponse>(parameters, methodName, headers).GetAwaiter().GetResult();
        }

        public async Task<TResponse> CallGrpcServiceAsync<TClient, TInput, TResponse>(TInput parameters, string methodName, Metadata headers)
        {
            ValidateParameters(_apiGatewayUrl, parameters, methodName);

            return await CallGrpcInternalAsync<TClient, TInput, TResponse>(parameters, methodName, headers);
        }

        protected virtual TResponse Invoke<TResponse>(MethodInfo serviceMethod, object clientChannel, object request, CallOptions options)
        {
            var response = serviceMethod.Invoke(clientChannel, new[] { (object)request, options });
            _logger.LogInformation($"grpc service is invoked and received response {response}");
            return (TResponse)response;
        }

        private static void ValidateParameters<TInput>(Uri serviceUri, TInput parameters, string serviceName)
        {
            if (serviceUri == null)
            {
                throw new ArgumentNullException(nameof(serviceUri));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (serviceName == null)
            {
                throw new ArgumentNullException(nameof(serviceName));
            }
        }

        private async Task<TResponse> CallGrpcInternalAsync<TClient, TInput, TResponse>(TInput request, string methodName, Metadata headers)
        {
            _logger.LogInformation(
                 $"CallGrpcService triggered with TClient:'{typeof(TClient)}',TInput:'{typeof(TInput)}',TResponse:'{typeof(TResponse)}',_apiGatewayUrl:'{_apiGatewayUrl}',parameters:'{request}',methodName:'{methodName}',headers:'{headers}'");
            using var httpClient = _clientFactory.CreateClient(string.Empty);

            await ManageHttpHeadersAsync(httpClient);

            var grpcChannelOptions = new GrpcChannelOptions { HttpClient = httpClient };
            if (_apiGatewayUrl.Scheme == Uri.UriSchemeHttp || Http2UnencryptedSupport)
            {
                AppContext.SetSwitch(
                    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            }

            Uri channelUri = null;
            try
            {
                channelUri = new Uri(_apiGatewayUrl + typeof(TClient).Namespace + "." + typeof(TClient).DeclaringType.Name + "/" + methodName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"The Channel URI generation is failed.", ex);
                throw;
            }

            using var channel = GrpcChannel.ForAddress(channelUri.AbsoluteUri.TrimEnd('/'), grpcChannelOptions);
            var clientConstructor =
                typeof(TClient).GetConstructor(new Type[] { typeof(ChannelBase) });
            if (clientConstructor == null)
            {
                var error =
                    $"Invalid client type {typeof(TClient)}. No constructor found with argument type {typeof(ChannelBase)}.";
                _logger.LogError(error);
                throw new ArgumentException(error, "TClient");
            }

            var clientChannel = clientConstructor.Invoke(new[] { channel });
            var serviceMethod =
                typeof(TClient).GetMethod(methodName, new Type[] { typeof(TInput), typeof(CallOptions) });
            if (serviceMethod == null)
            {
                var error =
                    $"Invalid methodName {methodName}. No method found in class {typeof(TClient)} with request arguments of type {typeof(TInput)}.";
                _logger.LogError(error);
                throw new ArgumentException(error, "methodName");
            }

            var callOptions = new CallOptions(headers);
            TResponse response = default(TResponse);
            try
            {
                response = Invoke<TResponse>(serviceMethod, clientChannel, request, callOptions);
            }
            catch (Exception e)
            {
                if (e.InnerException is RpcException)
                {
                    ExceptionDispatchInfo.Capture(e.InnerException).Throw();
                }
                else
                {
                    throw;
                }
            }

            _logger.LogInformation($"Received response '{response}'");
            return response;
        }

        private async Task ManageHttpHeadersAsync(HttpClient httpClient)
        {
            if (!httpClient.DefaultRequestHeaders.Contains(CorrelationIdOptions.DefaultHeader))
            {
                _logger.LogWarning(
                    $"CorrelationId with name '{CorrelationIdOptions.DefaultHeader}' is not found for this grpc request");
            }

            var token = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            _logger.LogTrace($"Token: {token}");

            _logger.LogTrace($"ManageHttpHeadersAsync - Headers: {httpClient.DefaultRequestHeaders}");
        }
    }
}
