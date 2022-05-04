using System.Net;
using System.Net.Sockets;
using DistributedFileSystem.Node8888.Services.Interfaces;

namespace DistributedFileSystem.Node8888.TcpTools;

public class Listener
{
    public static void Listen(IPAddress ipAddress, int port, IFileSystemService fileSystemService)
    {
        var tcpListener = new TcpListener(ipAddress, port);

        tcpListener.Start();

        while (true)
        {
            TcpClient tcpClient = tcpListener.AcceptTcpClient();

            var streamReader = new StreamReader(tcpClient.GetStream());
            string mode = streamReader.ReadLine();

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
            }
        }
    }
}