using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Editing;

namespace Ar4eR_ValerA
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(Ar4eR_ValerACodeFixProvider)), Shared]
    public class Ar4eR_ValerACodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(Ar4eR_ValerAAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            
            var declaration = root.FindToken(diagnosticSpan.Start).Parent
                .AncestorsAndSelf()
                .OfType<MethodDeclarationSyntax>().First();
            
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: CodeFixResources.CodeFixTitle,
                    createChangedDocument: c => MakeReturnTypeBool(context.Document, declaration, c),
                    equivalenceKey: nameof(CodeFixResources.CodeFixTitle)),
                diagnostic);
        }

        private async Task<Document> MakeReturnTypeBool(
            Document document,
            MethodDeclarationSyntax methodDeclarationNode,
            CancellationToken cancellationToken)
        {
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken);
            List<ReturnStatementSyntax> returnStatements = GetAllStatements(methodDeclarationNode);
            
            foreach (var returnStatement in returnStatements)
            {
                var oldReturnStatementNode = returnStatement;
                var newReturnStatementNode = oldReturnStatementNode
                    .WithExpression(SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression));

                var newStringOutStatement = SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        SyntaxFactory.IdentifierName("stringOut"),
                        oldReturnStatementNode.Expression));

                editor.InsertBefore(oldReturnStatementNode, newStringOutStatement);
                editor.ReplaceNode(oldReturnStatementNode, newReturnStatementNode);
            }


            var oldReturnTypeNode = (PredefinedTypeSyntax)methodDeclarationNode.ReturnType;
            var newReturnTypeNode = oldReturnTypeNode.WithKeyword(SyntaxFactory.Token(SyntaxKind.BoolKeyword));

            var newParameter = SyntaxFactory
                .Parameter(SyntaxFactory.Identifier("stringOut"))
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.OutKeyword)))
                .WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)));

            editor.InsertParameter(methodDeclarationNode, 0, newParameter);
            editor.ReplaceNode(oldReturnTypeNode, newReturnTypeNode);

            return editor.GetChangedDocument();
        }

        private List<ReturnStatementSyntax> GetAllStatements(
            SyntaxNode syntaxNode,
            List<ReturnStatementSyntax> returnStatementSyntaxes = null)
        {
            if (returnStatementSyntaxes is null)
            {
                returnStatementSyntaxes = new List<ReturnStatementSyntax>();
            }

            if (syntaxNode is ReturnStatementSyntax returnStatementSyntax)
            {
                returnStatementSyntaxes.Add(returnStatementSyntax);
            }

            foreach (var child in syntaxNode.ChildNodes())
            {
                GetAllStatements(child, returnStatementSyntaxes);
            }

            return returnStatementSyntaxes;
        }
    }
}