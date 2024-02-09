using CarAuto.Protos.Customer;

namespace CarAuto.UserService.Business.Interfaces
{
    public interface ICustomerLogic
    {
        Task<GetAllCustomersResponse> GetAllCustomersAsync();

        Task<CreateCustomerResponse> CreateCustomerAsync(CreateCustomerRequest customerRequest);

        Task<GetCustomerResponse> GetCustomerAsync(GetCustomerRequest request);

        Task UpdateCustomerAsync(UpdateCustomerRequest request);

        Task DeleteCustomerAsync(DeleteCustomerRequest request);

        Task<GetCustomerResponse> GetCurrentCustomer();
    }
}
