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

    private void ParseDeclaration(JavaParser.ClassBodyDeclarationContext parseTree)
    {
        var modifiers = Tools.SteppedDownContexts<JavaParser.ModifierContext>(parseTree);
        ParseAttributes(modifiers);

        var memberDeclaration = Tools.SteppedDownContext<JavaParser.MemberDeclarationContext>(parseTree);
        var methodDeclaration = Tools.SteppedDownContext<JavaParser.MethodDeclarationContext>(memberDeclaration);

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
            var classModifier = Tools.SteppedDownContext<JavaParser.ClassOrInterfaceModifierContext>(parseTree);
            var annotation = Tools.SteppedDownContext<JavaParser.AnnotationContext>(classModifier);
            var qualifiedName = Tools.SteppedDownContext<JavaParser.QualifiedNameContext>(annotation);
            var attributeName = qualifiedName?.GetText();

            if (string.IsNullOrEmpty(attributeName))
            {
                continue;
            }

            var elementValue = Tools.SteppedDownContext<JavaParser.ElementValueContext>(annotation);
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
        Name = Tools.SteppedDownContext<JavaParser.IdentifierContext>(parseTree)?.GetText();
    }

    private void ParseReturningType(JavaParser.MethodDeclarationContext parseTree)
    {
        var typeTypeOrVoid = Tools.SteppedDownContext<JavaParser.TypeTypeOrVoidContext>(parseTree);
        var typeType = Tools.SteppedDownContext<JavaParser.TypeTypeContext>(typeTypeOrVoid);
        ReturningType = typeType?.GetText();
    }

    private void ParseArguments(JavaParser.MethodDeclarationContext parseTree)
    {
        var formalParameters = Tools.SteppedDownContext<JavaParser.FormalParametersContext>(parseTree);
        var formalParameterList = Tools.SteppedDownContext<JavaParser.FormalParameterListContext>(formalParameters);

        if (formalParameterList is not null)
        {
            foreach (var formalParameter in formalParameterList.children)
            {
                var type = Tools.SteppedDownContext<JavaParser.TypeTypeContext>(formalParameter).GetText();
                var name = Tools.SteppedDownContext<JavaParser.VariableDeclaratorIdContext>(formalParameter).GetText();

                _args.Add(new Pair<string, string>(type, name));
            }
        }
    }
}