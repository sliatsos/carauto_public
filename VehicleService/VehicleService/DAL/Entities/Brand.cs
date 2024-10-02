using CarAuto.EFCore.BaseEntity;

namespace CarAuto.VehicleService.DAL.Entities
{
    public class Brand : BaseEntity
    {
        public string Code { get; set; }

        public string DisplayName { get; set; }

        public byte[] Image { get; set; }
    }
}