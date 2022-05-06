using System.Net.Sockets;
using System.Text;
using DistributedFileSystem.Domain.Services.Interfaces;

namespace DistributedFileSystem.Domain.Services;

public class WindowsFileSystemService : IFileSystemService
{
    public WindowsFileSystemService(string path)
    {
        Path = path;
    }
    
    public string Path { get; }

    public void SaveFile(TcpClient tcpClient)
    {
        var stream = tcpClient.GetStream();

        var fileNameLength = ReadStringLength(stream);
        var fileName = ReadString(fileNameLength, stream);
        var fullPath = $@"{Path}\{fileName}";
        
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fullPath) ?? string.Empty);
        ReadFile(fullPath, stream);
        stream.Close();
    }

    public void SendFile(TcpClient tcpClient)
    {
        var stream = tcpClient.GetStream();
        
        var fileNameLength = ReadStringLength(stream);
        var fileName = ReadString(fileNameLength, stream);
        var fullPath = $@"{Path}\{fileName}";
        
        var targetFilePathLength = ReadStringLength(stream);
        var targetFilePath = ReadString(targetFilePathLength, stream);

        SendString(targetFilePath, stream);
        
        tcpClient.Client.SendFile(fullPath);
        stream.Flush();

        stream.Close();
    }
    
    public void DeleteFile(TcpClient tcpClient)
    {
        var stream = tcpClient.GetStream();

        var fileNameLength = ReadStringLength(stream);
        var fileName = ReadString(fileNameLength, stream);
        stream.Close();
        
        var fullPath = $@"{Path}\{fileName}";
        
        File.Delete(fullPath);
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