using System.Net;
using DistributedFileSystem.Services;

var nodeService = new NodeService(new WindowsFileSystemService());

/*nodeService.AddNode("Node8888", IPAddress.Parse("127.0.0.1"), 8888, 1048576);
nodeService.AddFile(@"E:\Desktop\Распределённая файловая система\Server\Test1.txt", "Test1.txt");
nodeService.AddFile(@"E:\Desktop\Распределённая файловая система\Server\Test2.txt", "Test2.txt");
nodeService.AddFile(@"E:\Desktop\Распределённая файловая система\Server\Test1.txt", @"T\Test1.txt");
nodeService.AddFile(@"E:\Desktop\Распределённая файловая система\Server\Test2.txt", @"T\Test2.txt");
nodeService.DeleteFile(@"Test1.txt");
nodeService.DeleteFile(@"T\Test1.txt");*/

nodeService.Execute(
    @"E:\ITMO prog\Prog\C#\Tech-Ar4eR-ValerA\Lab4\DistributedFileSystem\DistributedFileSystem.Server\commands.json");