using CarAuto.Protos.Customer;
using CarAuto.Protos.Salesperson;

namespace CarAuto.UserService.Business.Interfaces
{
    public interface ISalespersonLogic
    {
        Task<GetAllSalespersonResponse> GetAllSalespersonsAsync();

        Task<GetSalespersonResponse> GetSalespersonAsync(GetSalespersonRequest request);

        Task<CreateSalespersonResponse> CreateSalespersonAsync(CreateSalespersonRequest salespersonRequest);

        Task UpdateSalespersonAsync(UpdateSalespersonRequest request);

        Task DeleteSalespersonAsync(DeleteSalespersonRequest request);
    }
}
