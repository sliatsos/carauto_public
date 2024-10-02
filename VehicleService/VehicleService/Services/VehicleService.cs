using CarAuto.CommonRegistration;
using CarAuto.Protos.Vehicle;
using CarAuto.VehicleService.Business.Interfaces;
using CarAuto.VehicleService.SignalR;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using static CarAuto.Protos.Vehicle.VehicleService;

namespace CarAuto.VehicleService.Services;

[Authorize(Roles = "admin,user")]
public class VehicleService : VehicleServiceBase
{
    private readonly IVehicleLogic _vehicleLogic;
    private readonly IHubContext<VehicleHub> _vehicleHub;

    public VehicleService(IVehicleLogic vehicleLogic, IHubContext<VehicleHub> vehicleHub)
    {
        _vehicleLogic = vehicleLogic ?? throw new ArgumentNullException(nameof(vehicleLogic));
        _vehicleHub = vehicleHub ?? throw new ArgumentNullException(nameof(vehicleHub));
    }

    [Authorize(AuthorizationPolicies.IsAdmin)]
    public override async Task<Empty> DeleteVehicle(DeleteVehicleRequest request, ServerCallContext context)
    {
        await _vehicleLogic.DeleteVehicleAsync(request);
        return new Empty();
    }

    public override async Task<GetAllVehiclesResponse> GetAllVehicles(Empty request, ServerCallContext context)
    {
        return await _vehicleLogic.GetAllVehiclesAsync();
    }

    public override async Task<GetVehicleResponse> GetVehicle(GetVehicleRequest request, ServerCallContext context)
    {
        return await _vehicleLogic.GetVehicleAsync(request);
    }

    public override async Task<SearchVehiclesResponse> SearchVehicles(SearchVehiclesRequest request, ServerCallContext context)
    {
        return await _vehicleLogic.SearchVehiclesAsync(request);
    }

    [Authorize(AuthorizationPolicies.IsAdmin)]
    public override async Task<Empty> UpdateVehicle(UpdateVehicleRequest request, ServerCallContext context)
    {
        await _vehicleLogic.UpdateVehicleAsync(request);
        return new Empty();
    }

    [Authorize(AuthorizationPolicies.IsAdmin)]
    public override async Task<CreateVehicleResponse> CreateVehicle(CreateVehicleRequest request, ServerCallContext context)
    {
        await _vehicleHub.Clients.All.SendAsync("CreateVehicle", request.Vehicle);
        return await _vehicleLogic.CreateVehicleAsync(request);
    }
}
