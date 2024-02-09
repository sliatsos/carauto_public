using CarAuto.Protos.Vehicle;

namespace CarAuto.VehicleService.Business.Interfaces
{
    public interface IOptionLogic
    {
        Task<CreateOptionResponse> CreateOptionAsync(CreateOptionRequest request);
        Task DeleteOptionAsync(DeleteOptionRequest request);
        Task<GetAllOptionsResponse> GetAllOptionsAsync();
        Task<GetOptionResponse> GetOptionAsync(GetOptionRequest request);
        Task UpdateOptionAsync(UpdateOptionRequest request);
    }
}