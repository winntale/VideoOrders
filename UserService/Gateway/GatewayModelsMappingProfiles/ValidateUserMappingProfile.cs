using AutoMapper;
using Core.Abstractions.OperationModels;
using Gateway.Models;

namespace Gateway.GatewayModelsMappingProfiles;

internal sealed class ValidateUserMappingProfile : Profile
{
    public ValidateUserMappingProfile()
    {
        CreateMap<ValidateUserAccessDto, ValidateUserAccessOperationModel>();

        CreateMap<UserAccessValidationResultOperationModel, UserAccessValidationResponseDto>();
    }
}