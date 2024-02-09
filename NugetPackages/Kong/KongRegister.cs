using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Reflection;
using KongRegister;
using KongRegister.Business.Interfaces;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Hosting;
using ServiceDescriptor = Google.Protobuf.Reflection.ServiceDescriptor;

namespace CarAuto.Kong;

public class KongRegister : IKongRegisterBusiness
{
    private readonly KongConfigure _kongConfig;
    private readonly ILogger<KongRegisterBackgroudService> _logger;
    private readonly IServer _server;
    private Dictionary<string, KongService> _service;
    private HttpClient _httpClient;
    private IHostEnvironment _env;

    public KongRegister(KongConfigure kongConfig, ILogger<KongRegisterBackgroudService> logger, IServer server, IHttpClientFactory httpClientFactory, IHostEnvironment env)
    {
        _kongConfig = kongConfig ?? throw new ArgumentNullException(nameof(kongConfig));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _server = server ?? throw new ArgumentNullException(nameof(server));
        _env = env ?? throw new ArgumentNullException(nameof(env));
        if (string.IsNullOrWhiteSpace(_kongConfig.GrpcReflectionApiPath))
        {
            _kongConfig.GrpcReflectionApiPath = "/grpc.reflection.v1alpha.ServerReflection/ServerReflectionInfo";
        }

        _httpClient = httpClientFactory?.CreateClient("kongadmin") ??
                      throw new ArgumentNullException(nameof(httpClientFactory));
        _httpClient.BaseAddress = _kongConfig.KongApiUrl;
        _httpClient.Timeout = new TimeSpan(0, 0, 10);
        _service = new Dictionary<string, KongService>();
        if (!string.IsNullOrWhiteSpace(_kongConfig.KongApiKeyHeaderName) &&
            !string.IsNullOrWhiteSpace(_kongConfig.KongApiKeyHeaderValue))
        {
            _httpClient.DefaultRequestHeaders.Add(_kongConfig.KongApiKeyHeaderName, _kongConfig.KongApiKeyHeaderValue);
        }
    }

    public async Task<string> RegisterAsync()
    {
        if (SkipProcess(true))
        {
            return string.Empty;
        }

        _logger.LogInformation($"Registering target in Kong using '{_kongConfig.KongApiUrl}'");
        try
        {
            var response = await SendKongRequest(HttpMethod.Get, string.Empty).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                return string.Empty;
            }

            AssignDynamicHostAndPort();

            var services = GetServiceNamesAndMethods(Assembly.GetEntryAssembly());
            _logger.LogInformation($"Detected '{services.Count}' services");
            foreach (var serviceName in services.Keys)
            {
                var registered = await RegisterService(serviceName, services[serviceName]).ConfigureAwait(false);
                if (!registered)
                {
                    return string.Empty;
                }
            }

            _logger.LogInformation($"Registration with kong gateway is sucesssfully completed");
            return JsonConvert.SerializeObject(_service);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public async Task<bool> UnregisterAsync()
    {
        _logger.LogInformation($"Unregistering microservice from Kong");
        if (SkipProcess(false))
        {
            return false;
        }

        try
        {
            var response = await SendKongRequest(HttpMethod.Get, string.Empty).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            foreach (var serviceName in _service.Keys)
            {
                var kongTargetUrl = $"/upstreams/{serviceName}/targets/{_service[serviceName].TargetId}";
                response = await SendKongRequest(HttpMethod.Delete, kongTargetUrl).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }
            }

            _logger.LogInformation($"Unregistration completed successfully.");
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            throw;
        }
    }

    public bool OnStartup()
    {
        return _kongConfig.OnStartup;
    }

    public bool Disabled()
    {
        return _kongConfig.Disabled;
    }


    protected virtual Dictionary<string, List<string>> GetServiceNamesAndMethods(Assembly assemblyInContext)
    {
        _ = assemblyInContext ?? throw new ArgumentNullException(nameof(assemblyInContext));
        var serviceList = new Dictionary<string, List<string>>();
        _logger.LogInformation($"GetServiceNamesAndMethods loaded assembly {assemblyInContext.FullName}");

        var descriptorProperties = assemblyInContext.GetTypes()
            .Select(t => t.BaseType?.CustomAttributes.FirstOrDefault(att => att.ConstructorArguments.Any(arg =>
                arg.Value is Type &&
                ((Type)arg.Value).GetProperty("Descriptor", typeof(ServiceDescriptor)) != null)))
            .Where(a => a != null)
            .Select(a => ((Type)a.ConstructorArguments.First().Value).GetProperty("Descriptor", typeof(ServiceDescriptor)));

        foreach (var descriptorProperty in descriptorProperties)
        {
            var embedDescriptor = descriptorProperty.GetValue(descriptorProperty) as ServiceDescriptor;
            serviceList.Add(
                embedDescriptor.FullName,
                embedDescriptor.Methods.Select(m => { return $"/{embedDescriptor.FullName}/{m.Name}"; }).ToList());
        }

        return serviceList;
    }

