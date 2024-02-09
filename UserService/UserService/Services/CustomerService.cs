using CarAuto.CommonRegistration;
using CarAuto.Protos.Customer;
using CarAuto.UserService.Business;
using CarAuto.UserService.Business.Interfaces;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using static CarAuto.Protos.Customer.CustomerService;

namespace CarAuto.UserService.Services
{
    public class CustomerService : CustomerServiceBase
    {
        private readonly ICustomerLogic _customerLogic;

        public CustomerService(ICustomerLogic customerLogic)
        {
            _customerLogic = customerLogic ?? throw new ArgumentNullException(nameof(customerLogic));
        }

        [Authorize(AuthorizationPolicies.IsAdmin)]
        public override async Task<GetAllCustomersResponse> GetAllCustomer(Empty request, ServerCallContext context)
        {
            return await _customerLogic.GetAllCustomersAsync();
        }

        [Authorize(AuthorizationPolicies.IsAdmin)]
        public override async Task<CreateCustomerResponse> CreateCustomer(CreateCustomerRequest request, ServerCallContext context)
        {
            return await _customerLogic.CreateCustomerAsync(request);
        }

        [Authorize(AuthorizationPolicies.IsAdmin)]
        public override async Task<GetCustomerResponse> GetCustomer(GetCustomerRequest request, ServerCallContext context)
        {
            return await _customerLogic.GetCustomerAsync(request);
        }

        [Authorize(AuthorizationPolicies.IsUser)]
        public override async Task<Empty> UpdateCustomer(UpdateCustomerRequest request, ServerCallContext context)
        {
            await _customerLogic.UpdateCustomerAsync(request);
            return new Empty();
        }

        [Authorize(AuthorizationPolicies.IsUser)]
        public override async Task<Empty> DeleteCustomer(DeleteCustomerRequest request, ServerCallContext context)
        {
            await _customerLogic.DeleteCustomerAsync(request);
            return new Empty();
        }

        [Authorize(AuthorizationPolicies.IsUser)]
        public override async Task<GetCustomerResponse> GetCurrentCustomer(Empty request, ServerCallContext context)
        {
            return await _customerLogic.GetCurrentCustomer();
        }
    }
}
