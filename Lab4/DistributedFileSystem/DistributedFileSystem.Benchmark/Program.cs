using BenchmarkDotNet.Running;

namespace DistributedFileSystem.Benchmark
{
    public class Program
    {
        public static void Main()
        {
            BenchmarkRunner.Run<Benchmark>();
        }
    }
}