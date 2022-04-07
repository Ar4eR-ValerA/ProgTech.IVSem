using Antlr4.Runtime.Tree;

namespace AntlrExample.UsableTreeModels;

public class CsDto
{
    private List<CsField> _args;

    public CsDto(JavaParser.CompilationUnitContext parseTree)
    {
        _args = new List<CsField>();

        ParseTree(parseTree);
    }

    public string Name { get; private set; }
    public IReadOnlyList<CsField> Args => _args;

    private void ParseTree(IParseTree parseTree)
    {
        for (var i = 0; i < parseTree.ChildCount; i++)
        {
            var child = parseTree.GetChild(i);

            switch (child)
            {
                case JavaParser.FieldDeclarationContext fieldDeclarationContext:
                    _args.Add(new CsField(fieldDeclarationContext));
                    break;

                case JavaParser.ClassDeclarationContext classDeclarationContext:
                    ParseClassDeclaration(classDeclarationContext);
                    ParseTree(child);
                    break;

                case JavaParser.TypeDeclarationContext or
                    JavaParser.ClassBodyContext or
                    JavaParser.ClassBodyDeclarationContext or
                    JavaParser.MemberDeclarationContext:
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
}