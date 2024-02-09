using CarAuto.EFCore.BaseEntity;

namespace CarAuto.UserService.DAL.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public PhoneType PhoneType { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }
    }
}
