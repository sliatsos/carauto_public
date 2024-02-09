using CarAuto.Protos.Vehicle;
using CarAuto.VehicleService.DAL.DTOs;

namespace CarAuto.VehicleService.Business.Interfaces;

public interface IVehicleLogic
{
    Task<GetAllVehiclesResponse> GetAllVehiclesAsync();

    Task<CreateVehicleResponse> CreateVehicleAsync(CreateVehicleRequest request);

    Task<GetVehicleResponse> GetVehicleAsync(GetVehicleRequest request);

    Task UpdateVehicleAsync(UpdateVehicleRequest request);

    Task DeleteVehicleAsync(DeleteVehicleRequest request);

    Task<SearchVehiclesResponse> SearchVehiclesAsync(SearchVehiclesRequest request);

    Task CreateVehicleAsync(VehicleDto vehicle);
}
