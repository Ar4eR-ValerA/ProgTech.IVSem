using System.Net;
using System.Text.Json;
using DistributedFileSystem.Services.Interfaces;
using DistributedFileSystem.Services.ValueObjects;

namespace DistributedFileSystem.Services;

public class NodeService
{
    private readonly List<Node> _nodes;
    private readonly Dictionary<string, Node> _fileNodes;
    private readonly IFileSystemService _fileSystemService;

    public NodeService(IFileSystemService fileSystemService)
    {
        _fileSystemService = fileSystemService;
        _nodes = new List<Node>();
        _fileNodes = new Dictionary<string, Node>();
    }


    public IReadOnlyList<Node> Nodes => _nodes;
    public IReadOnlyDictionary<string, Node> FilesNodes => _fileNodes;

    public void AddNode(string name, IPAddress ipAddress, int port, int size)
    {
        var node = new Node(name, ipAddress, port, size);

        _nodes.Add(node);
    }

    public void AddFile(string filePath, string newPath)
    {
        var file = new FileInfo(filePath);

        var suitableNodes = _nodes.Where(n => n.FreeMemory > file.Length).ToList();

        if (suitableNodes.Count == 0)
        {
            throw new Exception("There is no nodes with enough space");
        }

        var selectedNode = suitableNodes[new Random().Next(0, suitableNodes.Count)];
        _fileSystemService.SendFile(filePath, selectedNode.IpAddress, selectedNode.Port, newPath);
        selectedNode.FreeMemory -= (int)file.Length;

        _fileNodes.Add(newPath, selectedNode);
    }

    public void DeleteFile(string filePath)
    {
        var node = FilesNodes[filePath];

        _fileSystemService.DeleteFile(filePath, node.IpAddress, node.Port);
        node.FreeMemory += (int)new FileInfo(filePath).Length;
        _fileNodes.Remove(filePath);
    }

    public void CleanNode(string name)
    {
        var node = _nodes.First(n => n.Name == name);
        _nodes.Remove(node);

        foreach (var (filePath, _) in _fileNodes.Where(fileNode => fileNode.Value == node).ToList())
        {
            _fileSystemService.SaveFile(filePath, node.IpAddress, node.Port, filePath);
            _fileSystemService.DeleteFile(filePath, node.IpAddress, node.Port);
            _fileNodes.Remove(filePath);
            AddFile(filePath, filePath);

            File.Delete(filePath);
            GetInformation(filePath);
        }
    }

    public void Balance()
    {
        var fileSizes = new List<(string filePath, int size)>();

        foreach (var (filePath, node) in _fileNodes)
        {
            var file = new FileInfo(filePath);

            _fileSystemService.SaveFile(filePath, node.IpAddress, node.Port, filePath);
            _fileSystemService.DeleteFile(filePath, node.IpAddress, node.Port);
            node.FreeMemory += (int)file.Length;
            _fileNodes.Remove(filePath);

            fileSizes.Add((filePath, (int)file.Length));
            GetInformation(filePath);
        }

        if (fileSizes.Count == 0)
        {
            return;
        }

        fileSizes = fileSizes.OrderByDescending(x => x.size).ToList();

        if (_nodes.Count == 0)
        {
            return;
        }

        const double balanceConst = 0.9;
        while (fileSizes.Count > 0)
        {
            var prevPart = 0.01;
            foreach (var node in Nodes)
            {
                var currentPart = 0.0;
                while (fileSizes.Count > 0 &&
                       prevPart * balanceConst > currentPart &&
                       fileSizes.First().size < node.Size)
                {
                    var file = fileSizes.First();
                    AddFile(file.filePath, file.filePath, node);

                    currentPart += (double)fileSizes.First().size / node.Size;

                    fileSizes.Remove(file);
                    File.Delete(file.filePath);
                    GetInformation(file.filePath);
                }

                prevPart = currentPart;
            }
        }
    }

    public void Execute(string commandFilePath)
    {
        var commandsJson = File.ReadAllText(commandFilePath);
        var commands = JsonSerializer.Deserialize<CommandFile>(commandsJson);

        if (commands is null)
        {
            return;
        }

        foreach (var command in commands.Commands)
        {
            switch (command.Name)
            {
                case "add-node":
                    AddNode(
                        command.Arguments[0],
                        IPAddress.Parse(command.Arguments[1]),
                        Convert.ToInt32(command.Arguments[2]),
                        Convert.ToInt32(command.Arguments[3]));
                    GetInformation(command);
                    break;

                case "add-file":
                    AddFile(command.Arguments[0], command.Arguments[1]);
                    GetInformation(command);
                    break;

                case "delete-file":
                    DeleteFile(command.Arguments[0]);
                    GetInformation(command);
                    break;

                case "clean-node":
                    CleanNode(command.Arguments[0]);
                    GetInformation(command);
                    break;

                case "balance":
                    Balance();
                    GetInformation(command);
                    break;

                case "exec":
                    Execute(command.Arguments[0]);
                    GetInformation(command);
                    break;
            }
        }
    }

    public void GetInformation(Command command)
    {
        Console.Clear();

        Console.WriteLine($"Command {command.Name} done. Args:");
        foreach (var argument in command.Arguments)
        {
            Console.WriteLine($"{argument}");
        }

        Console.WriteLine();

        Console.WriteLine("Nodes:");
        foreach (var node in Nodes)
        {
            Console.WriteLine(
                $"{node.Name}: {(int)((double)(node.Size - node.FreeMemory) / node.Size * 100.0)} % " +
                $"({(node.Size - node.FreeMemory) / 1048576} / {node.Size / 1048576} MB)");
        }
    }

    public void GetInformation(string filePath)
    {
        Console.Clear();

        Console.WriteLine($"File {filePath} was moved");

        Console.WriteLine("Nodes:");
        foreach (var node in Nodes)
        {
            Console.WriteLine(
                $"{node.Name}: {(int)((double)(node.Size - node.FreeMemory) / node.Size * 100.0)} % " +
                $"({(node.Size - node.FreeMemory) / 1048576} / {node.Size / 1048576} MB)");
        }
    }

    private void AddFile(string filePath, string newPath, Node node)
    {
        var file = new FileInfo(filePath);

        _fileSystemService.SendFile(filePath, node.IpAddress, node.Port, newPath);
        node.FreeMemory -= (int)file.Length;

        _fileNodes.Add(newPath, node);
    }
}