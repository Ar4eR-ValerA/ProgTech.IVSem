using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace AntlrExample.UsableTreeModels;

public class CsMethod
{
    private List<Pair<string, string>> _args;

    public CsMethod(JavaParser.ClassBodyDeclarationContext parseTree)
    {
        IsValid = true;
        _args = new List<Pair<string, string>>();

        ParseDeclaration(parseTree);

        Validate();
    }

    public string Name { get; private set; }
    public string Route { get; private set; }
    public string RestVerb { get; private set; }
    public string ReturningType { get; private set; }
    public bool IsValid { get; set; }

    public IReadOnlyList<Pair<string, string>> Args => _args;

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
            IsValid = false;
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

    private void ParseDeclaration(JavaParser.ClassBodyDeclarationContext parseTree)
    {
        var modifiers = SteppedDownContexts<JavaParser.ModifierContext>(parseTree);
        ParseAttributes(modifiers);

        var memberDeclaration = SteppedDownContext<JavaParser.MemberDeclarationContext>(parseTree);
        var methodDeclaration = SteppedDownContext<JavaParser.MethodDeclarationContext>(memberDeclaration);

        ParseName(methodDeclaration);
        if (Name is null)
        {
            return;
        }

        ParseReturningType(methodDeclaration);
        if (ReturningType is null)
        {
            return;
        }

        ParseArguments(methodDeclaration);
    }

    private void ParseAttributes(IReadOnlyList<JavaParser.ModifierContext> parseTrees)
    {
        foreach (var parseTree in parseTrees)
        {
            var classModifier = SteppedDownContext<JavaParser.ClassOrInterfaceModifierContext>(parseTree);
            var annotation = SteppedDownContext<JavaParser.AnnotationContext>(classModifier);
            var qualifiedName = SteppedDownContext<JavaParser.QualifiedNameContext>(annotation);
            var attributeName = qualifiedName?.GetText();

            if (string.IsNullOrEmpty(attributeName))
            {
                continue;
            }

            var elementValue = SteppedDownContext<JavaParser.ElementValueContext>(annotation);
            var attributeArg = elementValue?.GetText();

            if (string.IsNullOrEmpty(attributeArg))
            {
                continue;
            }

            RestVerb = attributeName.Remove(attributeName.Length - 7, 7);
            Route = attributeArg.Trim('"');
            break;
        }
    }

    private void ParseName(JavaParser.MethodDeclarationContext parseTree)
    {
        Name = SteppedDownContext<JavaParser.IdentifierContext>(parseTree)?.GetText();
    }

    private void ParseReturningType(JavaParser.MethodDeclarationContext parseTree)
    {
        var typeTypeOrVoid = SteppedDownContext<JavaParser.TypeTypeOrVoidContext>(parseTree);
        var typeType = SteppedDownContext<JavaParser.TypeTypeContext>(typeTypeOrVoid);
        ReturningType = typeType?.GetText();
    }

    private void ParseArguments(JavaParser.MethodDeclarationContext parseTree)
    {
        var formalParameters = SteppedDownContext<JavaParser.FormalParametersContext>(parseTree);
        var formalParameterList = SteppedDownContext<JavaParser.FormalParameterListContext>(formalParameters);

        if (formalParameterList is not null)
        {
            foreach (var formalParameter in formalParameterList.children)
            {
                var type = SteppedDownContext<JavaParser.TypeTypeContext>(formalParameter).GetText();
                var name = SteppedDownContext<JavaParser.VariableDeclaratorIdContext>(formalParameter).GetText();

                _args.Add(new Pair<string, string>(type, name));
            }
        }
    }
}