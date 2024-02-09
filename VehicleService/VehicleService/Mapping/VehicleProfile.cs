using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using VehicleEntity = CarAuto.VehicleService.DAL.Entities.Vehicle;
using VehicleProto = CarAuto.Protos.Vehicle.Vehicle;
using BrandEntity = CarAuto.VehicleService.DAL.Entities.Brand;
using BrandProto = CarAuto.Protos.Vehicle.Brand;
using ModelEntity = CarAuto.VehicleService.DAL.Entities.Model;
using ModelProto = CarAuto.Protos.Vehicle.Model;
using OptionEntity = CarAuto.VehicleService.DAL.Entities.Option;
using OptionProto = CarAuto.Protos.Vehicle.Option;
using CarAuto.VehicleService.DAL.DTOs;

namespace CarAuto.VehicleService.Mapping;

public class VehicleProfile : Profile
{
    public VehicleProfile()
    {
        CreateMap<VehicleEntity, VehicleProto>()
            .ForPath(e => e.Mileage.Uom, e => e.MapFrom(e => e.MileageUnitOfMeasure))
            .ForPath(e => e.Mileage.Value, e => e.MapFrom(e => e.MileageValue))
            .ReverseMap();

        CreateMap<BrandEntity, BrandProto>()
            .ReverseMap();
        CreateMap<ModelEntity, ModelProto>()
            .ReverseMap();
        CreateMap<OptionEntity, OptionProto>()
            .ReverseMap();

        CreateMap<VehicleEntity, VehicleDto>()
            .ReverseMap();
        CreateMap<BrandEntity, BrandDto>()
            .ReverseMap();
        CreateMap<ModelEntity, ModelDto>()
            .ReverseMap();
        CreateMap<OptionEntity, OptionDto>()
            .ReverseMap();

        CreateMap<Guid, string>().
            ConvertUsing(e => e.ToString());

        CreateMap<string, Guid>()
            .ConvertUsing(e => string.IsNullOrEmpty(e) ? Guid.NewGuid() : Guid.Parse(e));

        CreateMap<string, Guid?>()
            .ConvertUsing(e => string.IsNullOrEmpty(e) ? null : Guid.Parse(e));

        CreateMap<Guid?, string>().
            ConvertUsing(e => e == null ? string.Empty : e.ToString());

        CreateMap<DateTime, Timestamp>()
            .ConvertUsing(e => Timestamp.FromDateTime(e.ToUniversalTime()));

        CreateMap<Timestamp, DateTime>()
            .ConvertUsing(e => e != null ? e.ToDateTime().ToUniversalTime() : DateTime.MinValue);
    }
}
