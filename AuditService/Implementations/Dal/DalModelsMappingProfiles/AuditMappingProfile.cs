using Dal.Abstractions.Entities;
using Dal.ElasticModels;
using AutoMapper;
using System.Text.Json;
using Dal.Abstractions.Enums;
using Events.Abstractions.Models;

namespace Dal.DalModelsMappingProfiles;

public sealed class AuditMappingProfile : Profile
{
    public AuditMappingProfile()
    {
        CreateMap<OrderCreatedEvent, AuditEntry>()
            .ForMember(d => d.Id, o => o.MapFrom(_ => Guid.NewGuid()))
            .ForMember(d => d.OrderId, o => o.MapFrom(src => src.OrderId))
            .ForMember(d => d.UserId, o => o.MapFrom(src => src.UserId))
            .ForMember(d => d.CameraId, o => o.MapFrom(src => src.CameraId))
            .ForMember(d => d.ServiceName, o => o.MapFrom(_ => AuditServiceName.OrderService))
            .ForMember(d => d.EventType, o => o.MapFrom(_ => AuditEventType.OrderCreatedEvent))
            .ForMember(d => d.EventAction, o => o.MapFrom(_ => AuditEventAction.Created))
            .ForMember(d => d.EventOutcome, o => o.MapFrom(_ => AuditEventOutcome.Success))
            .ForMember(d => d.StatusFrom, o => o.MapFrom(_ => (OrderLifecycleStatus?)null))
            .ForMember(d => d.StatusTo, o => o.MapFrom(_ => OrderLifecycleStatus.Created))
            .ForMember(d => d.FailureReason, o => o.MapFrom(_ => (string?)null))
            .ForMember(d => d.PayloadJson, o => o.MapFrom(src => JsonSerializer.Serialize(src, new JsonSerializerOptions())))
            .ForMember(d => d.OccurredAtUtc, o => o.MapFrom(src => src.CreatedAtUtc));

        CreateMap<ResourceReservedEvent, AuditEntry>()
            .ForMember(d => d.Id, o => o.MapFrom(_ => Guid.NewGuid()))
            .ForMember(d => d.OrderId, o => o.MapFrom(src => src.OrderId))
            .ForMember(d => d.UserId, o => o.Ignore())
            .ForMember(d => d.CameraId, o => o.Ignore())
            .ForMember(d => d.ServiceName, o => o.MapFrom(_ => AuditServiceName.ResourceManagementService))
            .ForMember(d => d.EventType, o => o.MapFrom(_ => AuditEventType.ResourceReservedEvent))
            .ForMember(d => d.EventAction, o => o.MapFrom(_ => AuditEventAction.Reserved))
            .ForMember(d => d.EventOutcome, o => o.MapFrom(_ => AuditEventOutcome.Success))
            .ForMember(d => d.StatusFrom, o => o.MapFrom(_ => OrderLifecycleStatus.Created))
            .ForMember(d => d.StatusTo, o => o.MapFrom(_ => OrderLifecycleStatus.ResourceReserved))
            .ForMember(d => d.FailureReason, o => o.Ignore())
            .ForMember(d => d.PayloadJson, o => o.MapFrom(src => JsonSerializer.Serialize(src, new JsonSerializerOptions())))
            .ForMember(d => d.OccurredAtUtc, o => o.MapFrom(src => src.ReservedAtUtc));

        CreateMap<ResourceReservationFailedEvent, AuditEntry>()
            .ForMember(d => d.Id, o => o.MapFrom(_ => Guid.NewGuid()))
            .ForMember(d => d.OrderId, o => o.MapFrom(src => src.OrderId))
            .ForMember(d => d.UserId, o => o.Ignore())
            .ForMember(d => d.CameraId, o => o.Ignore())
            .ForMember(d => d.ServiceName, o => o.MapFrom(_ => AuditServiceName.ResourceManagementService))
            .ForMember(d => d.EventType, o => o.MapFrom(_ => AuditEventType.ResourceReservationFailedEvent))
            .ForMember(d => d.EventAction, o => o.MapFrom(_ => AuditEventAction.ReservationFailed))
            .ForMember(d => d.EventOutcome, o => o.MapFrom(_ => AuditEventOutcome.Failure))
            .ForMember(d => d.StatusFrom, o => o.MapFrom(_ => OrderLifecycleStatus.Created))
            .ForMember(d => d.StatusTo, o => o.MapFrom(_ => OrderLifecycleStatus.ResourceReservationFailed))
            .ForMember(d => d.FailureReason, o => o.MapFrom(src => src.Reason))
            .ForMember(d => d.PayloadJson, o => o.MapFrom(src => JsonSerializer.Serialize(src, new JsonSerializerOptions())))
            .ForMember(d => d.OccurredAtUtc, o => o.MapFrom(src => src.FailedAtUtc));

        CreateMap<ProcessingStartedEvent, AuditEntry>()
            .ForMember(d => d.Id, o => o.MapFrom(_ => Guid.NewGuid()))
            .ForMember(d => d.OrderId, o => o.MapFrom(src => src.OrderId))
            .ForMember(d => d.UserId, o => o.Ignore())
            .ForMember(d => d.CameraId, o => o.Ignore())
            .ForMember(d => d.ServiceName, o => o.MapFrom(_ => AuditServiceName.ProcessingSystem))
            .ForMember(d => d.EventType, o => o.MapFrom(_ => AuditEventType.ProcessingStartedEvent))
            .ForMember(d => d.EventAction, o => o.MapFrom(_ => AuditEventAction.ProcessingStarted))
            .ForMember(d => d.EventOutcome, o => o.MapFrom(_ => AuditEventOutcome.Success))
            .ForMember(d => d.StatusFrom, o => o.MapFrom(_ => OrderLifecycleStatus.ResourceReserved))
            .ForMember(d => d.StatusTo, o => o.MapFrom(_ => OrderLifecycleStatus.ProcessingStarted))
            .ForMember(d => d.FailureReason, o => o.MapFrom(_ => (string?)null))
            .ForMember(d => d.PayloadJson, o => o.MapFrom(src => JsonSerializer.Serialize(src, new JsonSerializerOptions())))
            .ForMember(d => d.OccurredAtUtc, o => o.MapFrom(src => src.StartedAtUtc));

        CreateMap<OrderCompletedEvent, AuditEntry>()
            .ForMember(d => d.Id, o => o.MapFrom(_ => Guid.NewGuid()))
            .ForMember(d => d.OrderId, o => o.MapFrom(src => src.OrderId))
            .ForMember(d => d.UserId, o => o.Ignore())
            .ForMember(d => d.CameraId, o => o.Ignore())
            .ForMember(d => d.ServiceName, o => o.MapFrom(_ => AuditServiceName.ProcessingSystem))
            .ForMember(d => d.EventType, o => o.MapFrom(_ => AuditEventType.OrderCompletedEvent))
            .ForMember(d => d.EventAction, o => o.MapFrom(_ => AuditEventAction.Completed))
            .ForMember(d => d.EventOutcome, o => o.MapFrom(_ => AuditEventOutcome.Success))
            .ForMember(d => d.StatusFrom, o => o.MapFrom(_ => OrderLifecycleStatus.ProcessingStarted))
            .ForMember(d => d.StatusTo, o => o.MapFrom(_ => OrderLifecycleStatus.Completed))
            .ForMember(d => d.FailureReason, o => o.MapFrom(_ => (string?)null))
            .ForMember(d => d.PayloadJson, o => o.MapFrom(src => JsonSerializer.Serialize(src, new JsonSerializerOptions())))
            .ForMember(d => d.OccurredAtUtc, o => o.MapFrom(src => src.CompletedAtUtc));

        CreateMap<OrderFailedEvent, AuditEntry>()
            .ForMember(d => d.Id, o => o.MapFrom(_ => Guid.NewGuid()))
            .ForMember(d => d.OrderId, o => o.MapFrom(src => src.OrderId))
            .ForMember(d => d.UserId, o => o.Ignore())
            .ForMember(d => d.CameraId, o => o.Ignore())
            .ForMember(d => d.ServiceName, o => o.MapFrom(_ => AuditServiceName.ProcessingSystem))
            .ForMember(d => d.EventType, o => o.MapFrom(_ => AuditEventType.OrderFailedEvent))
            .ForMember(d => d.EventAction, o => o.MapFrom(_ => AuditEventAction.Failed))
            .ForMember(d => d.EventOutcome, o => o.MapFrom(_ => AuditEventOutcome.Failure))
            .ForMember(d => d.StatusFrom, o => o.MapFrom(_ => OrderLifecycleStatus.ProcessingStarted))
            .ForMember(d => d.StatusTo, o => o.MapFrom(_ => OrderLifecycleStatus.Failed))
            .ForMember(d => d.FailureReason, o => o.MapFrom(src => src.Reason))
            .ForMember(d => d.PayloadJson, o => o.MapFrom(src => JsonSerializer.Serialize(src, new JsonSerializerOptions())))
            .ForMember(d => d.OccurredAtUtc, o => o.MapFrom(src => src.FailedAtUtc));

        CreateMap<AuditEntry, AuditEntryDocument>()
            .ForMember(d => d.ServiceName, o => o.MapFrom(src => src.ServiceName.ToString()))
            .ForMember(d => d.EventType, o => o.MapFrom(src => src.EventType.ToString()))
            .ForMember(d => d.EventAction, o => o.MapFrom(src => src.EventAction.ToString()))
            .ForMember(d => d.EventOutcome, o => o.MapFrom(src => src.EventOutcome.ToString()))
            .ForMember(d => d.StatusFrom, o => o.MapFrom(src => src.StatusFrom.HasValue ? src.StatusFrom.Value.ToString() : null))
            .ForMember(d => d.StatusTo, o => o.MapFrom(src => src.StatusTo.HasValue ? src.StatusTo.Value.ToString() : null));
    }
}