using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using DistributedFileSystem.Services;

namespace DistributedFileSystem.Benchmark;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class Benchmark
{
    private NodeService? _nodeService;
    
    [GlobalSetup]
    public void Setup()
    {
        _nodeService = new NodeService(new WindowsFileSystemService());
    }
    
    [IterationSetup]
    public void ClearNodes()
    {
        _nodeService = new NodeService(new WindowsFileSystemService());
        
        for (int i = 88; i <= 91; i++)
        {
            var dir = new DirectoryInfo(@"E:\Desktop\Распределённая файловая система\Node88" + i);
            foreach (var file in dir.GetFiles())
            {
                File.Delete(file.FullName);
            }
        }
    }
    
    [Benchmark]
    public void NodeService()
    {
        _nodeService!.Execute(
            @"E:\ITMO prog\Prog\C#\Tech-Ar4eR-ValerA\Lab4\DistributedFileSystem\DistributedFileSystem.Server\commands.json");
    }
}