using System.Net;

namespace DistributedFileSystem.Services.Interfaces;

public interface IFileSystemService
{
    public void SaveFileToServer(string filePath, IPAddress ipAddress, int port, string newFilePath);

    public void TransferFileToOtherNode(
        string filePath,
        IPAddress ipAddressFrom,
        int portFrom,
        IPAddress ipAddressTo,
        int portTo);

    public void SendFileToNode(string filePath, IPAddress ipAddress, int port, string newFilePath);
    public void DeleteFile(string filePath, IPAddress ipAddress, int port);
    public long GetFileSize(string filePath, IPAddress ipAddress, int port);
}