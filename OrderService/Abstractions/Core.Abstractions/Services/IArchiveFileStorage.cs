namespace Core.Abstractions.Services;

public interface IArchiveFileStorage
{
    string BuildStoragePath(string storedFileName);
    FileStream OpenRead(string storagePath);
    bool Exists(string storagePath);
}