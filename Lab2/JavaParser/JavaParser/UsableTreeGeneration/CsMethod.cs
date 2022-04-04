namespace AntlrExample.UsableTreeGeneration;

public class CsMethod
{
    private List<string> _args;

    public CsMethod(string name, string url, string restVerb, List<string> args)
    {
        Name = name;
        URL = url;
        RestVerb = restVerb;
        _args = args;
    }

    public string Name { get; private set; }
    public string URL { get; private set; }
    public string RestVerb { get; private set; }
    
    //TODO: Нужны более сложные аргументы.
    public IReadOnlyList<string> Args => _args;
}