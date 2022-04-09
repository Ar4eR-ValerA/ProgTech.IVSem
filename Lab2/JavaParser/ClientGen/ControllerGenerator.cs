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
            CompilationUnit().WithUsings(
                SingletonList<UsingDirectiveSyntax>(
                    UsingDirective(
                        QualifiedName(
                            QualifiedName(
                                IdentifierName("System"),
                                IdentifierName("Text")),
                            IdentifierName("Json"))))).WithMembers(SingletonList<MemberDeclarationSyntax>(
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
                                            GetHttpClientField(),
                                            new MethodGenerator(CsController.Methods[0], Url).Generate(),
                                            new MethodGenerator(CsController.Methods[1], Url).Generate()
                                        }))))))
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
}