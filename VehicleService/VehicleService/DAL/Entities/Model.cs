using CarAuto.EFCore.BaseEntity;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarAuto.VehicleService.DAL.Entities;

public class Model : BaseEntity
{
    public string Code { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int ModelYear { get; set; }

    public Brand? Brand { get; set; }

    [ForeignKey(nameof(Brand))]
    public Guid? BrandId { get; set; }

    public List<Option> Options { get; set; } = new List<Option>();
}
