using AutoMapper;
using BasicBilling.API.Application.DTOs;
using BasicBilling.API.Domain.Entities;
using BasicBilling.API.Domain.Enums;

namespace BasicBilling.API.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Bill, BillDto>()
            .ForMember(dest => dest.ServiceType, opt => opt.MapFrom(src => src.ServiceType.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<CreateBillDto, Bill>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => BillStatus.Pending));

        CreateMap<Payment, PaymentDto>()
            .ForMember(dest => dest.ServiceType, opt => opt.MapFrom(src => src.Bill.ServiceType.ToString()))
            .ForMember(dest => dest.Period, opt => opt.MapFrom(src => src.Bill.Period))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Bill.Status.ToString()));
    }
}
