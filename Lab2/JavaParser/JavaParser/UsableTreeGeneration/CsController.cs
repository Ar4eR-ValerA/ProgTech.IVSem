using Antlr4.Runtime.Tree;

namespace AntlrExample.UsableTreeGeneration;

public class CsController
{
    private List<CsMethod> _methods;

    public CsController(JavaParser.CompilationUnitContext parseTree)
    {
        _methods = new List<CsMethod>();
        ParseTree(parseTree);

        ValidateAndThrow();
    }

    public string Name { get; private set; }
    public IReadOnlyList<CsMethod> Methods => _methods;

    private void ValidateAndThrow()
    {
        if (string.IsNullOrEmpty(Name))
        {
            throw new Exception("Controller's name not found");
        }

        if (_methods.Count == 0)
        {
            throw new Exception("Controller's methods not found");
        }
    }

    private T FindNewContext<T>(IParseTree parseTree) where T : IParseTree
    {
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
    
    private void ParseTree(IParseTree parseTree)
    {
        for (var i = 0; i < parseTree.ChildCount; i++)
        {
            var child = parseTree.GetChild(i);

            switch (child)
            {
                case JavaParser.ClassDeclarationContext classDeclarationContext:
                    ParseClassDeclaration(classDeclarationContext);
                    ParseTree(child);
                    break;

                case JavaParser.ClassBodyDeclarationContext classBodyDeclarationContext:
                    AddMethod(classBodyDeclarationContext);
                    break;

                case JavaParser.TypeDeclarationContext or JavaParser.ClassBodyContext:
                    ParseTree(child);
                    break;
            }
        }
    }

    private void ParseClassDeclaration(JavaParser.ClassDeclarationContext parseTree)
    {
        var child = FindNewContext<JavaParser.IdentifierContext>(parseTree);
        Name = child?.GetText();
    }

    private void AddMethod(JavaParser.ClassBodyDeclarationContext parseTree)
    {
        var method = new CsMethod(parseTree);

        if (method.IsValid)
        {
            _methods.Add(method);
        }
    }
}