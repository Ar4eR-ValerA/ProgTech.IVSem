using System.Net.Sockets;
using System.Text;
using DistributedFileSystem.Node8888.Services.Interfaces;

namespace DistributedFileSystem.Node8888.Services;

public class WindowsFileSystemService : IFileSystemService
{
    public WindowsFileSystemService(string path)
    {
        Path = path;
    }
    
    public string Path { get; }

    public void SaveFile(TcpClient tcpClient)
    {
        var streamReader = new StreamReader(tcpClient.GetStream());

        var fileNameLength = ReadInt(tcpClient.GetStream());
        var fileName = ReadString(fileNameLength, tcpClient.GetStream());

        var fileSize = ReadInt(tcpClient.GetStream());
        var fullPath = $@"{Path}\{fileName}";
        
        var buffer = new byte[fileSize];
                
        tcpClient.GetStream().Read(buffer, 0, fileSize);
        streamReader.Close();

        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fullPath) ?? string.Empty);
        using var fileStream = new FileStream(fullPath, FileMode.Create);
        fileStream.Write(buffer, 0, buffer.Length);
        
        fileStream.Flush();
        fileStream.Close();
    }

    public void SendFile(TcpClient tcpClient)
    {
        var streamReader = new StreamReader(tcpClient.GetStream());
        
        var fileNameLength = ReadInt(tcpClient.GetStream());
        var fileName = ReadString(fileNameLength, tcpClient.GetStream());
        var fullPath = Path + fileName;
        
        var targetFileLength = ReadInt(tcpClient.GetStream());
        var targetFile = ReadString(targetFileLength, tcpClient.GetStream());
        streamReader.Close();

        var bytes = File.ReadAllBytes(fullPath);

        using var streamWriter = new StreamWriter(tcpClient.GetStream());
        
        SendString(targetFile, streamWriter);
        
        streamWriter.Write(Encoding.Default.GetString(BitConverter.GetBytes(bytes.Length)));
        streamWriter.Flush();

        tcpClient.Client.SendFile(fullPath);
        streamWriter.Flush();

        streamWriter.Close();
    }
    
    public void DeleteFile(TcpClient tcpClient)
    {
        var streamReader = new StreamReader(tcpClient.GetStream());

        var fileNameLength = ReadInt(tcpClient.GetStream());
        var fileName = ReadString(fileNameLength, tcpClient.GetStream());
        streamReader.Close();
        
        var fullPath = $@"{Path}\{fileName}";
        
        File.Delete(fullPath);
    }

    private string ReadString(int length, NetworkStream stream)
    {
        var buffer = new byte[length];
        stream.Read(buffer, 0, buffer.Length);
        return Encoding.Default.GetString(buffer);
    }

    private int ReadInt(NetworkStream stream)
    {
        var buffer = new byte[4];
        stream.Read(buffer, 0, buffer.Length);
        return BitConverter.ToInt32(buffer);
    }

    private void SendString(string str, StreamWriter streamWriter)
    {
        streamWriter.Write(Encoding.Default.GetString(BitConverter.GetBytes(str.Length)));
        streamWriter.Write(str);
        streamWriter.Flush();
    }
}