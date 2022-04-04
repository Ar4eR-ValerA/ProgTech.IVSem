using Antlr4.Runtime.Tree;

namespace AntlrExample;

public class MyJavaParserListener : JavaParserBaseListener
{
    public IParseTree Tree { get; set; }

    public override void EnterCompilationUnit(JavaParser.CompilationUnitContext context)
    {
        Tree = context;
    }
}