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

    public string Url { get; private set; }
    public CsMethod CsMethod { get; set; }

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
        bool isQuery;

        if (CsMethod.Args
            .Select(arg => TypeMapper.MapType(arg.Type))
            .All(TypeMapper.IsQuery))
        {
            requestContent = GetQueryRequestContent();
            isQuery = true;
        }
        else
        {
            requestContent = GetBodyRequestContent();
            isQuery = false;
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
                                                    .WithArgumentList(
                                                        ArgumentList(
                                                            SeparatedList<ArgumentSyntax>(
                                                                GetRequestArgs(isQuery)))))))))),
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
                                        InterpolatedStringExpression(
                                                Token(SyntaxKind.InterpolatedStringStartToken))
                                            .WithContents(
                                                List<InterpolatedStringContentSyntax>(
                                                    GetQueryArgs())))))))
        };
    }

    private SyntaxNodeOrToken[] GetRequestArgs(bool isQuery)
    {
        if (CsMethod.RestVerb == "Get")
        {
            return new SyntaxNodeOrToken[]
            {
                Argument(
                    BinaryExpression(
                        SyntaxKind.AddExpression,
                        LiteralExpression(
                            SyntaxKind.StringLiteralExpression,
                            Literal(GetRoute())),
                        IdentifierName("content")))
            };
        }

        if (isQuery)
        {
            return new SyntaxNodeOrToken[]
            {
                Argument(
                    BinaryExpression(
                        SyntaxKind.AddExpression,
                        LiteralExpression(
                            SyntaxKind.StringLiteralExpression,
                            Literal(GetRoute())),
                        IdentifierName("content"))),
                Token(SyntaxKind.CommaToken),
                Argument(
                    ObjectCreationExpression(
                            IdentifierName("StringContent"))
                        .WithArgumentList(
                            ArgumentList(
                                SingletonSeparatedList<ArgumentSyntax>(
                                    Argument(
                                        LiteralExpression(
                                            SyntaxKind.StringLiteralExpression,
                                            Literal("")))))))
            };
        }

        return new SyntaxNodeOrToken[]
        {
            Argument(
                LiteralExpression(
                    SyntaxKind.StringLiteralExpression,
                    Literal(GetRoute()))),
            Token(SyntaxKind.CommaToken),
            Argument(
                IdentifierName("content"))
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

        if (CsMethod.Args.Count > 1)
        {
            throw new Exception("There are more than 1 dto");
        }

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
                                    InvocationExpression(
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                IdentifierName("JsonContent"),
                                                IdentifierName("Create")))
                                        .WithArgumentList(
                                            ArgumentList(
                                                SingletonSeparatedList<ArgumentSyntax>(
                                                    Argument(
                                                        IdentifierName(CsMethod.Args.First().Name)))))))))));
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