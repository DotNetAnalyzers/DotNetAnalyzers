Imports Microsoft.CodeAnalysis.CSharp
Imports Microsoft.CodeAnalysis.CSharp.Syntax

<ExportCodeFixProvider("MisAssignmentConstructor_Analyzer1CodeFixProvider", LanguageNames.CSharp), [Shared]>
Public Class MisAssignmentConstructor_Analyzer1CodeFixProvider
  Inherits CodeFixProvider

  Public NotOverridable Overrides Function GetFixableDiagnosticIds() As ImmutableArray(Of String)
    Return ImmutableArray.Create(MisAssignmentConstructor_Analyzer1Analyzer.DiagnosticId)
  End Function

  Public NotOverridable Overrides Function GetFixAllProvider() As FixAllProvider
    Return WellKnownFixAllProviders.BatchFixer
  End Function

  Private Async Function Prefix_WithUnderscore(document As Document, assignment_statement As AssignmentExpressionSyntax, cancellationToken As CancellationToken) As Task(Of Solution)
    Dim LHS = DirectCast(assignment_statement.Left, SimpleNameSyntax)
    Dim _LHS = LHS.WithIdentifier(SyntaxFactory.Identifier("_" & LHS.Identifier.Text))
    Dim semanticModel = Await document.GetSemanticModelAsync(cancellationToken)
    Dim root = Await semanticModel.SyntaxTree.GetRootAsync
    Dim tree = root.ReplaceNode(LHS, _LHS).SyntaxTree
    Dim newSolution = document.WithSyntaxRoot(Await tree.GetRootAsync)
    Return newSolution.Project.Solution
  End Function

  Private Async Function Prefix_WithThisDot(document As Document, assignment_statement As AssignmentExpressionSyntax, cancellationToken As CancellationToken) As Task(Of Solution)
    Dim LHS = DirectCast(assignment_statement.Left, SimpleNameSyntax)
    Dim _LHS = SyntaxFactory.ExpressionStatement(
         SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, SyntaxFactory.ThisExpression, SyntaxFactory.IdentifierName(LHS.Identifier.Text))
           )
    Dim ass = SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, _LHS.Expression, assignment_statement.Right)
    Dim semanticModel = Await document.GetSemanticModelAsync(cancellationToken)
    Dim root = Await semanticModel.SyntaxTree.GetRootAsync
    Dim tree = root.ReplaceNode(Of SyntaxNode)(assignment_statement, ass).SyntaxTree
    Dim newSolution = Document.WithSyntaxRoot(Await tree.GetRootAsync)
    Return newSolution.Project.Solution
  End Function

  Public NotOverridable Overrides Async Function ComputeFixesAsync(context As CodeFixContext) As Task

    Dim root = Await context.Document.GetSyntaxRootAsync(context.CancellationToken)
    Dim diagnostic = context.Diagnostics.First
    Dim diagnosticSpan = diagnostic.Location.SourceSpan
    Dim declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType(Of SimpleNameSyntax)()
    If declarations.Any Then
      Dim ID_Node = declarations.First
      Dim Assignment_Statement = TryCast(ID_Node.Parent, AssignmentExpressionSyntax)
      If Assignment_Statement Is Nothing Then Return
      context.RegisterFix(CodeAction.Create("Prefix with underscore", Function(c) Prefix_WithUnderscore(context.Document, Assignment_Statement, c)), diagnostic)
      context.RegisterFix(CodeAction.Create("Prefix with this.", Function(c) Prefix_WithThisDot(context.Document, Assignment_Statement, c)), diagnostic)
    End If
    Return
  End Function
End Class