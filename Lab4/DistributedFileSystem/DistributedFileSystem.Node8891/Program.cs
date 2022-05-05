using System.Net;
using DistributedFileSystem.Domain.Services;
using DistributedFileSystem.Domain.TcpTools;

IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
const int port = 8891;
const string path = @"E:\Desktop\Распределённая файловая система\Node8891";

var listener = new Listener();
listener.Listen(ipAddress, port, new WindowsFileSystemService(path));