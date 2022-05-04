using System.Net.Sockets;
using DistributedFileSystem.Node8888.Services.Interfaces;

namespace DistributedFileSystem.Node8888.Services;

public class WindowsFileSystemService : IFileSystemService
{
    public void SaveFile(TcpClient tcpClient)
    {
        var streamReader = new StreamReader(tcpClient.GetStream());
        
        string fileSize = streamReader.ReadLine();
        string fileName = streamReader.ReadLine();

        int length = Convert.ToInt32(fileSize);
        byte[] buffer = new byte[length];
                
        tcpClient.GetStream().Read(buffer, 0, length);

        using var fileStream = new FileStream(fileName ?? string.Empty, FileMode.Create);
        fileStream.Write(buffer, 0, buffer.Length);
        fileStream.Flush();
        fileStream.Close();
    }

    public void SendFile(TcpClient tcpClient)
    {
        var streamReader = new StreamReader(tcpClient.GetStream());
        
        string fileName = streamReader.ReadLine();
        string targetFile = streamReader.ReadLine();

        byte[] bytes = File.ReadAllBytes(fileName);

        var streamWriter = new StreamWriter(tcpClient.GetStream());
        streamWriter.WriteLine(bytes.Length.ToString());
        streamWriter.Flush();

        streamWriter.WriteLine(targetFile);
        streamWriter.Flush();

        tcpClient.Client.SendFile(fileName);
        streamWriter.Flush();
        File.Delete(fileName);

        streamWriter.Close();
    }
}