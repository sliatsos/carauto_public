using CarAuto.UserService.DAL.Entities;

namespace CarAuto.UserService.DAL.DTOs;

public class CustomerDto
{
    public Guid Id { get; set; }

    public string Code { get; set; }

    public string DisplayName { get; set; }

    public DateTime ModifiedOn { get; set; }

    public string Image { get; set; }

    public ContactType Type { get; set; }

    public AddressDto Address { get; set; }

    public CommunicationDto Communication { get; set; }
}
