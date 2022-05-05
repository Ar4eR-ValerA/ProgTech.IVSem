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

        var selectedNode = suitableNodes[new Random().Next(0, suitableNodes.Count - 1)];
        _fileSystemService.SendFile(filePath, selectedNode.IpAddress, selectedNode.Port, newPath);
        selectedNode.FreeMemory -= (int)file.Length;

        _fileNodes.Add(newPath, selectedNode);
    }

    public void DeleteFile(string filePath)
    {
        var node = FilesNodes[filePath];

        _fileSystemService.DeleteFile(filePath, node.IpAddress, node.Port);
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
        }
    }

    public void Balance()
    {
        var fileSizes = new SortedDictionary<string, int>();

        foreach (var (filePath, node) in _fileNodes)
        {
            _fileSystemService.SaveFile(filePath, node.IpAddress, node.Port, filePath);
            _fileSystemService.DeleteFile(filePath, node.IpAddress, node.Port);
            _fileNodes.Remove(filePath);

            fileSizes.Add(filePath, (int)new FileInfo(filePath).Length);
        }

        if (fileSizes.Count == 0)
        {
            return;
        }

        var firstFile = fileSizes.First();
        var suitableNodes = _nodes.Where(n => n.FreeMemory > firstFile.Value).ToList();
        var prevNode = Nodes[new Random().Next(0, suitableNodes.Count - 1)];

        _fileSystemService.SendFile(firstFile.Key, prevNode.IpAddress, prevNode.Port, firstFile.Key);
        _fileNodes.Add(firstFile.Key, prevNode);
        prevNode.FreeMemory -= firstFile.Value;
        fileSizes.Remove(firstFile.Key);
        File.Delete(firstFile.Key);

        while (fileSizes.Count > 0)
        {
            foreach (var node in Nodes)
            {
                while (prevNode.Size - prevNode.FreeMemory > node.Size - node.FreeMemory &&
                       fileSizes.Count > 0 &&
                       fileSizes.First().Value < node.Size)
                {
                    var filePath = fileSizes.First();
                    AddFile(filePath.Key, filePath.Key, node);
                    fileSizes.Remove(filePath.Key);
                    File.Delete(filePath.Key);
                }
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
                    break;

                case "add-file":
                    AddFile(command.Arguments[0], command.Arguments[1]);
                    break;

                case "delete-file":
                    DeleteFile(command.Arguments[0]);
                    break;

                case "clean-node":
                    CleanNode(command.Arguments[0]);
                    break;

                case "balance":
                    Balance();
                    break;

                case "exec":
                    Execute(command.Arguments[0]);
                    break;
            }
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