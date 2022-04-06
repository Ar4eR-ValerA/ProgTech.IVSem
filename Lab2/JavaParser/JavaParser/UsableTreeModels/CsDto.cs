namespace AntlrExample.UsableTreeModels;

public class CsDto
{
    private List<string> _args;

    public CsDto()
    {
        _args = new List<string>();
    }

    public string Name;
    public IReadOnlyList<string> Args => _args;
}