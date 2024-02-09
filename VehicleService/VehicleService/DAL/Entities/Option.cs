using CarAuto.EFCore.BaseEntity;
using CarAuto.Protos.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarAuto.VehicleService.DAL.Entities;

public class Option : BaseEntity
{
    public string Code { get; set; } = string.Empty;

    public string InternalCode { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public OptionType Type { get; set; } = OptionType.Option;

    public double UnitPrice { get; set; }

    public double UnitCost { get; set; }

    public Vehicle? Vehicle { get; set; }

    [ForeignKey(nameof(Vehicle))] 
    public Guid? VehicleId { get; set; }

    public Model? Model { get; set; }

    [ForeignKey(nameof(Model))]
    public Guid? ModelId { get; set; }
}
