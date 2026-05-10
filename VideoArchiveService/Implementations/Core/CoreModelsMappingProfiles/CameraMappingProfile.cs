using AutoMapper;

namespace Core.CoreModelsMappingProfiles;

internal sealed class CameraMappingProfile : Profile
{
    public CameraMappingProfile()
    {
        // Camera -> CameraOperationModel mapping is done manually in
        // ListCamerasOperation to avoid AutoMapper issues with records that
        // have `required init` properties combined with default-valued
        // collection properties.
    }
}
