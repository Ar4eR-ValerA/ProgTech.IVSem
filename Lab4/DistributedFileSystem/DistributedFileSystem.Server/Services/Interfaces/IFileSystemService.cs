using System.Net;

namespace DistributedFileSystem.Services.Interfaces;

public interface IFileSystemService
{
    public void SaveFile(string filePath, IPAddress ipAddress, int port, string newFilePath);
    public void SendFile(string filePath, IPAddress ipAddress, int port, string newFilePath);
    public void DeleteFile(string filePath, IPAddress ipAddress, int port);
}