namespace CarAuto.VehicleService.DAL.DTOs
{
    public class ModelDto
    {
        public Guid Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int ModelYear { get; set; }

        public Guid BrandId { get; set; }

        public List<OptionDto> Options { get; set; } = new List<OptionDto>();
    }
}
