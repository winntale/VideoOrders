using AutoMapper;
using Core.Abstractions.OperationModels;
using Dal.Abstractions.Entities;
using Dal.Abstractions.Models;

namespace Core.CoreModelsMappingProfiles;

internal sealed class GetOrderByIdOperationMappingProfiles : Profile
{
    public GetOrderByIdOperationMappingProfiles()
    {
        CreateMap<GetOrderByIdOperationModel, GetOrderByIdRepositoryModel>();
        
        CreateMap<ArchiveFile, ArchiveFileOperationModel>()
            .ForMember(d => d.OrderId, o => o.Ignore())
            .ForMember(d => d.IsReady, o => o.Ignore())
            .ForMember(d => d.DownloadUrl, o => o.Ignore())
            .ForMember(d => d.StreamUrl, o => o.Ignore());
    }
}