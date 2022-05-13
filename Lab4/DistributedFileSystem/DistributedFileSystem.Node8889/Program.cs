using System.Net;
using DistributedFileSystem.Domain.Services;
using DistributedFileSystem.Domain.TcpTools;

IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
const int port = 8889;
const string path = @"D:\DFS\Node8889";

var listener = new Listener();
listener.Listen(ipAddress, port, new WindowsFileSystemService(path));