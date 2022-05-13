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

    public void SaveFileFromServer(TcpClient tcpClient)
    {
        var stream = tcpClient.GetStream();

        var fileNameLength = ReadStringLength(stream);
        var fileName = ReadString(fileNameLength, stream);
        var fullPath = $@"{Path}\{fileName}";
        
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fullPath) ?? string.Empty);
        ReadFile(fullPath, stream);
        stream.Close();
    }

    public void SaveFileFromNode(TcpClient tcpClient)
    {
        var stream = tcpClient.GetStream();

        var fileNameLength = ReadStringLength(stream);
        var fileName = ReadString(fileNameLength, stream);
        var fullPath = $@"{Path}\{fileName}";
        
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fullPath) ?? string.Empty);
        
        ReadFile(fullPath, stream);
        
        stream.Close();
        tcpClient.Close();
    }

    public void SendFileToServer(TcpClient tcpClient)
    {
        var stream = tcpClient.GetStream();
        
        var fileNameLength = ReadStringLength(stream);
        var fileName = ReadString(fileNameLength, stream);
        var fullPath = $@"{Path}\{fileName}";

        tcpClient.Client.SendFile(fullPath);
        stream.Flush();

        stream.Close();
    }

    public void SendFileToNode(TcpClient tcpClient)
    {
        var stream = tcpClient.GetStream();
        
        var fileNameLength = ReadStringLength(stream);
        var fileName = ReadString(fileNameLength, stream);
        var fullPath = $@"{Path}\{fileName}";

        var ipAddressLength = ReadStringLength(stream);
        var ipAddress = ReadString(ipAddressLength, stream);
        
        var portLength = ReadStringLength(stream);
        var port = ReadString(portLength, stream);

        stream.Flush();
        stream.Close();
        
        var newTcpClient = new TcpClient(ipAddress, Convert.ToInt32(port));
        var newStream = newTcpClient.GetStream();
        
        SendString("save-from-node", newStream);
        SendString(fileName, newStream);
        
        newTcpClient.Client.SendFile(fullPath);
        newStream.Close();
        newTcpClient.Close();
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

    public void SendFileSize(TcpClient tcpClient)
    {
        var stream = tcpClient.GetStream();

        var fileNameLength = ReadStringLength(stream);
        var fileName = ReadString(fileNameLength, stream);
        var fullPath = $@"{Path}\{fileName}";

        var fileSize = new FileInfo(fullPath).Length;
        stream.Write(BitConverter.GetBytes(fileSize));
        
        stream.Close();
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