    private async Task<HttpResponseMessage> SendKongRequest(HttpMethod method, string requestUri, HttpContent body = null)
    {
        using var request = new HttpRequestMessage(method, requestUri) { Content = body };
        var response = await _httpClient.SendAsync(request).ConfigureAwait(false);
        _logger.LogInformation(
            $"Kong request uri {requestUri} status {(int)response.StatusCode} {response.ReasonPhrase}");
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                $"Kong request {requestUri} failed : status {(int)response.StatusCode} {response.ReasonPhrase}. Details {response.Content.ReadAsStringAsync().Result}");
        }

        return response;
    }

    private void AssignDynamicHostAndPort()
    {
        if (string.IsNullOrWhiteSpace(_kongConfig.ServiceHost))
        {
            using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
            socket.Connect("1.2.3.4", 65530);
            IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
            _logger.LogInformation($"Socket IPEndPoint is '{endPoint.Address}:{endPoint.Port}'");
            _kongConfig.ServiceHost = endPoint.Address.ToString();
        }

        if (_kongConfig.ServicePort == 0)
        {
            var features = _server.Features;
            var serverAddressesFeature = features.Get<IServerAddressesFeature>();
            if (serverAddressesFeature.Addresses.Count == 0)
            {
                serverAddressesFeature.Addresses.Add("http://+:5000");
            }

            var address = serverAddressesFeature.Addresses.First().Trim('/');
            _logger.LogInformation($"IServerAddressesFeature address is '{address}'");
            var port = address?.Substring(address.LastIndexOf(':') + 1);
            _kongConfig.ServicePort = !string.IsNullOrWhiteSpace(address) ? int.Parse(port, CultureInfo.InvariantCulture) : 0;
        }

        _logger.LogInformation(
            $"Service host is '{_kongConfig.ServiceHost}' and port is '{_kongConfig.KongApiUrl}'");
    }

    private bool SkipProcess(bool register)
    {
        if (_kongConfig.Disabled)
        {
            _logger.LogInformation(
                $"Set property 'Disabled' to false to register or unregister automatically");
            return true;
        }

        if (!_kongConfig.OnStartup && register)
        {
            _logger.LogInformation(
                $"Set property 'OnStartup' to true to register automatically");
            return true;
        }

        if (!_kongConfig.OnShutDown && !register)
        {
            _logger.LogInformation(
                $"Set property 'OnShutDown' to true to unregister automatically");
            return true;
        }

        return false;
    }

    private async Task<bool> RegisterService(string serviceName, List<string> routePaths)
    {
        _logger.LogInformation($"Detected '{routePaths.Count}' operations for service '{serviceName}'");

        string serviceId;
        var body = JsonContent.Create(new { Name = serviceName });
        var response = await SendKongRequest(HttpMethod.Put, $"upstreams/{serviceName}", body)
            .ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        body = JsonContent.Create(new
        {
            host = serviceName,
            port = _kongConfig.ServicePort,
            protocol = _kongConfig.ServiceProtocol.ToString().ToLowerInvariant(),
            name = serviceName,
        });
        response = await SendKongRequest(HttpMethod.Put, $"services/{serviceName}", body).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var service = await response.Content.ReadAsAsync<dynamic>().ConfigureAwait(false);
        serviceId = service.id;
        _logger.LogInformation(
            $"Kong Service '{service.id}' registered with name {serviceName}.");
        _service.Add(serviceId, new KongService());
        if (_env.IsDevelopment())
        {
            routePaths.Add(_kongConfig.GrpcReflectionApiPath);
        }

        body = JsonContent.Create(new
        {
            paths = routePaths,
            protocols = new[] { _kongConfig.ServiceProtocol.ToString().ToLowerInvariant() },
            name = $"{serviceName}.routes",
        });
        response = await SendKongRequest(HttpMethod.Put, $"services/{serviceName}/routes/{serviceName}.routes", body).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var route = await response.Content.ReadAsAsync<dynamic>().ConfigureAwait(false);
        _service[serviceId] = new KongService { RouteId = route.id };

        response = await SendKongRequest(HttpMethod.Get, $"upstreams/{serviceName}/targets").ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var targetResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        var jobject = JObject.Parse(targetResponse);
        var targetData = jobject
            .SelectTokens($"data[?(@.target=='{_kongConfig.ServiceHost}:{_kongConfig.ServicePort}')]")
            .FirstOrDefault();
        if (targetData != null)
        {
            _service[serviceId] = new KongService
            { RouteId = route.id, TargetId = targetData["id"].Value<string>() };
        }
        else
        {
            body = JsonContent.Create(new
            {
                target = $"{_kongConfig.ServiceHost}:{_kongConfig.ServicePort}",
            });
            response = await SendKongRequest(HttpMethod.Post, $"upstreams/{serviceName}/targets", body)
                .ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var target = await response.Content.ReadAsAsync<dynamic>().ConfigureAwait(false);
            _service[serviceId] = new KongService { RouteId = route.id, TargetId = target.id };
        }

        return true;
    }
}