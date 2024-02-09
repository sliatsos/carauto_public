using CarAuto.Protos.User;

namespace CarAuto.UserService.Business.Interfaces
{
    public interface IUserLogic
    {
        public Task<string> CreateUserAsync(UserProto user);

        Task<UserProto> GetUserAsync(Guid id);

        Task<UserProto> GetUserAsync();

        Task UpdateUserAsync(Guid id, UserProto user);

        Task UpdateUserAsync(UserProto user);

        Task DeleteUserAsync(Guid id);

        Task<GetAllUsersResponse> GetAllUsersAsync();
    }
}
