using CarAuto.Protos.Enums;

namespace CarAuto.NewOrderService.DAL.DTOs;

public class VehicleDto
{
    public Guid Id { get; set; }
    public string Vin { get; set; }
    public DateTime RegistrationDate { get; set; }
    public string LicenseNo { get; set; }
    public Guid ModelId { get; set; }
    public Guid BrandId { get; set; }
    public UnitOfMeasure MileageUnitOfMeasure { get; set; }
    public BusinessPartnerType BusinessPartnerType { get; set; }
    public VehicleStatus VehicleStatus { get; set; }
    public VehicleType VehicleType { get; set; }
    public string ExternalId { get; set; }
    public string Code { get; set; }
    public Guid BusinessPartnerId { get; set; }
}
