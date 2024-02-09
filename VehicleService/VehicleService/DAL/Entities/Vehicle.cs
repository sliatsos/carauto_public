using CarAuto.EFCore.BaseEntity;
using CarAuto.Protos.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarAuto.VehicleService.DAL.Entities;

public class Vehicle : BaseEntity
{
    public string ExternalId { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public VehicleType VehicleType { get; set; }

    [MaxLength(20)]
    public string Vin { get; set; } = string.Empty;

    public string LicenseNo { get; set; } = string.Empty; 

    public DateTime RegistrationDate { get; set; }

    public DateTime ModifiedOn { get; set; }

    public VehicleStatus VehicleStatus { get; set; }

    public BusinessPartnerType BusinessPartnerType { get; set; }

    public Guid BusinessPartnerId { get; set; }

    public UnitOfMeasure MileageUnitOfMeasure { get; set; }

    public double MileageValue { get; set; }

    [ForeignKey(nameof(Brand))]
    public Guid BrandId { get; set; }

    public Brand Brand { get; set; }

    [ForeignKey(nameof(Model))]
    public Guid ModelId { get; set; }

    public Model Model { get; set; }

    public List<Option> Options { get; set; } = new List<Option>();
}
