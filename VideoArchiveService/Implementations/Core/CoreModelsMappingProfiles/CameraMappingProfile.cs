using AutoMapper;
using Core.Abstractions.OperationModels;
using Dal.Abstractions.Entities;

namespace Core.CoreModelsMappingProfiles;

internal sealed class CameraMappingProfile : Profile
{
    public CameraMappingProfile()
    {
        CreateMap<Camera, CameraOperationModel>();
    }
}
