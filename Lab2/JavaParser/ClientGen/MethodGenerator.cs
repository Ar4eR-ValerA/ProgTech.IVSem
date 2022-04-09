using AntlrExample.UsableTreeModels;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace ClientGen;

public class MethodGenerator
{
    public MethodGenerator(CsMethod csMethod, string url)
    {
        Url = url;
        CsMethod = csMethod;
    }

    public string Url { get; set; }

    public MemberDeclarationSyntax Generate()
    {
        return MethodDeclaration(
                IdentifierName(GetReturnType()),
                Identifier(GetName()))
            .WithModifiers(
                TokenList(
                    new[]
                    {
                        Token(SyntaxKind.PublicKeyword),
                        Token(SyntaxKind.AsyncKeyword)
                    }))
            .WithParameterList(
                ParameterList(
                    SeparatedList<ParameterSyntax>(
                        GetArgs())))
            .WithBody(
                Block(GetMethodBody()));
    }

    public CsMethod CsMethod { get; private set; }

    private string GetReturnType()
    {
        if (CsMethod.ReturnType == "void")
        {
            return "Task";
        }

        return $"Task<{TypeMapper.MapType(CsMethod.ReturnType)}>";
    }

    private string GetName()
    {
        return CsMethod.Name;
    }

    private BlockSyntax GetMethodBody()
    {
        StatementSyntax[] requestContent;

        if (CsMethod.Args
            .Select(arg => Type.GetType(TypeMapper.MapType(arg.Type)))
            .All(type => type is not null && (type.IsPrimitive || type == typeof(string))))
        {
            requestContent = GetQueryRequestContent();
        }
        else
        {
            requestContent = GetBodyRequestContent();
        }

        var block = Block()
            .AddStatements(requestContent)
            .AddStatements(new StatementSyntax[]
            {
                LocalDeclarationStatement(
                    VariableDeclaration(
                            IdentifierName(
                                Identifier(
                                    TriviaList(),
                                    SyntaxKind.VarKeyword,
                                    "var",
                                    "var",
                                    TriviaList())))
                        .WithVariables(
                            SingletonSeparatedList<VariableDeclaratorSyntax>(
                                VariableDeclarator(
                                        Identifier("response"))
                                    .WithInitializer(
                                        EqualsValueClause(
                                            AwaitExpression(
                                                InvocationExpression(
                                                        MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            IdentifierName("_client"),
                                                            IdentifierName(GetRestMethod())))
                                                    //TODO: Если Get, то без контента.
                                                    //TODO: Починить работу с дтошками.
                                                    .WithArgumentList(
                                                        ArgumentList(
                                                            SeparatedList<ArgumentSyntax>(
                                                                new SyntaxNodeOrToken[]
                                                                {
                                                                    Argument(
                                                                        LiteralExpression(
                                                                            SyntaxKind.StringLiteralExpression,
                                                                            Literal(GetRoute()))),
                                                                    Token(SyntaxKind.CommaToken),
                                                                    Argument(
                                                                        IdentifierName("content"))
                                                                }))))))))),
                LocalDeclarationStatement(
                    VariableDeclaration(
                            IdentifierName(
                                Identifier(
                                    TriviaList(),
                                    SyntaxKind.VarKeyword,
                                    "var",
                                    "var",
                                    TriviaList())))
                        .WithVariables(
                            SingletonSeparatedList<VariableDeclaratorSyntax>(
                                VariableDeclarator(
                                        Identifier("responseString"))
                                    .WithInitializer(
                                        EqualsValueClause(
                                            AwaitExpression(
                                                InvocationExpression(
                                                    MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            IdentifierName("response"),
                                                            IdentifierName("Content")),
                                                        IdentifierName("ReadAsStringAsync")))))))))
            })
            .AddStatements(GetReturnStatment());

        return block;
    }

    private StatementSyntax[] GetQueryRequestContent()
    {
        return new StatementSyntax[]
        {
            LocalDeclarationStatement(
                VariableDeclaration(
                        IdentifierName(
                            Identifier(
                                TriviaList(),
                                SyntaxKind.VarKeyword,
                                "var",
                                "var",
                                TriviaList())))
                    .WithVariables(
                        SingletonSeparatedList<VariableDeclaratorSyntax>(
                            VariableDeclarator(
                                    Identifier("content"))
                                .WithInitializer(
                                    EqualsValueClause(
                                        ObjectCreationExpression(
                                                IdentifierName("StringContent"))
                                            .WithArgumentList(
                                                ArgumentList(
                                                    SingletonSeparatedList<ArgumentSyntax>(
                                                        Argument(
                                                            InterpolatedStringExpression(
                                                                    Token(SyntaxKind
                                                                        .InterpolatedStringStartToken))
                                                                .WithContents(
                                                                    List<InterpolatedStringContentSyntax>(
                                                                        GetQueryArgs())))))))))))
        };
    }

    private List<InterpolatedStringContentSyntax> GetQueryArgs()
    {
        var args = new List<InterpolatedStringContentSyntax>();

        if (CsMethod.Args.Count == 0)
        {
            return args;
        }

        args.Add(InterpolatedStringText()
            .WithTextToken(
                Token(
                    TriviaList(),
                    SyntaxKind.InterpolatedStringTextToken,
                    $"?{CsMethod.Args.First().Name}=",
                    $"?{CsMethod.Args.First().Name}=",
                    TriviaList())));
        args.Add(Interpolation(IdentifierName($"{CsMethod.Args.First().Name}")));

        foreach (var arg in CsMethod.Args)
        {
            if (arg == CsMethod.Args.First())
            {
                continue;
            }

            args.Add(InterpolatedStringText()
                .WithTextToken(
                    Token(
                        TriviaList(),
                        SyntaxKind.InterpolatedStringTextToken,
                        $"&{arg.Name}=",
                        $"&{arg.Name}=",
                        TriviaList())));
            args.Add(Interpolation(IdentifierName($"{arg.Name}")));
        }

        return args;
    }

    private StatementSyntax[] GetBodyRequestContent()
    {
        var statementSyntaxes = new List<StatementSyntax>();

        statementSyntaxes.Add(LocalDeclarationStatement(
            VariableDeclaration(
                    IdentifierName(
                        Identifier(
                            TriviaList(),
                            SyntaxKind.VarKeyword,
                            "var",
                            "var",
                            TriviaList())))
                .WithVariables(
                    SingletonSeparatedList<VariableDeclaratorSyntax>(
                        VariableDeclarator(
                                Identifier("content"))
                            .WithInitializer(
                                EqualsValueClause(
                                    ObjectCreationExpression(
                                            IdentifierName("MultipartFormDataContent"))
                                        .WithArgumentList(
                                            ArgumentList())))))));

        foreach (var arg in CsMethod.Args)
        {
            statementSyntaxes.Add(ExpressionStatement(
                InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("content"),
                            IdentifierName("Add")))
                    .WithArgumentList(
                        ArgumentList(
                            SeparatedList<ArgumentSyntax>(
                                new SyntaxNodeOrToken[]
                                {
                                    Argument(
                                        ObjectCreationExpression(
                                                IdentifierName("StringContent"))
                                            .WithArgumentList(
                                                ArgumentList(
                                                    SingletonSeparatedList<ArgumentSyntax>(
                                                        Argument(
                                                            MemberAccessExpression(
                                                                SyntaxKind.SimpleMemberAccessExpression,
                                                                IdentifierName(arg.Name),
                                                                IdentifierName(arg.Name))))))),
                                    Token(SyntaxKind.CommaToken),
                                    Argument(
                                        LiteralExpression(
                                            SyntaxKind.StringLiteralExpression,
                                            Literal(arg.Name)))
                                })))));
        }

        return statementSyntaxes.ToArray();
    }

    private SyntaxNodeOrToken[] GetArgs()
    {
        var syntaxNodeOrTokens = new List<SyntaxNodeOrToken>();

        if (CsMethod.Args.Count == 0)
        {
            return syntaxNodeOrTokens.ToArray();
        }

        var arg = CsMethod.Args.First();
        syntaxNodeOrTokens.Add(Parameter(Identifier(arg.Name))
            .WithType(IdentifierName(TypeMapper.MapType(arg.Type))));

        for (var i = 1; i < CsMethod.Args.Count; i++)
        {
            syntaxNodeOrTokens.Add(Token(SyntaxKind.CommaToken));

            arg = CsMethod.Args[i];
            syntaxNodeOrTokens.Add(Parameter(Identifier(arg.Name))
                .WithType(IdentifierName(TypeMapper.MapType(arg.Type))));
        }

        return syntaxNodeOrTokens.ToArray();
    }

    private string GetRestMethod()
    {
        return $"{CsMethod.RestVerb}Async";
    }

    private string GetRoute()
    {
        return $"{Url}{CsMethod.Route}";
    }

    private StatementSyntax[] GetReturnStatment()
    {
        var statementSyntaxes = new List<StatementSyntax>();

        if (CsMethod.ReturnType != "void")
        {
            statementSyntaxes.Add(ReturnStatement(
                InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("JsonSerializer"),
                            GenericName(
                                    Identifier("Deserialize"))
                                .WithTypeArgumentList(
                                    TypeArgumentList(
                                        SingletonSeparatedList<TypeSyntax>(
                                            IdentifierName(TypeMapper.MapType(CsMethod.ReturnType)))))))
                    .WithArgumentList(
                        ArgumentList(
                            SingletonSeparatedList<ArgumentSyntax>(
                                Argument(
                                    IdentifierName("responseString")))))));
        }

        return statementSyntaxes.ToArray();
    }
}