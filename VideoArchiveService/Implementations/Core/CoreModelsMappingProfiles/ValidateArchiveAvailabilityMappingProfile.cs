using AutoMapper;
using Core.Abstractions.OperationModels;
using Dal.Abstractions.Models;

namespace Core.CoreModelsMappingProfiles;

internal sealed class ValidateArchiveAvailabilityMappingProfile : Profile
{
    public ValidateArchiveAvailabilityMappingProfile()
    {
        CreateMap<ValidateArchiveAvailabilityOperationModel, CameraRepositoryModel>();
        
        CreateMap<ValidateArchiveAvailabilityOperationModel, VideoSegmentRepositoryModel>();
    }
}