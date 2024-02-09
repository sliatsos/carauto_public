using CarAuto.Protos.Salesperson;
using CarAuto.UserService.Business;
using CarAuto.UserService.Business.Interfaces;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace CarAuto.UserService.Services
{
    public class SalespersonService : Protos.Salesperson.SalespersonService.SalespersonServiceBase
    {
        private readonly ISalespersonLogic _salespersonLogic;

        public SalespersonService(ISalespersonLogic salespersonLogic)
        {
            _salespersonLogic = salespersonLogic ?? throw new ArgumentNullException(nameof(salespersonLogic));
        }

        public override async Task<GetAllSalespersonResponse> GetAllSalespersons(Empty request, ServerCallContext context)
        {
            return await _salespersonLogic.GetAllSalespersonsAsync();
        }

        public override async Task<GetSalespersonResponse> GetSalesperson(GetSalespersonRequest request, ServerCallContext context)
        {
            return await _salespersonLogic.GetSalespersonAsync(request);
        }

        public override async Task<CreateSalespersonResponse> CreateSalesperson(CreateSalespersonRequest request, ServerCallContext context)
        {
            return await _salespersonLogic.CreateSalespersonAsync(request);
        }

        public override async Task<Empty> DeleteSalesperson(DeleteSalespersonRequest request, ServerCallContext context)
        {
            await _salespersonLogic.DeleteSalespersonAsync(request);
            return new Empty();
        }

        public override async Task<Empty> UpdateSalesperson(UpdateSalespersonRequest request, ServerCallContext context)
        {
            await _salespersonLogic.UpdateSalespersonAsync(request);
            return new Empty();
        }
    }
}
