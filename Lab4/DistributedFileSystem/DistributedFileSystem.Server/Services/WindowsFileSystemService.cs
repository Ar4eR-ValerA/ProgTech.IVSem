using System.Net;
using System.Net.Sockets;
using System.Text;
using DistributedFileSystem.Services.Interfaces;

namespace DistributedFileSystem.Services;

public class WindowsFileSystemService : IFileSystemService
{
    public void SaveFileToServer(string filePath, IPAddress ipAddress, int port, string newFilePath)
    {
        var tcpClient = new TcpClient(ipAddress.ToString(), port);

        var stream = tcpClient.GetStream();

        SendString("send-to-server", stream);
        SendString(filePath, stream);

        ReadFile(newFilePath, stream);
        stream.Close();
        tcpClient.Close();
    }

    public void TransferFileToOtherNode(
        string filePath,
        IPAddress ipAddressFrom,
        int portFrom,
        IPAddress ipAddressTo,
        int portTo)
    {
        var tcpClientFrom = new TcpClient(ipAddressFrom.ToString(), portFrom);
        var streamFrom = tcpClientFrom.GetStream();

        SendString("send-to-node", streamFrom);
        SendString(filePath, streamFrom);
        SendString(ipAddressTo.ToString(), streamFrom);
        SendString(portTo.ToString(), streamFrom);

        streamFrom.Close();
        tcpClientFrom.Close();
    }


    public void SendFileToNode(string filePath, IPAddress ipAddress, int port, string newFilePath)
    {
        var tcpClient = new TcpClient(ipAddress.ToString(), port);

        var stream = tcpClient.GetStream();

        SendString("save-from-server", stream);
        SendString(newFilePath, stream);

        tcpClient.Client.SendFile(filePath);
        stream.Flush();

        stream.Close();
        tcpClient.Close();
    }

    public void DeleteFile(string filePath, IPAddress ipAddress, int port)
    {
        var tcpClient = new TcpClient(ipAddress.ToString(), port);
        var stream = tcpClient.GetStream();

        SendString("delete", stream);
        SendString(filePath, stream);

        stream.Close();
        tcpClient.Close();
    }

    public long GetFileSize(string filePath, IPAddress ipAddress, int port)
    {
        var tcpClient = new TcpClient(ipAddress.ToString(), port);
        var stream = tcpClient.GetStream();

        SendString("send-file-size", stream);
        SendString(filePath, stream);

        var fileSize = ReadNumber(stream);
        
        stream.Close();
        tcpClient.Close();

        return fileSize;
    }

    private string ReadString(int length, NetworkStream stream)
    {
        var buffer = new byte[length];
        stream.Read(buffer, 0, buffer.Length);
        return Encoding.Default.GetString(buffer);
    }

    private long ReadNumber(NetworkStream stream)
    {
        var buffer = new byte[8];
        stream.Read(buffer, 0, buffer.Length);
        return BitConverter.ToInt64(buffer);
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