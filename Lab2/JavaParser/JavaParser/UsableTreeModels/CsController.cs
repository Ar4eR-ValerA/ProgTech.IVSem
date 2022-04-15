using Antlr4.Runtime.Tree;

namespace AntlrExample.UsableTreeModels;

public class CsController
{
    private List<CsMethod> _methods;
    private List<CsDto> _dtos;

    public CsController(
        JavaParser.CompilationUnitContext controllerParseTree,
        List<JavaParser.CompilationUnitContext> dtoParseTrees)
    {
        _methods = new List<CsMethod>();
        ParseTree(controllerParseTree);

        _dtos = new List<CsDto>();
        foreach (var dtoParseTree in dtoParseTrees)
        {
            _dtos.Add(new CsDto(dtoParseTree));
        }

        ValidateAndThrow();
    }

    public string Name { get; private set; }
    public IReadOnlyList<CsMethod> Methods => _methods;
    public IReadOnlyCollection<CsDto> Dtos => _dtos;

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
        var child = Tools.SteppedDownContext<JavaParser.IdentifierContext>(parseTree);
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