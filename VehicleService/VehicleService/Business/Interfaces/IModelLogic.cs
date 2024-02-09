using CarAuto.Protos.Vehicle;

namespace CarAuto.VehicleService.Business.Interfaces
{
    public interface IModelLogic
    {
        Task<CreateModelResponse> CreateModelAsync(CreateModelRequest request);
        Task DeleteModelAsync(DeleteModelRequest request);
        Task<GetAllModelsResponse> GetAllModelsAsync();
        Task<GetModelResponse> GetModelAsync(GetModelRequest request);
        Task UpdateModelAsync(UpdateModelRequest request);
    }
}