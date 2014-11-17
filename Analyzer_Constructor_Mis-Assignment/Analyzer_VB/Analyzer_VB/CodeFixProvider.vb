<ExportCodeFixProvider("MisAssignmentConstructor_Analyzer1CodeFixProvider", LanguageNames.VisualBasic), [Shared]>
Public Class MisAssignmentConstructor_Analyzer1CodeFixProvider
  Inherits CodeFixProvider

  Public NotOverridable Overrides Function GetFixableDiagnosticIds() As ImmutableArray(Of String)
    Return ImmutableArray.Create(MisAssignmentConstructor_Analyzer1Analyzer.DiagnosticId)
  End Function

  Public NotOverridable Overrides Function GetFixAllProvider() As FixAllProvider
    Return WellKnownFixAllProviders.BatchFixer
  End Function

  Public NotOverridable Overrides Async Function ComputeFixesAsync(context As CodeFixContext) As Task
    Dim root = Await context.Document.GetSyntaxRootAsync(context.CancellationToken)
    Dim diagnostic = context.Diagnostics.First
    Dim diagnosticSpan = diagnostic.Location.SourceSpan
    Dim declarations = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType(Of SimpleNameSyntax)()
    If declarations.Any Then
      Dim ID_Node = declarations.First
      Dim Assignment_Statement = TryCast(ID_Node.Parent, AssignmentStatementSyntax)
      If Assignment_Statement Is Nothing Then Return
      context.RegisterFix(CodeAction.Create("Prefix with underscore", Function(c) Prefix_WithUnderscore(context.Document, Assignment_Statement, c)), diagnostic)
      context.RegisterFix(CodeAction.Create("Prefix with Me.", Function(c) Prefix_WithMeDot(context.Document, Assignment_Statement, c)), diagnostic)
    End If
    Return
  End Function

  Private Async Function Prefix_WithUnderscore(document As Document, assignment_statement As AssignmentStatementSyntax, cancellationToken As CancellationToken) As Task(Of Solution)
    Dim LHS = DirectCast(assignment_statement.Left, SimpleNameSyntax)
    Dim _LHS = LHS.WithIdentifier(SyntaxFactory.Identifier("_" & LHS.Identifier.Text))
    Dim semanticModel = Await document.GetSemanticModelAsync(cancellationToken)
    Dim root = Await semanticModel.SyntaxTree.GetRootAsync
    Dim tree = root.ReplaceNode(LHS, _LHS).SyntaxTree
    Dim newSolution = document.WithSyntaxRoot(Await tree.GetRootAsync)
    Return newSolution.Project.Solution
  End Function

  Private Async Function Prefix_WithMeDot(document As Document, assignment_statement As AssignmentStatementSyntax, cancellationToken As CancellationToken) As Task(Of Solution)
    Dim LHS = DirectCast(assignment_statement.Left, SimpleNameSyntax)
    Dim _LHS = SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(SyntaxFactory.SimpleMemberAccessExpression(SyntaxFactory.MeExpression, SyntaxFactory.IdentifierName(LHS.Identifier.Text))))
    Dim ass = SyntaxFactory.SimpleAssignmentStatement(_LHS.Expression, assignment_statement.Right)
    Dim semanticModel = Await document.GetSemanticModelAsync(cancellationToken)
    Dim root = Await semanticModel.SyntaxTree.GetRootAsync
    Dim tree = root.ReplaceNode(Of SyntaxNode)(assignment_statement, ass).SyntaxTree
    Dim newSolution = document.WithSyntaxRoot(Await tree.GetRootAsync)
    Return newSolution.Project.Solution
  End Function
End Class