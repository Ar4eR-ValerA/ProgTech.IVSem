using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Ar4eR_ValerA
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class Ar4eR_ValerA_I3_Analyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "Ar4eR_ValerA";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.I3_AnalyzerTitle),
            Resources.ResourceManager, typeof(Resources));

        private static readonly LocalizableString MessageFormat =
            new LocalizableResourceString(nameof(Resources.I3_AnalyzerMessageFormat), Resources.ResourceManager,
                typeof(Resources));

        private static readonly LocalizableString Description =
            new LocalizableResourceString(nameof(Resources.I3_AnalyzerDescription), Resources.ResourceManager,
                typeof(Resources));

        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat,
            Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        {
            get { return ImmutableArray.Create(Rule); }
        }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeArgument, SyntaxKind.Argument);
        }

        private static void AnalyzeArgument(SyntaxNodeAnalysisContext context)
        {
            var argumentSyntax = (ArgumentSyntax)context.Node;

            if (argumentSyntax.Expression is LiteralExpressionSyntax literalExpressionSyntax &&
                literalExpressionSyntax.Token.Kind() is SyntaxKind.NumericLiteralToken &&
                !(argumentSyntax.Parent is EqualsValueClauseSyntax))
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    context.Node.GetLocation(),
                    literalExpressionSyntax.ToString());

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}