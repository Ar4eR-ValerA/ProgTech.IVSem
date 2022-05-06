using System.Net;
using System.Net.Sockets;
using System.Text;
using DistributedFileSystem.Services.Interfaces;

namespace DistributedFileSystem.Services;

public class WindowsFileSystemService : IFileSystemService
{
    public void SaveFile(string filePath, IPAddress ipAddress, int port, string newFilePath)
    {
        using var tcpClient = new TcpClient(ipAddress.ToString(), port);

        var stream = tcpClient.GetStream();
        
        SendString("send", stream);
        SendString(filePath, stream);
        SendString(newFilePath, stream);

        var fileNameLength = ReadStringLength(stream);
        var fileName = ReadString(fileNameLength, stream);
        
        ReadFile(fileName, stream);
        stream.Close();
        tcpClient.Close();
    }

    public void SendFile(string filePath, IPAddress ipAddress, int port, string newFilePath)
    {
        using var tcpClient = new TcpClient(ipAddress.ToString(), port);

        var stream = tcpClient.GetStream();

        SendString("save", stream);
        SendString(newFilePath, stream);

        tcpClient.Client.SendFile(filePath);
        stream.Flush();

        stream.Close();
        tcpClient.Close();
    }

    public void DeleteFile(string filePath, IPAddress ipAddress, int port)
    {
        using var tcpClient = new TcpClient(ipAddress.ToString(), port);
        var stream = tcpClient.GetStream();
        
        SendString("delete", stream);
        SendString(filePath, stream);

        stream.Close();
        tcpClient.Close();
    }

    private string ReadString(int length, NetworkStream stream)
    {
        var buffer = new byte[length];
        stream.Read(buffer, 0, buffer.Length);
        return Encoding.Default.GetString(buffer);
    }

    private int ReadStringLength(NetworkStream stream)
    {
        var buffer = new byte[4];
        stream.Read(buffer, 0, buffer.Length);
        return BitConverter.ToInt32(buffer);
    }

    private void SendString(string str, NetworkStream streamWriter)
    {
        streamWriter.Write(BitConverter.GetBytes(str.Length));
        streamWriter.Write(Encoding.ASCII.GetBytes(str));
        streamWriter.Flush();
    }

    private void ReadFile(string filePath, NetworkStream stream)
    {
        var file = File.Create(filePath);
        
        var buffer = new byte[1024];
        int bytesRead;
        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
        {
            file.Write(buffer, 0, bytesRead);
        }
        file.Close();
    }
}