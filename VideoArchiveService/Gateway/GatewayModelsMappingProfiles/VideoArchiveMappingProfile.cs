using AutoMapper;
using Core.Abstractions.OperationModels;
using Gateway.Models;

namespace Gateway.GatewayModelsMappingProfiles;

public sealed class VideoArchiveMappingProfile : Profile
{
    public VideoArchiveMappingProfile()
    {
        CreateMap<ValidateArchiveAvailabilityRequestDto, ValidateArchiveAvailabilityOperationModel>();
        CreateMap<ArchiveAvailabilityResultOperationModel, ArchiveAvailabilityResponseDto>();
    }
}