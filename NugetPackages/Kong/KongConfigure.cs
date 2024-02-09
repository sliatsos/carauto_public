namespace CarAuto.Kong;

public class KongConfigure
{
    public bool Disabled { get; set; }

    public bool OnStartup { get; set; }

    public bool OnShutDown { get; set; }

    public Uri KongApiUrl { get; set; }

    public string KongApiKeyHeaderName { get; set; }

    public string KongApiKeyHeaderValue { get; set; }

    public string ServiceHost { get; set; }

    public int ServicePort { get; set; }

    public KongServiceProtocolType ServiceProtocol { get; set; }

    public string GrpcReflectionApiPath { get; set; }
}
