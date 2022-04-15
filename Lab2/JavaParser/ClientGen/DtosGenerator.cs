using AntlrExample.UsableTreeModels;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ClientGen;

public class DtosGenerator
{
    private List<CsDto> _csDtos;

    public DtosGenerator(List<CsDto> csDtos)
    {
        _csDtos = csDtos;
    }

    public IReadOnlyList<CsDto> CsDtos => _csDtos;

    public void Generate(string path)
    {
        foreach (var dto in CsDtos)
        {
            var compilationUnitSyntax = CompilationUnit()
                .WithMembers(
                    SingletonList<MemberDeclarationSyntax>(
                        FileScopedNamespaceDeclaration(
                                IdentifierName("Client"))
                            .WithMembers(
                                SingletonList<MemberDeclarationSyntax>(
                                    ClassDeclaration(dto.Name)
                                        .WithModifiers(
                                            TokenList(
                                                Token(SyntaxKind.PublicKeyword)))
                                        .WithMembers(
                                            List<MemberDeclarationSyntax>(
                                                new MemberDeclarationSyntax[]
                                                {
                                                    ConstructorDeclaration(
                                                            Identifier(dto.Name))
                                                        .WithModifiers(
                                                            TokenList(
                                                                Token(SyntaxKind.PublicKeyword)))
                                                        .WithParameterList(
                                                            ParameterList(
                                                                SeparatedList<ParameterSyntax>(
                                                                    GetConstructorArgs(dto))))
                                                        .WithBody(
                                                            Block(
                                                                GetConstructorExpressions(dto))),
                                                }).AddRange(GetProperties(dto)))))))
                .NormalizeWhitespace();

            compilationUnitSyntax.NormalizeWhitespace();

            File.WriteAllText(@$"{path}\{dto.Name}.cs", compilationUnitSyntax.ToString());
        }
    }

    private SyntaxNodeOrToken[] GetConstructorArgs(CsDto dto)
    {
        var syntaxNodeOrTokens = new List<SyntaxNodeOrToken>();

        var arg = dto.Args.First();
        syntaxNodeOrTokens.Add(Parameter(Identifier(arg.Name))
            .WithType(IdentifierName(TypeMapper.MapType(arg.Type))));

        for (var i = 1; i < dto.Args.Count; i++)
        {
            syntaxNodeOrTokens.Add(Token(SyntaxKind.CommaToken));

            arg = dto.Args[i];
            syntaxNodeOrTokens.Add(Parameter(Identifier(arg.Name))
                .WithType(IdentifierName(TypeMapper.MapType(arg.Type))));
        }

        return syntaxNodeOrTokens.ToArray();
    }

    private StatementSyntax[] GetConstructorExpressions(CsDto dto)
    {
        var statementSyntaxes = new List<StatementSyntax>();

        foreach (var arg in dto.Args)
        {
            statementSyntaxes.Add(ExpressionStatement(
                AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        ThisExpression(),
                        IdentifierName(arg.Name)),
                    IdentifierName(arg.Name))));
        }

        return statementSyntaxes.ToArray();
    }

    private List<MemberDeclarationSyntax> GetProperties(CsDto dto)
    {
        var memberDeclarationSyntaxes = new List<MemberDeclarationSyntax>();

        foreach (var arg in dto.Args)
        {
            memberDeclarationSyntaxes.Add(PropertyDeclaration(
                    IdentifierName(TypeMapper.MapType(arg.Type)),
                    Identifier(arg.Name))
                .WithModifiers(
                    TokenList(
                        Token(SyntaxKind.PublicKeyword)))
                .WithAccessorList(
                    AccessorList(
                        List<AccessorDeclarationSyntax>(
                            new AccessorDeclarationSyntax[]
                            {
                                AccessorDeclaration(
                                        SyntaxKind.GetAccessorDeclaration)
                                    .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken)),
                                AccessorDeclaration(
                                        SyntaxKind.SetAccessorDeclaration)
                                    .WithSemicolonToken(
                                        Token(SyntaxKind.SemicolonToken))
                            }))));
        }

        return memberDeclarationSyntaxes;
    }
}