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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(Ar4eR_ValerA_O5_CodeFixProvider)), Shared]
    public class Ar4eR_ValerA_O5_CodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(Ar4eR_ValerA_O5_Analyzer.DiagnosticId); }
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
                    title: CodeFixResources.O5_CodeFixTitle,
                    createChangedDocument: c => MakeReturnTypeBool(context.Document, declaration, c),
                    equivalenceKey: nameof(CodeFixResources.O5_CodeFixTitle)),
                diagnostic);
        }

        private async Task<Document> MakeReturnTypeBool(
            Document document,
            MethodDeclarationSyntax methodDeclarationNode,
            CancellationToken cancellationToken)
        {
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken);
            var oldReturnTypeNode = (PredefinedTypeSyntax) methodDeclarationNode.ReturnType;
            var returnStatements = methodDeclarationNode
                .DescendantNodesAndSelf()
                .Where(node => node is ReturnStatementSyntax)
                .ToList();

            foreach (var returnStatement in returnStatements)
            {
                var oldReturnStatementNode = (ReturnStatementSyntax) returnStatement;
                var newReturnStatementNode = oldReturnStatementNode
                    .WithExpression(SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression));

                var newStringOutStatement = SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.AssignmentExpression(
                        SyntaxKind.SimpleAssignmentExpression,
                        SyntaxFactory.IdentifierName($"{oldReturnTypeNode.Keyword}Out"),
                        oldReturnStatementNode.Expression));

                editor.InsertBefore(oldReturnStatementNode, newStringOutStatement);
                editor.ReplaceNode(oldReturnStatementNode, newReturnStatementNode);
            }

            if (returnStatements.Count == 0)
            {
                editor.InsertAfter(
                    methodDeclarationNode.Body.Statements.Last(),
                    SyntaxFactory.ReturnStatement()
                        .WithExpression(
                            SyntaxFactory.LiteralExpression(
                                SyntaxKind.TrueLiteralExpression))
                );
            }

            var newReturnTypeNode = oldReturnTypeNode.WithKeyword(SyntaxFactory.Token(SyntaxKind.BoolKeyword));
            editor.ReplaceNode(oldReturnTypeNode, newReturnTypeNode);

            if (oldReturnTypeNode.Keyword.ToString() == SyntaxFactory.Token(SyntaxKind.VoidKeyword).ToString())
            {
                return editor.GetChangedDocument();
            }

            var newParameter = SyntaxFactory
                .Parameter(SyntaxFactory.Identifier($"{oldReturnTypeNode.Keyword}Out"))
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.OutKeyword)))
                .WithType(SyntaxFactory.PredefinedType(oldReturnTypeNode.Keyword));
            editor.InsertParameter(methodDeclarationNode, 0, newParameter);

            return editor.GetChangedDocument();
        }
    }
}