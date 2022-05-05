using System.Net.Sockets;

namespace DistributedFileSystem.Domain.Services.Interfaces;

public interface IFileSystemService
{
    public void SaveFile(TcpClient tcpClient);
    public void SendFile(TcpClient tcpClient);
    public void DeleteFile(TcpClient tcpClient);
}