using CarAuto.CommonRegistration;
using CarAuto.Protos.Vehicle;
using CarAuto.VehicleService.Business.Interfaces;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using static CarAuto.Protos.Vehicle.BrandService;

namespace CarAuto.VehicleService.Services;

[Authorize(Roles = "admin,user")]
public class BrandService : BrandServiceBase
{
    private readonly IBrandLogic _brandLogic;

    public BrandService(IBrandLogic brandLogic) 
    {
        _brandLogic = brandLogic ?? throw new ArgumentNullException(nameof(brandLogic));
    }

    [Authorize(AuthorizationPolicies.IsAdmin)]
    public override async Task<CreateBrandResponse> CreateBrand(CreateBrandRequest request, ServerCallContext context)
    {
        return await _brandLogic.CreateBrandAsync(request);
    }

    [Authorize(AuthorizationPolicies.IsAdmin)]
    public override async Task<Empty> DeleteBrand(DeleteBrandRequest request, ServerCallContext context)
    {
        await _brandLogic.DeleteBrandAsync(request);
        return new Empty();
    }

    public override async Task<GetAllBrandsResponse> GetAllBrands(Empty request, ServerCallContext context)
    {
        return await _brandLogic.GetAllBrandsAsync();
    }

    public override async Task<GetBrandResponse> GetBrand(GetBrandRequest request, ServerCallContext context)
    {
        return await _brandLogic.GetBrandAsync(request);
    }

    [Authorize(AuthorizationPolicies.IsAdmin)]
    public override async Task<Empty> UpdateBrand(UpdateBrandRequest request, ServerCallContext context)
    {
        await _brandLogic.UpdateBrandAsync(request);
        return new Empty();
    }
}
