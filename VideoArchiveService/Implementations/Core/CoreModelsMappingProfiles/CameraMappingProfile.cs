using AutoMapper;
using Core.Abstractions.OperationModels;
using Dal.Abstractions.Entities;

namespace Core.CoreModelsMappingProfiles;

internal sealed class CameraMappingProfile : Profile
{
    public CameraMappingProfile()
    {
        CreateMap<Camera, CameraOperationModel>()
            .ForMember(d => d.Segments, o => o.MapFrom(s => s.VideoSegments));

        CreateMap<VideoSegment, SegmentRangeOperationModel>();
    }
}
