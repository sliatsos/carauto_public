using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CarAuto.EFCore.BaseEntity;

namespace CarAuto.UserService.DAL.Entities;

public class Customer : BaseEntity
{
    public string Code { get; set; }

    public string DisplayName { get; set; }

    public DateTime ModifiedOn { get; set; }

    public string Email { get; set; }

    public string Country { get; set; }

    public string City { get; set; }

    public string Address { get; set; }

    public string PostCode { get; set; }

    public string County { get; set; }

    public string Phone { get; set; }

    public string Mobile { get; set; }

    public ContactType Type { get; set; } 

    public byte[] Image { get; set; }

    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
}
