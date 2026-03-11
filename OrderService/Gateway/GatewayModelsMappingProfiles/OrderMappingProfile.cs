using AutoMapper;
using Core.Abstractions.OperationModels;
using Gateway.Models;

namespace Gateway.GatewayModelsMappingProfiles;

internal sealed class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<CreateOrderDto, CreateOrderOperationModel>();
        
        CreateMap<OrderDetailsOperationModel, OrderResponseDto>()
            .ForMember(dest => dest.Status,
                opt =>
                    opt.MapFrom(src => src.Status.ToString()));
    }
    
}