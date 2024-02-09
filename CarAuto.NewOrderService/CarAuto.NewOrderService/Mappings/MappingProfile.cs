﻿using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using OrderProto = CarAuto.Protos.Order.Order;
using OrderEntity = CarAuto.OrderService.DAL.Entities.Order;
using OptionProto = CarAuto.Protos.Vehicle.Option;
using OptionEntity = CarAuto.OrderService.DAL.Entities.Option;
using CarAuto.NewOrderService.DAL.Entities;
using CarAuto.NewOrderService.DAL.DTOs;

namespace CarAuto.OrderService.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<OrderProto, OrderEntity>()
            .ReverseMap();

        CreateMap<OptionProto, OptionEntity>()
            .ReverseMap();

        CreateMap<Guid, string>().
            ConvertUsing(e => e.ToString());

        CreateMap<string, Guid>()
            .ConvertUsing(e => string.IsNullOrEmpty(e) ? Guid.Empty : Guid.Parse(e));

        CreateMap<string, Guid?>()
            .ConvertUsing(e => string.IsNullOrEmpty(e) ? null : Guid.Parse(e));

        CreateMap<Guid?, string>().
            ConvertUsing(e => e == null ? string.Empty : e.ToString());

        CreateMap<DateTime, Timestamp>()
            .ConvertUsing(e => Timestamp.FromDateTime(e.ToUniversalTime()));

        CreateMap<Timestamp, DateTime>()
            .ConvertUsing(e => e != null ? e.ToDateTime().ToUniversalTime() : DateTime.MinValue);

        CreateMap<QuoteToOrderDto, QuoteOrderLink>()
            .ReverseMap();
    }
}
