using CarAuto.CommonRegistration;
using CarAuto.Protos.Vehicle;
using CarAuto.VehicleService.Business.Interfaces;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using static CarAuto.Protos.Vehicle.OptionService;

namespace CarAuto.VehicleService.Services;

[Authorize(Roles = "admin,user")]
public class OptionService : OptionServiceBase
{
    private readonly IOptionLogic _optionLogic;

    public OptionService(IOptionLogic OptionLogic) 
    {
        _optionLogic = OptionLogic ?? throw new ArgumentNullException(nameof(OptionLogic));
    }

    [Authorize(AuthorizationPolicies.IsAdmin)]
    public override async Task<CreateOptionResponse> CreateOption(CreateOptionRequest request, ServerCallContext context)
    {
        return await _optionLogic.CreateOptionAsync(request);
    }

    [Authorize(AuthorizationPolicies.IsAdmin)]
    public override async Task<Empty> DeleteOption(DeleteOptionRequest request, ServerCallContext context)
    {
        await _optionLogic.DeleteOptionAsync(request);
        return new Empty();
    }

    public override async Task<GetAllOptionsResponse> GetAllOptions(Empty request, ServerCallContext context)
    {
        return await _optionLogic.GetAllOptionsAsync();
    }

    public override async Task<GetOptionResponse> GetOption(GetOptionRequest request, ServerCallContext context)
    {
        return await _optionLogic.GetOptionAsync(request);
    }

    [Authorize(AuthorizationPolicies.IsAdmin)]
    public override async Task<Empty> UpdateOption(UpdateOptionRequest request, ServerCallContext context)
    {
        await _optionLogic.UpdateOptionAsync(request);
        return new Empty();
    }
}
