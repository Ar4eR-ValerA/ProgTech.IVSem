using System.Net;

namespace DistributedFileSystem.Services.ValueObjects;

public class Node
{
    public Node(string name, IPAddress ipAddress, int port, int size)
    {
        Name = name;
        Id = Guid.NewGuid();
        Size = size;
        FreeMemory = size;
        IpAddress = ipAddress;
        Port = port;
    }

    public string Name { get; }
    public Guid Id { get; }
    public int Size { get; }
    public int FreeMemory { get; set; }
    public IPAddress IpAddress { get; }
    public int Port { get; }
}