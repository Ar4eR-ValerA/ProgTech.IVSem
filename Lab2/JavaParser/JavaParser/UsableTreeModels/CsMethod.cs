﻿using Antlr4.Runtime.Tree;

namespace AntlrExample.UsableTreeModels;

public class CsMethod
{
    private List<string> _args;

    public CsMethod(JavaParser.ClassBodyDeclarationContext parseTree)
    {
        IsValid = true;

        ParseMethodDeclaration(parseTree);

        Validate();
    }

    public string Name { get; private set; }
    public string Route { get; private set; }
    public string RestVerb { get; private set; }
    public string ReturningType { get; private set; }
    public bool IsValid { get; set; }

    //TODO: Args.
    //TODO: Нужны сложные аргументы.
    public IReadOnlyList<string> Args => _args;

    private void Validate()
    {
        if (string.IsNullOrEmpty(Name))
        {
            IsValid = false;
        }

        if (string.IsNullOrEmpty(Route))
        {
            IsValid = false;
        }

        if (string.IsNullOrEmpty(RestVerb))
        {
            IsValid = false;
        }

        if (ReturningType is null)
        {
            TODO: IsValid = false;
        }
    }

    private T SteppedDownContext<T>(IParseTree parseTree) where T : IParseTree
    {
        if (parseTree is null)
        {
            return default;
        }

        for (var i = 0; i < parseTree.ChildCount; i++)
        {
            var child = parseTree.GetChild(i);

            if (child is T childT)
            {
                return childT;
            }
        }

        return default;
    }

    private IReadOnlyList<T> SteppedDownContexts<T>(IParseTree parseTree) where T : IParseTree
    {
        var contexts = new List<T>();

        if (parseTree is null)
        {
            return contexts;
        }

        for (var i = 0; i < parseTree.ChildCount; i++)
        {
            var child = parseTree.GetChild(i);

            if (child is T childT)
            {
                contexts.Add(childT);
            }
        }

        return contexts;
    }

    private T MultiSteppedDownContext<T>(IParseTree parseTree) where T : IParseTree
    {
        if (parseTree is null)
        {
            return default;
        }
        
        for (var i = 0; i < parseTree.ChildCount; i++)
        {
            var child = parseTree.GetChild(i);

            if (child is T childT)
            {
                return childT;
            }

            var steppedDownContext = MultiSteppedDownContext<T>(child);
            if (steppedDownContext is T steppedDown)
            {
                return steppedDown;
            }
        }

        return default;
    }

    private void ParseMethodDeclaration(JavaParser.ClassBodyDeclarationContext parseTree)
    {
        for (var i = 0; i < parseTree.ChildCount; i++)
        {
            var modifiers = SteppedDownContexts<JavaParser.ModifierContext>(parseTree);
            ParseMethodAttributes(modifiers);
        }

        var memberDeclaration = SteppedDownContext<JavaParser.MemberDeclarationContext>(parseTree);
        var methodDeclaration = SteppedDownContext<JavaParser.MethodDeclarationContext>(memberDeclaration);
        Name = SteppedDownContext<JavaParser.IdentifierContext>(methodDeclaration)?.GetText();

        if (Name is null)
        {
            return;
        }

        var typeTypeOrVoid = SteppedDownContext<JavaParser.TypeTypeOrVoidContext>(methodDeclaration);
        var typeType = SteppedDownContext<JavaParser.TypeTypeContext>(typeTypeOrVoid);

        if (typeType is null)
        {
            return;
        }
        
        var someType = typeType.GetChild(0);

        ReturningType = someType switch
        {
            JavaParser.PrimitiveTypeContext primitiveType => ParseType(primitiveType),
            JavaParser.ClassOrInterfaceTypeContext classOrInterfaceType => ParseType(classOrInterfaceType),
            _ => ReturningType
        };
    }

    private void ParseMethodAttributes(IReadOnlyList<JavaParser.ModifierContext> parseTrees)
    {
        foreach (var parseTree in parseTrees)
        {
            var classModifier = SteppedDownContext<JavaParser.ClassOrInterfaceModifierContext>(parseTree);
            var annotation = SteppedDownContext<JavaParser.AnnotationContext>(classModifier);
            var qualifiedName = SteppedDownContext<JavaParser.QualifiedNameContext>(annotation);
            var identifier = SteppedDownContext<JavaParser.IdentifierContext>(qualifiedName);
            var attributeName = identifier?.GetText();

            if (string.IsNullOrEmpty(attributeName))
            {
                continue;
            }

            var elementValue = SteppedDownContext<JavaParser.ElementValueContext>(annotation);
            var expression = SteppedDownContext<JavaParser.ExpressionContext>(elementValue);
            var primary = SteppedDownContext<JavaParser.PrimaryContext>(expression);
            var literal = SteppedDownContext<JavaParser.LiteralContext>(primary);
            var attributeArg = literal?.GetText();

            if (string.IsNullOrEmpty(attributeArg))
            {
                continue;
            }

            RestVerb = attributeName.Remove(attributeName.Length - 7, 7);
            Route = attributeArg.Trim('"');
            break;
        }
    }

    private string ParseType(JavaParser.PrimitiveTypeContext parseTree)
    {
        return parseTree.GetText();
    }

    private string ParseType(JavaParser.ClassOrInterfaceTypeContext parseTree)
    {
        var name = SteppedDownContext<JavaParser.IdentifierContext>(parseTree).GetText();

        var typeArguments = SteppedDownContext<JavaParser.TypeArgumentsContext>(parseTree);
        var typeType = MultiSteppedDownContext<JavaParser.TypeTypeContext>(typeArguments);

        if (typeType is null)
        {
            return name;
        }
        
        var someType = typeType.GetChild(0);

        return someType switch
        {
            JavaParser.PrimitiveTypeContext primitiveType => $"{name}<{ParseType(primitiveType)}>",
            JavaParser.ClassOrInterfaceTypeContext classOrInterfaceType => $"{name}<{ParseType(classOrInterfaceType)}>",
            _ => null
        };
    }
}