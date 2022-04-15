using AntlrExample.UsableTreeModels;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ClientGen;

public class ControllerGenerator
{
    public ControllerGenerator(CsController csController, string url)
    {
        CsController = csController;
        Url = url;
        DtosGenerator = new DtosGenerator(csController.Dtos.ToList());
    }

    public CsController CsController { get; private set; }
    public string Url { get; set; }
    public DtosGenerator DtosGenerator { get; private set; }

    public void Generate(string path)
    {
        DtosGenerator.Generate(path);

        var compilationUnitSyntax =
            CompilationUnit()
                .WithUsings(
                    List<UsingDirectiveSyntax>(
                        new UsingDirectiveSyntax[]
                        {
                            UsingDirective(
                                QualifiedName(
                                    QualifiedName(
                                        QualifiedName(
                                            IdentifierName("System"),
                                            IdentifierName("Net")),
                                        IdentifierName("Http")),
                                    IdentifierName("Json"))),
                            UsingDirective(
                                QualifiedName(
                                    QualifiedName(
                                        IdentifierName("System"),
                                        IdentifierName("Text")),
                                    IdentifierName("Json")))
                        }))
                .WithMembers(SingletonList<MemberDeclarationSyntax>(
                    FileScopedNamespaceDeclaration(
                            IdentifierName("Client"))
                        .WithMembers(
                            SingletonList<MemberDeclarationSyntax>(
                                ClassDeclaration(CsController.Name)
                                    .WithModifiers(
                                        TokenList(
                                            Token(SyntaxKind.PublicKeyword)))
                                    .WithMembers(
                                        List<MemberDeclarationSyntax>(
                                            new MemberDeclarationSyntax[]
                                            {
                                                GetHttpClientField()
                                            }).AddRange(GetMethods()))))))
                .NormalizeWhitespace();

        File.WriteAllText(@$"{path}\{CsController.Name}Client.cs", compilationUnitSyntax.ToString());
    }

    private MemberDeclarationSyntax GetHttpClientField()
    {
        return FieldDeclaration(
            VariableDeclaration(
                    IdentifierName("HttpClient"))
                .WithVariables(
                    SingletonSeparatedList<VariableDeclaratorSyntax>(
                        VariableDeclarator(
                                Identifier("_client"))
                            .WithInitializer(
                                EqualsValueClause(
                                    ObjectCreationExpression(
                                            IdentifierName("HttpClient"))
                                        .WithArgumentList(
                                            ArgumentList())))))).WithModifiers(
            TokenList(
                new[]
                {
                    Token(SyntaxKind.PrivateKeyword),
                    Token(SyntaxKind.StaticKeyword)
                }));
    }

    private List<MemberDeclarationSyntax> GetMethods()
    {
        return CsController.Methods
            .Select(method => new MethodGenerator(method, Url))
            .Select(methodGenerator => methodGenerator.Generate())
            .ToList();
    }
}