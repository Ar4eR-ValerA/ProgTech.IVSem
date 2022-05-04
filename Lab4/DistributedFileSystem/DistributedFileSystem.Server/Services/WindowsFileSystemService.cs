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

        using var streamWriter = new StreamWriter(tcpClient.GetStream());

        SendString("send", streamWriter);
        SendString(filePath, streamWriter);
        SendString(newFilePath, streamWriter);
        streamWriter.Close();

        var streamReader = new StreamReader(tcpClient.GetStream());

        var fileNameLength = ReadInt(tcpClient.GetStream());
        var fileName = ReadString(fileNameLength, tcpClient.GetStream());
        
        var fileSize = ReadInt(tcpClient.GetStream());
        var buffer = new byte[fileSize];

        tcpClient.GetStream().Read(buffer, 0, fileSize);
        streamReader.Close();

        using var fileStream = new FileStream(fileName, FileMode.Create);
        fileStream.Write(buffer, 0, buffer.Length);
        fileStream.Flush();
        fileStream.Close();
        tcpClient.Close();
    }

    public void SendFile(string filePath, IPAddress ipAddress, int port, string newFilePath)
    {
        using var tcpClient = new TcpClient(ipAddress.ToString(), port);
        var bytes = File.ReadAllBytes(filePath);

        var streamWriter = new StreamWriter(tcpClient.GetStream());
        
        SendString("save", streamWriter);
        SendString(newFilePath, streamWriter);

        streamWriter.Write(Encoding.Default.GetString(BitConverter.GetBytes(bytes.Length)));
        streamWriter.Flush();
        
        tcpClient.Client.SendFile(filePath);
        streamWriter.Flush();
            
        streamWriter.Close();
        tcpClient.Close();
    }
    
    public void DeleteFile(string filePath, IPAddress ipAddress, int port)
    {
        using var tcpClient = new TcpClient(ipAddress.ToString(), port);
        var streamWriter = new StreamWriter(tcpClient.GetStream());
        
        SendString("delete", streamWriter);
        SendString(filePath, streamWriter);

        streamWriter.Close();
        tcpClient.Close();
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