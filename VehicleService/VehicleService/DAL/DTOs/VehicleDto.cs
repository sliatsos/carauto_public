using CarAuto.EFCore.BaseEntity;
using CarAuto.Protos.Enums;
using System.ComponentModel.DataAnnotations;

namespace CarAuto.VehicleService.DAL.DTOs;

public class VehicleDto
{
    public Guid Id { get; set; }

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

    public Guid BrandId { get; set; }

    public Guid ModelId { get; set; }

    public BrandDto Brand { get; set; }

    public ModelDto Model { get; set; }

    public List<OptionDto> Options { get; set; } = new List<OptionDto>();
}
