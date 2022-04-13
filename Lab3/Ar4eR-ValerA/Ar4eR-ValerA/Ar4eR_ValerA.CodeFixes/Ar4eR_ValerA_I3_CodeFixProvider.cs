using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Editing;

namespace Ar4eR_ValerA
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(Ar4eR_ValerA_I3_CodeFixProvider)), Shared]
    public class Ar4eR_ValerA_I3_CodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(Ar4eR_ValerA_I3_Analyzer.DiagnosticId); }
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
                    createChangedDocument: c => RemoveMagicNumber(context.Document, declaration, c),
                    equivalenceKey: nameof(CodeFixResources.CodeFixTitle)),
                diagnostic);
        }

        private async Task<Document> RemoveMagicNumber(
            Document document,
            MethodDeclarationSyntax methodSyntax,
            CancellationToken cancellationToken)
        {
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken);

            int magicNumberCount = 1;
            var literalExpressionSyntaxes = methodSyntax
                .DescendantNodesAndSelf()
                .Where(node => node is LiteralExpressionSyntax);
            
            foreach (var syntaxNode in literalExpressionSyntaxes)
            {
                var argumentSyntax = (LiteralExpressionSyntax)syntaxNode;
                var newLocalDeclaration = SyntaxFactory.LocalDeclarationStatement(
                    SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.ConstKeyword)),
                    SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                        SyntaxFactory.SeparatedList<VariableDeclaratorSyntax>(new[]
                        {
                            SyntaxFactory.VariableDeclarator(
                                    SyntaxFactory.Identifier($"magicNumber{magicNumberCount}"))
                                .WithInitializer(SyntaxFactory.EqualsValueClause(argumentSyntax))
                        })));


                editor.InsertBefore(argumentSyntax.FirstAncestorOrSelf<StatementSyntax>(), newLocalDeclaration);
                editor.ReplaceNode(argumentSyntax, SyntaxFactory.IdentifierName($"magicNumber{magicNumberCount}"));
                magicNumberCount++;
            }

            return editor.GetChangedDocument();
        }
    }
}