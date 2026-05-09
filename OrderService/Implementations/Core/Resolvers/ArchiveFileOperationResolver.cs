using AutoMapper;
using Dal.Abstractions.Enums;
using Core.Abstractions.OperationModels;
using Dal.Abstractions.Entities;

namespace Core.Resolvers;

public sealed class ArchiveFileOperationResolver : IValueResolver<Order, OrderDetailsOperationModel, ArchiveFileOperationModel?>
{
    public ArchiveFileOperationModel? Resolve(Order source, OrderDetailsOperationModel destination,
        ArchiveFileOperationModel? destMember, ResolutionContext context)
    {
        if (source.ArchiveFile is null)
        {
            return null;
        }

        return new ArchiveFileOperationModel
        {
            OrderId = source.Id,
            OriginalFileName = source.ArchiveFile.OriginalFileName,
            ContentType = source.ArchiveFile.ContentType,
            FileSize = source.ArchiveFile.FileSize,
            IsReady = source.Status == OrderStatus.Completed,
            DownloadUrl = $"/Orders/{source.Id}/download",
            StreamUrl = $"/Orders/{source.Id}/stream"
        };
    }
}