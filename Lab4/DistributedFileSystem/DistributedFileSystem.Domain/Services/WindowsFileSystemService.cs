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

        var fileSize = ReadStringLength(stream);
        var fullPath = $@"{Path}\{fileName}";
        
        var buffer = new byte[fileSize];
                
        stream.Read(buffer, 0, fileSize);
        stream.Close();

        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fullPath) ?? string.Empty);
        using var fileStream = new FileStream(fullPath, FileMode.Create);
        fileStream.Write(buffer, 0, buffer.Length);
        
        fileStream.Flush();
        fileStream.Close();
    }

    public void SendFile(TcpClient tcpClient)
    {
        var stream = tcpClient.GetStream();
        
        var fileNameLength = ReadStringLength(stream);
        var fileName = ReadString(fileNameLength, stream);
        var fullPath = $@"{Path}\{fileName}";
        
        var targetFileLength = ReadStringLength(stream);
        var targetFile = ReadString(targetFileLength, stream);

        SendString(targetFile, stream);
        
        var bytes = File.ReadAllBytes(fullPath);
        SendInt(bytes.Length, stream);
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
    
    private void SendInt(int number, NetworkStream streamWriter)
    {
        streamWriter.Write(BitConverter.GetBytes(number));
        streamWriter.Flush();
    }
}