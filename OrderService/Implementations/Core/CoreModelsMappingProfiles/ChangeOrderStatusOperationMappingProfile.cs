using AutoMapper;
using Core.Abstractions.OperationModels;
using Dal.Abstractions.Models;

namespace Core.CoreModelsMappingProfiles;

internal sealed class ChangeOrderStatusOperationMappingProfile : Profile
{
    public ChangeOrderStatusOperationMappingProfile()
    {
        CreateMap<ChangeOrderStatusOperationModel, GetOrderByIdRepositoryModel>();
    }
}