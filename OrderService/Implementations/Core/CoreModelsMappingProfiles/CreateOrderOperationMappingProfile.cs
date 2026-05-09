using AutoMapper;
using Core.Abstractions.Enums;
using Core.Abstractions.OperationModels;
using Core.Resolvers;
using Dal.Abstractions.Entities;
using Events.Abstractions.Models;
using UserServiceClient.Abstractions.Models;
using VideoArchiveClient.Abstractions.Models;

namespace Core.CoreModelsMappingProfiles;

internal sealed class CreateOrderOperationMappingProfile : Profile
{
    public CreateOrderOperationMappingProfile()
    {
        CreateMap<CreateOrderOperationModel, Order>()
            .ForMember(d => d.Id,
                opt => opt.MapFrom((_, _, _, ctx) =>
                    (Guid)ctx.Items["Id"]))
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
                    (DateTimeOffset)ctx.Items["UpdatedAtUtc"]))
            .ForMember(d => d.ArchiveFile, opt => opt.Ignore());

        CreateMap<Order, OrderDetailsOperationModel>()
            .ForMember(d => d.ArchiveFile, o => o.MapFrom<ArchiveFileOperationResolver>());
        //
        // CreateMap<Order, OrderCreatedEvent>()
        //     .ForMember(d => d.OrderId, opt => opt.MapFrom(s => s.Id))
        //     .ForMember(d => d.UserId, opt => opt.MapFrom(s => s.UserId))
        //     .ForMember(d => d.CameraId, opt => opt.MapFrom(s => s.CameraId))
        //     .ForMember(d => d.FromUtc, opt => opt.MapFrom(s => s.FromUtc))
        //     .ForMember(d => d.ToUtc, opt => opt.MapFrom(s => s.ToUtc))
        //     .ForMember(d => d.CreatedAtUtc, opt => opt.MapFrom(s => s.CreatedAtUtc))


        CreateMap<CreateOrderOperationModel, ValidateAccessClientModel>();

        CreateMap<CreateOrderOperationModel, ValidateArchiveAvailabilityClientModel>();
    }
}