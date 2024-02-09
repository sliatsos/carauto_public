using CarAuto.EFCore.BaseEntity;
using CarAuto.Protos.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarAuto.OrderService.DAL.Entities;

public class Option : BaseEntity
{
    public string Code { get; set; } = string.Empty;

    public string InternalCode { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public OptionType Type { get; set; }

    public double UnitPrice { get; set; }

    public double UnitCost { get; set; }
}
