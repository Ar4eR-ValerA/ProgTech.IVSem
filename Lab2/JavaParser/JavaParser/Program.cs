using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using AntlrExample.ParsingTools;
using AntlrExample.UsableTreeModels;

var text = File.ReadAllText(
    @$"D:\Projects\Tech.Ar4eR-ValerA\Lab2\JavaMVC\src\main\java\com\example\javamvc\controllers\JavaMvcApplication.java");
var stream = CharStreams.fromString(text);
var lexer = new JavaLexer(stream);
var tokens = new CommonTokenStream(lexer);
var parser = new JavaParser(tokens);

var listener = new MyJavaParserListener();
var walker = new ParseTreeWalker();

walker.Walk(listener, parser.compilationUnit());

JavaParser.CompilationUnitContext controllerTree = listener.Tree;
var dtoTrees = new List<JavaParser.CompilationUnitContext>();
var graphVisualizer = new GraphVisualizer();
foreach (var file in
         Directory.GetFiles(@$"D:\Projects\Tech.Ar4eR-ValerA\Lab2\JavaMVC\src\main\java\com\example\javamvc\dtos"))
{
    text = File.ReadAllText(file);
    stream = CharStreams.fromString(text);
    lexer = new JavaLexer(stream);
    tokens = new CommonTokenStream(lexer);
    parser = new JavaParser(tokens);

    listener = new MyJavaParserListener();
    walker = new ParseTreeWalker();

    walker.Walk(listener, parser.compilationUnit());

    dtoTrees.Add(listener.Tree);
    graphVisualizer.Visualize(
        listener.Tree,
        @$"D:\Projects\Tech.Ar4eR-ValerA\Lab2\JavaParser\{file.Split("\\").Last()}.txt");
}

var csController = new CsController(controllerTree, dtoTrees);

Console.WriteLine();

graphVisualizer.Visualize(controllerTree, @$"D:\Projects\Tech.Ar4eR-ValerA\Lab2\JavaParser\Tree.txt");