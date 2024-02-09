using CarAuto.Protos.Vehicle;

namespace CarAuto.VehicleService.Business.Interfaces
{
    public interface IBrandLogic
    {
        Task<CreateBrandResponse> CreateBrandAsync(CreateBrandRequest request);
        Task DeleteBrandAsync(DeleteBrandRequest request);
        Task<GetAllBrandsResponse> GetAllBrandsAsync();
        Task<GetBrandResponse> GetBrandAsync(GetBrandRequest request);
        Task UpdateBrandAsync(UpdateBrandRequest request);
    }
}