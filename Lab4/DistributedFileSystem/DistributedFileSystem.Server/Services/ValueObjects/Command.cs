using System.Text.Json.Serialization;

namespace DistributedFileSystem.Services.ValueObjects;

public class Command
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("args")]
    public List<string> Arguments { get; set; }
}