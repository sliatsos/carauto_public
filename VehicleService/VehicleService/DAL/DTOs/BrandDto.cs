namespace CarAuto.VehicleService.DAL.DTOs
{
    public class BrandDto
    {
        public Guid Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;
    }
}
