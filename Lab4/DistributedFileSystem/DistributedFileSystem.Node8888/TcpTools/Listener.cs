using System.Net;
using System.Net.Sockets;
using System.Text;
using DistributedFileSystem.Node8888.Services.Interfaces;

namespace DistributedFileSystem.Node8888.TcpTools;

public class Listener
{
    public void Listen(IPAddress ipAddress, int port, IFileSystemService fileSystemService)
    {
        var tcpListener = new TcpListener(ipAddress, port);

        tcpListener.Start();

        while (true)
        {
            TcpClient tcpClient = tcpListener.AcceptTcpClient();
            
            var modeLength = ReadInt(tcpClient.GetStream());
            var mode = ReadString(modeLength, tcpClient.GetStream());

            switch (mode)
            {
                case "save":
                    fileSystemService.SaveFile(tcpClient);

                    tcpClient.Close();
                    break;

                case "send":
                {
                    fileSystemService.SendFile(tcpClient);
                    
                    tcpClient.Close();
                    break;
                }
                
                case "delete":
                    fileSystemService.DeleteFile(tcpClient);
                    
                    tcpClient.Close();
                    break;

                case "stop":
                    tcpClient.Close();
                    tcpListener.Stop();
                    return;
            }
        }
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
}