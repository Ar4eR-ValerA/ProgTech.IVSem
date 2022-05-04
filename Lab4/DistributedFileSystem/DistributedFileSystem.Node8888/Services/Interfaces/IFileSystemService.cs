using System.Net.Sockets;

namespace DistributedFileSystem.Node8888.Services.Interfaces;

public interface IFileSystemService
{
    public void SaveFile(TcpClient tcpClient);
    public void SendFile(TcpClient tcpClient);
}