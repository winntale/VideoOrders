using AutoMapper;
using Core.Abstractions.OperationModels;
using Dal.Abstractions.Models;

namespace Core.CoreModelsMappingProfiles;

internal sealed class GetOrderByIdOperationMappingProfiles : Profile
{
    public GetOrderByIdOperationMappingProfiles()
    {
        CreateMap<GetOrderByIdOperationModel, GetOrderByIdRepositoryModel>();
    }
}