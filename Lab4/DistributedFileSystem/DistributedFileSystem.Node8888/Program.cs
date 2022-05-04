using System.Net;
using DistributedFileSystem.Node8888.Services;
using DistributedFileSystem.Node8888.TcpTools;

IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
const int port = 8888;
const string path = @"E:\Desktop\Распределённая файловая система\Node8888";

var listener = new Listener();
listener.Listen(ipAddress, port, new WindowsFileSystemService(path));