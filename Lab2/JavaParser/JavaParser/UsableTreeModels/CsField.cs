using Antlr4.Runtime.Tree;

namespace AntlrExample.UsableTreeModels;

public class CsField
{
    public string Name { get; private set; }
    public string Type { get; private set; }

    public CsField(JavaParser.FieldDeclarationContext parseTree)
    {
        Name = Tools.SteppedDownContext<JavaParser.VariableDeclaratorsContext>(parseTree).GetText();
        Type = Tools.SteppedDownContext<JavaParser.TypeTypeContext>(parseTree).GetText();
    }
}