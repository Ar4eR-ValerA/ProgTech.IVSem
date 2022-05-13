using System.Text.Json.Serialization;

namespace DistributedFileSystem.Services.ValueObjects;

public class CommandFile
{
    [JsonPropertyName("commands")]
    public List<Command> Commands { get; set; }
}