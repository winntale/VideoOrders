using AutoMapper;
using Core.Abstractions.Enums;
using Core.Abstractions.OperationModels;
using Dal.Abstractions.Entities;

namespace Core.CoreModelsMappingProfiles;

internal sealed class CreateOrderOperationMappingProfile : Profile
{
    public CreateOrderOperationMappingProfile()
    {
        CreateMap<CreateOrderOperationModel, Order>()
            .ForMember(d => d.Status,
                opt => opt.MapFrom((_, _, _, ctx) =>
                    (OrderStatus)ctx.Items["Status"]))
            .ForMember(d => d.FailureReason,
                opt => opt.MapFrom((_, _, _, ctx) =>
                    (string?)ctx.Items["FailureReason"]))
            .ForMember(d => d.CreatedAtUtc,
                opt => opt.MapFrom((_, _, _, ctx) =>
                    (DateTimeOffset)ctx.Items["CreatedAtUtc"]))
            .ForMember(d => d.UpdatedAtUtc,
                opt => opt.MapFrom((_, _, _, ctx) =>
                    (DateTimeOffset)ctx.Items["UpdatedAtUtc"]));
        CreateMap<Order, OrderDetailsOperationModel>();
    }
}