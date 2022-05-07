using System.Net.Sockets;

namespace DistributedFileSystem.Domain.Services.Interfaces;

public interface IFileSystemService
{
    public void SaveFileFromServer(TcpClient tcpClient);
    public void SaveFileFromNode(TcpClient tcpClient);
    public void SendFileToServer(TcpClient tcpClient);
    public void SendFileToNode(TcpClient tcpClient);
    public void DeleteFile(TcpClient tcpClient);
    public void SendFileSize(TcpClient tcpClient);
}