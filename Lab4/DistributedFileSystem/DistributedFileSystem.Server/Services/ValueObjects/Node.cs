using System.Net;

namespace DistributedFileSystem.Services.ValueObjects;

public class Node
{
    public Node(string name, IPAddress ipAddress, int port, long size)
    {
        Name = name;
        Size = size;
        FreeMemory = size;
        IpAddress = ipAddress;
        Port = port;
    }

    public string Name { get; }
    public long Size { get; }
    public long FreeMemory { get; set; }
    public IPAddress IpAddress { get; }
    public int Port { get; }
}