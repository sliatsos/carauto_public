namespace CarAuto.VehicleService.DAL.DTOs;
public class DataSynchronizationDto
{
    public Action Action { get; set; }

    public string Entity { get; set; }

    public PayloadDto Payload { get; set; }
}
