using AutoMapper;
using Keycloak.Net.Models.Users;

namespace CarAuto.UserService.Mappings;

public class UserProfile : Profile
{
    public UserProfile() 
    {
        CreateMap<User, UserProto>()
            .ReverseMap();

        CreateMap<User, UserKeycloak>()
            .ForMember(e => e.Enabled, e => e.MapFrom(e => true))
            .ForMember(e => e.Access, e => e.MapFrom(e => new UserAccess
            {
                Impersonate = false,
                Manage = false,
                ManageGroupMembership = false,
                MapRoles = false,
                View = true,
            }));

        CreateMap<PhoneType, PhoneTypeProto>()
            .ReverseMap();

        CreateMap<Guid, string>().
            ConvertUsing(e => e.ToString());

        CreateMap<string, Guid>()
            .ConvertUsing(e => string.IsNullOrEmpty(e) ? Guid.NewGuid() : Guid.Parse(e));
    }
}
