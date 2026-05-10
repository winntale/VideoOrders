using AutoMapper;
using Core.Abstractions.OperationModels;
using Gateway.Models;

namespace Gateway.GatewayModelsMappingProfiles;

internal sealed class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        CreateMap<LoginRequestDto, LoginUserOperationModel>();
        CreateMap<RegisterRequestDto, RegisterUserOperationModel>();
        CreateMap<AuthenticatedUserOperationModel, AuthenticatedUserDto>();
    }
}
