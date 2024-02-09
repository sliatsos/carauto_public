using AutoMapper;
using CarAuto.ClaimParser;
using CarAuto.CommonRegistration;
using CarAuto.EFCore.BaseEntity.UnitOfWork;
using CarAuto.Protos.User;
using CarAuto.UserService.Business.Interfaces;
using CarAuto.UserService.Utils;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Keycloak.Net;
using Keycloak.Net.Models.Users;

namespace CarAuto.UserService.Business;

public class UserLogic : IUserLogic
{
    private readonly KeycloakClient _keycloakClient;
    private readonly KeycloakConfig _keycloakConfig;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClaimParser _claimParser;

    public UserLogic(
        KeycloakClient keycloakClient,
        KeycloakConfig keycloakConfig,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IClaimParser claimParser)
    {
        _keycloakClient = keycloakClient ?? throw new ArgumentNullException(nameof(keycloakClient));
        _keycloakConfig = keycloakConfig ?? throw new ArgumentNullException(nameof(keycloakConfig));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _claimParser = claimParser ?? throw new ArgumentNullException(nameof(claimParser));
    }

    public async Task<string> CreateUserAsync(UserProto user)
    {
        var entity = _mapper.Map<User>(user);
        var userRepo = await _unitOfWork.GetRepositoryAsync<User>();

        var keycloakUser = _mapper.Map<UserKeycloak>(entity);
        keycloakUser.Credentials = new List<Credentials>
        {
            new Credentials
            {
                Type = "password",
                Value = user.Password,
            }
        };
        keycloakUser.Groups = new List<string>
        {
           AuthorizationGroups.Users,
        };
        var userId = await _keycloakClient.CreateAndRetrieveUserIdAsync(_keycloakConfig.KeyCloakRealm, keycloakUser);
        entity.Id = Guid.Parse(userId);

        await userRepo.InsertAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return userId;
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var userRepo = await _unitOfWork.GetRepositoryAsync<User>();
        await _keycloakClient.DeleteUserAsync(_keycloakConfig.KeyCloakRealm, id.ToString());
        await userRepo.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<GetAllUsersResponse> GetAllUsersAsync()
    {
        var userRepo = await _unitOfWork.GetRepositoryAsync<User>();
        var response = new GetAllUsersResponse();
        response.Users.AddRange(userRepo.Queryable.Select(e => _mapper.Map<UserProto>(e)));

        return response;
    }

    public async Task<UserProto> GetUserAsync(Guid id)
    {
        var userRepo = await _unitOfWork.GetRepositoryAsync<User>();
        var user = await userRepo.GetByIdAsync(id);
        return _mapper.Map<UserProto>(user);
    }

    public async Task<UserProto> GetUserAsync()
    {
        var userId = _claimParser.GetUserId();
        return await GetUserAsync(userId);
    }

    public async Task UpdateUserAsync(UserProto user)
    {
        var userId = _claimParser.GetUserId();
        await UpdateUserAsync(userId, user);
    }

    public async Task UpdateUserAsync(Guid id, UserProto user)
    {
        var userRepo = await _unitOfWork.GetRepositoryAsync<User>();
        var userEntity = await userRepo.GetByIdAsync(id);
        user.Id = id.ToString();
        _mapper.Map(user, userEntity);
        await _unitOfWork.SaveChangesAsync();
    }
}
