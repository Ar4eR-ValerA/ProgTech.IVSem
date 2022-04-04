using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using AntlrExample.ParsingTools;
using AntlrExample.UsableTreeGeneration;

var text = File.ReadAllText(
    @$"D:\Projects\Tech.Ar4eR-ValerA\Lab2\JavaMVC\src\main\java\com\example\javamvc\controllers\JavaMvcApplication.java");
var stream = CharStreams.fromString(text);
var lexer = new JavaLexer(stream);
var tokens = new CommonTokenStream(lexer);
var parser = new JavaParser(tokens);

var listener = new MyJavaParserListener();
var walker = new ParseTreeWalker();

walker.Walk(listener, parser.compilationUnit());

JavaParser.CompilationUnitContext tree = listener.Tree;
var csController = new CsController(tree);

GraphVisualizer graphVisualizer = new GraphVisualizer();
graphVisualizer.Visualize(tree, @$"D:\Projects\Tech.Ar4eR-ValerA\Lab2\JavaParser\Tree.txt");