namespace CarAuto.Logger;

public class KafkaConfiguration
{
    public int BatchSizeLimit { get; set; }

    public int Period { get; set; }

    public string BootstrapServers { get; set; }

    public string Topic { get; set; }
}
