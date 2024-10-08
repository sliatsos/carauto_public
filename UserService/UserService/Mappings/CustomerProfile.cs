﻿using System.Text;
using AutoMapper;
using CarAuto.UserService.DAL.DTOs;
using CarAuto.UserService.DAL.Entities;
using Google.Protobuf.WellKnownTypes;
using CustomerEntity = CarAuto.UserService.DAL.Entities.Customer;
using CustomerProto = CarAuto.Protos.Customer.Customer;
using SalespersonProto = CarAuto.Protos.Salesperson.Salesperson;
using SalespersonEntity = CarAuto.UserService.DAL.Entities.Salesperson;
using System.Linq;

namespace CarAuto.UserService.Mappings
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<CustomerProto, CustomerEntity>()
                .ReverseMap();

            CreateMap<SalespersonProto, SalespersonEntity>()
                .ReverseMap();

            CreateMap<CustomerDto, CustomerEntity>()
                .ForMember(e => e.Address, e => e.MapFrom(e => e.Address.Address))
                .ForMember(e => e.Country, e => e.MapFrom(e => e.Address.Country))
                .ForMember(e => e.County, e => e.MapFrom(e => e.Address.County))
                .ForMember(e => e.PostCode, e => e.MapFrom(e => e.Address.PostCode))
                .ForMember(e => e.City, e => e.MapFrom(e => e.Address.City))
                .ForMember(e => e.Email, e => e.MapFrom(e => e.Communication.Email))
                .ForMember(e => e.Mobile, e => e.MapFrom(e => e.Communication.Mobile))
                .ForMember(e => e.Phone, e => e.MapFrom(e => e.Communication.Phone))
                .ReverseMap()
                .ForPath(e => e.Address.Address, e => e.MapFrom(e => e.Address))
                .ForPath(e => e.Address.Country, e => e.MapFrom(e => e.Country))
                .ForPath(e => e.Address.County, e => e.MapFrom(e => e.County))
                .ForPath(e => e.Address.PostCode, e => e.MapFrom(e => e.PostCode))
                .ForPath(e => e.Address.City, e => e.MapFrom(e => e.City))
                .ForPath(e => e.Communication.Email, e => e.MapFrom(e => e.Email))
                .ForPath(e => e.Communication.Mobile, e => e.MapFrom(e => e.Mobile))
                .ForPath(e => e.Communication.Phone, e => e.MapFrom(e => e.Phone));

            CreateMap<Salesperson, SalespersonDto>()
                .ReverseMap();

            CreateMap<string?, string>()
                .ConvertUsing(e => string.IsNullOrEmpty(e) ? string.Empty : e);
            CreateMap<string, string>()
                .ConvertUsing(e => string.IsNullOrEmpty(e) ? string.Empty : e);

            CreateMap<ContactType, Protos.Enums.ContactType>()
                .ReverseMap();

            CreateMap<DateTime, Timestamp>()
                .ConvertUsing(e => Timestamp.FromDateTime(e));

            CreateMap<Timestamp, DateTime>()
                .ConvertUsing(e => e == null ? DateTime.UtcNow : e.ToDateTime().ToUniversalTime());


            CreateMap<byte[], string>()
                .ConvertUsing(e => Convert.ToBase64String(e));
            CreateMap<string, byte[]>()
                .ConvertUsing(e => Convert.FromBase64String(e));
        }
    }
}
