using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Runtime.Serialization;
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
            // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var declaration = root.FindToken(diagnosticSpan.Start).Parent
                .AncestorsAndSelf()
                .OfType<MethodDeclarationSyntax>().First();

            // Register a code action that will invoke the fix.
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

            var oldReturnStatementNode = (ReturnStatementSyntax)methodDeclarationNode.Body.Statements
                .First(s => s is ReturnStatementSyntax);
            var newReturnStatementNode = oldReturnStatementNode
                .WithExpression(SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression));

            var oldReturnTypeNode = (PredefinedTypeSyntax)methodDeclarationNode.ReturnType;
            var newReturnTypeNode = oldReturnTypeNode.WithKeyword(SyntaxFactory.Token(SyntaxKind.BoolKeyword));

            var newParameter = SyntaxFactory
                .Parameter(SyntaxFactory.Identifier("stringOut"))
                .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.OutKeyword)))
                .WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)));

            var newStringOutStatement = SyntaxFactory.ExpressionStatement(
                SyntaxFactory.AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    SyntaxFactory.IdentifierName("stringOut"),
                    oldReturnStatementNode.Expression));

            editor.InsertParameter(methodDeclarationNode, 0, newParameter);
            editor.InsertBefore(oldReturnStatementNode, newStringOutStatement);
            editor.ReplaceNode(oldReturnStatementNode, newReturnStatementNode);
            editor.ReplaceNode(oldReturnTypeNode, newReturnTypeNode);

            return editor.GetChangedDocument();
        }
    }
}