using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using AntlrExample.ParsingTools;
using AntlrExample.UsableTreeGeneration;

var text = File.ReadAllText(@$"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\JavaMvcApplication.java");
var stream = CharStreams.fromString(text);
var lexer = new JavaLexer(stream);
var tokens = new CommonTokenStream(lexer);
var parser = new JavaParser(tokens);

var listener = new MyJavaParserListener();
var walker = new ParseTreeWalker();

walker.Walk(listener, parser.compilationUnit());

JavaParser.CompilationUnitContext tree = listener.Tree;
var csController = new CsController(tree);

Console.WriteLine();