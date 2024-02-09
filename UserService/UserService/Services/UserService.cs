using CarAuto.CommonRegistration;
using CarAuto.Protos.User;
using CarAuto.UserService.Business;
using CarAuto.UserService.Business.Interfaces;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using static CarAuto.Protos.User.UserService;

namespace CarAuto.UserService.Services
{
    //[Authorize(AuthorizationPolicies.IsUser)]
    public class UserService : UserServiceBase
    {
        private readonly IUserLogic _userLogic;

        public UserService(IUserLogic userLogic)
        {
            _userLogic = userLogic ?? throw new ArgumentNullException(nameof(userLogic));
        }

        #region User

        [Authorize(AuthorizationPolicies.IsUser)]
        public override async Task<GetUserResponse> GetCurrentUser(Empty request, ServerCallContext context)
        {
            return new GetUserResponse
            {
                User = await _userLogic.GetUserAsync(),
            };
        }

        [Authorize(AuthorizationPolicies.IsUser)]
        public override async Task<Empty> UpdateCurrentUser(UpdateUserRequest request, ServerCallContext context)
        {
            await _userLogic.UpdateUserAsync(request.User);
            return new Empty();
        }

        #endregion

        #region Administrator

        [Authorize(AuthorizationPolicies.IsAdmin)]
        public override async Task<Empty> UpdateUser(UpdateUserRequest request, ServerCallContext context)
        {
            await _userLogic.UpdateUserAsync(Guid.Parse(request.User.Id), request.User);
            return new Empty();
        }

        [Authorize(AuthorizationPolicies.IsAdmin)]
        public override async Task<GetUserResponse> GetUser(GetUserRequest request, ServerCallContext context)
        {
            return new GetUserResponse
            {
                User = await _userLogic.GetUserAsync(Guid.Parse(request.Id)),
            };
        }

        [Authorize(AuthorizationPolicies.IsAdmin)]
        public override async Task<Empty> DeleteUser(DeleteUserRequest request, ServerCallContext context)
        {
            await _userLogic.DeleteUserAsync(Guid.Parse(request.Id));
            return new Empty();
        }

        [Authorize(AuthorizationPolicies.IsAdmin)]
        public override async Task<GetAllUsersResponse> GetAllUsers(Empty request, ServerCallContext context)
        {
            return await _userLogic.GetAllUsersAsync();
        }

        #endregion

        #region Anonymous

        [AllowAnonymous]
        public override async Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
        {
            var userId = await _userLogic.CreateUserAsync(request.User);
            return new CreateUserResponse
            {
                Id = userId,
            };
        }

        #endregion
    }
}
