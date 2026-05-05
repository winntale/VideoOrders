using Core.Abstractions.Services;
using Core.Options;

namespace Core.Services;

internal sealed class ArchiveFileStorage(ArchiveStorageOptions options) : IArchiveFileStorage
{
    public string BuildStoragePath(string storedFileName)
    {
        Directory.CreateDirectory(options.RootPath);
        return Path.Combine(options.RootPath, storedFileName);
    }

    public FileStream OpenRead(string storagePath)
    {
        return new FileStream(storagePath, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    public bool Exists(string storagePath)
    {
        return File.Exists(storagePath);
    }
}