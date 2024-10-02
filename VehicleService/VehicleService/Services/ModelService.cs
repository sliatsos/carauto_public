using CarAuto.CommonRegistration;
using CarAuto.Protos.Vehicle;
using CarAuto.VehicleService.Business.Interfaces;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using static CarAuto.Protos.Vehicle.ModelService;

namespace CarAuto.VehicleService.Services;

[Authorize(Roles = "admin,user")]
public class ModelService : ModelServiceBase
{
    private readonly IModelLogic _modelLogic;

    public ModelService(IModelLogic modelLogic) 
    {
        _modelLogic = modelLogic ?? throw new ArgumentNullException(nameof(modelLogic));
    }

    [Authorize(AuthorizationPolicies.IsAdmin)]
    public override async Task<CreateModelResponse> CreateModel(CreateModelRequest request, ServerCallContext context)
    {
        return await _modelLogic.CreateModelAsync(request);
    }

    [Authorize(AuthorizationPolicies.IsAdmin)]
    public override async Task<Empty> DeleteModel(DeleteModelRequest request, ServerCallContext context)
    {
        await _modelLogic.DeleteModelAsync(request);
        return new Empty();
    }

    public override async Task<GetAllModelsResponse> GetAllModels(Empty request, ServerCallContext context)
    {
        return await _modelLogic.GetAllModelsAsync();
    }

    public override async Task<GetModelResponse> GetModel(GetModelRequest request, ServerCallContext context)
    {
        return await _modelLogic.GetModelAsync(request);
    }

    [Authorize(AuthorizationPolicies.IsAdmin)]
    public override async Task<Empty> UpdateModel(UpdateModelRequest request, ServerCallContext context)
    {
        await _modelLogic.UpdateModelAsync(request);
        return new Empty();
    }
}
