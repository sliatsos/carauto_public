using CarAuto.EFCore.BaseEntity;
using CarAuto.Protos.Enums;

namespace CarAuto.VehicleService.DAL.DTOs;

public class OptionServiceDto
{
    public Guid Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string InternalCode { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public OptionType Type { get; set; }

    public double UnitPrice { get; set; }

    public Guid? ModelId { get; set; }

    public double UnitCost { get; set; }
}
