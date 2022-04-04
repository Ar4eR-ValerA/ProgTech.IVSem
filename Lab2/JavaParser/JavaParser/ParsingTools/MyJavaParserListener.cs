namespace AntlrExample.ParsingTools;

public class MyJavaParserListener : JavaParserBaseListener
{
    public JavaParser.CompilationUnitContext Tree { get; set; }

    public override void EnterCompilationUnit(JavaParser.CompilationUnitContext context)
    {
        Tree = context;
    }
}