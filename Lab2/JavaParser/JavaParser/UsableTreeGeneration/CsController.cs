using Antlr4.Runtime.Tree;

namespace AntlrExample.UsableTreeGeneration;

public class CsController
{
    private List<CsMethod> _methods;

    public CsController(IParseTree tree)
    {
        _methods = new List<CsMethod>();
        ParseTree(tree);

        CheckNullAndThrow();
    }

    public string Name { get; private set; }
    public IReadOnlyList<CsMethod> Methods => _methods;

    private void CheckNullAndThrow()
    {
        if (string.IsNullOrEmpty(Name))
        {
            //throw new Exception("Controller's name not found");
        }

        if (_methods.Count == 0)
        {
            //throw new Exception("Controller's methods not found");
        }
    }

    //TODO: Сделать отдельный сервис для парсинга.
    //TODO: Добавить брейков бы
    //TODO: Если что-то из метода не удалось найти, то просто не учитываем его.
    private void ParseTree(IParseTree parseTree)
    {
        for (var i = 0; i < parseTree.ChildCount; i++)
        {
            var child = parseTree.GetChild(i);

            switch (child)
            {
                case JavaParser.ClassDeclarationContext:
                    ParseClassDeclaration(child);
                    ParseTree(child);
                    break;

                case JavaParser.ClassBodyDeclarationContext:
                    ParseMethodDeclaration(child);
                    break;

                case JavaParser.TypeDeclarationContext or JavaParser.ClassBodyContext:
                    ParseTree(child);
                    break;
            }
        }
    }

    private void ParseClassDeclaration(IParseTree parseTree)
    {
        for (var i = 0; i < parseTree.ChildCount; i++)
        {
            var child = parseTree.GetChild(i);

            if (child is JavaParser.IdentifierContext)
            {
                Name = child.GetText();
            }
        }
    }

    private void ParseMethodDeclaration(IParseTree parseTree)
    {
        string methodName = null;

        for (var i = 0; i < parseTree.ChildCount; i++)
        {
            ParseMethodAttributes();
        }

        for (var i = 0; i < parseTree.ChildCount; i++)
        {
            var child = parseTree.GetChild(i);

            if (child is JavaParser.MemberDeclarationContext)
            {
                methodName = ParseMethodName(child);
            }
        }


        var method = new CsMethod(methodName, null, null, null);
        _methods.Add(method);
    }

    private void ParseMethodAttributes()
    {
    }

    private string ParseMethodName(IParseTree parseTree)
    {
        for (var i = 0; i < parseTree.ChildCount; i++)
        {
            var child = parseTree.GetChild(i);

            if (child is JavaParser.MethodDeclarationContext)
            {
                parseTree = child;
            }
        }

        if (parseTree is not JavaParser.MethodDeclarationContext)
        {
            return null;
        }
        
        for (var i = 0; i < parseTree.ChildCount; i++)
        {
            var child = parseTree.GetChild(i);

            if (child is JavaParser.IdentifierContext)
            {
                return child.GetText();
            }
        }

        return null;
    }
}