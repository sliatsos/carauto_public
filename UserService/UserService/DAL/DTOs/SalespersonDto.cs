namespace CarAuto.UserService.DAL.DTOs
{
    public class SalespersonDto
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public string DisplayName { get; set; }

        public string Email { get; set; }

        public byte[] Image { get; set; }

        public string Phone { get; set; }
    }
}
