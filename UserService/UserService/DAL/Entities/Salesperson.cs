using System.ComponentModel.DataAnnotations.Schema;
using CarAuto.EFCore.BaseEntity;

namespace CarAuto.UserService.DAL.Entities
{
    public class Salesperson : BaseEntity
    {
        public string Code { get; set; }

        public string DisplayName { get; set; }

        public string Email { get; set; }

        public byte[] Image { get; set; }

        public string Phone { get; set; }

        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}
