using Antlr4.Runtime.Tree;

namespace AntlrExample.UsableTreeGeneration;

public class CsMethod
{
    private List<string> _args;

    public CsMethod(JavaParser.ClassBodyDeclarationContext parseTree)
    {
        IsValid = true;
        
        ParseMethodDeclaration(parseTree);
        
        Validate();
    }

    public string Name { get; private set; }
    public string Url { get; private set; }
    public string RestVerb { get; private set; }
    public bool IsValid { get; set; }

    //TODO: Нужны более сложные аргументы.
    public IReadOnlyList<string> Args => _args;

    private void Validate()
    {
        if (string.IsNullOrEmpty(Name))
        {
            IsValid = false;
        }

        if (string.IsNullOrEmpty(Url))
        {
            //IsValid = false;
        }

        if (string.IsNullOrEmpty(RestVerb))
        {
            //IsValid = false;
        }
    }

    private T FindNewContext<T>(IParseTree parseTree) where T : IParseTree
    {
        if (parseTree is null)
        {
            return default;
        }

        for (var i = 0; i < parseTree.ChildCount; i++)
        {
            var child = parseTree.GetChild(i);

            if (child is T childT)
            {
                return childT;
            }
        }

        return default;
    }

    private void ParseMethodDeclaration(JavaParser.ClassBodyDeclarationContext parseTree)
    {
        for (var i = 0; i < parseTree.ChildCount; i++)
        {
            ParseMethodAttributes();
        }

        var memberParseTree = FindNewContext<JavaParser.MemberDeclarationContext>(parseTree);
        var methodDeclarationParseTree = FindNewContext<JavaParser.MethodDeclarationContext>(memberParseTree);
        Name = FindNewContext<JavaParser.IdentifierContext>(methodDeclarationParseTree)?.GetText();
    }

    private void ParseMethodAttributes()
    {
    }
}