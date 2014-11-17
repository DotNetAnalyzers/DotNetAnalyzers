Imports Microsoft.CodeAnalysis.Diagnostics

<DiagnosticAnalyzer(LanguageNames.VisualBasic)>
Public Class MisAssignmentConstructor_Analyzer1Analyzer
  Inherits Diagnostics.DiagnosticAnalyzer



  Public Const DiagnosticId = "MA_001"
  Friend Const Title = "Did you mean to assign to a parameter arguement?"
  Friend Const MessageFormat = "Did you mean to assign to a parameter arguement? (Parameter name '{0}') "
  Friend Const Category = "Naming"

  Friend Shared Rule As New DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault:=True)

  Public Overrides ReadOnly Property SupportedDiagnostics As ImmutableArray(Of DiagnosticDescriptor)
    Get
      Return ImmutableArray.Create(Rule)
    End Get
  End Property

  Public Overrides Sub Initialize(context As AnalysisContext)
    context.RegisterSyntaxNodeAction(Of SyntaxKind)(AddressOf AnalyzeNode, SyntaxKind.SubNewStatement)
  End Sub

  Public Sub AnalyzeNode(context As SyntaxNodeAnalysisContext)
    Dim constructor = TryCast(context.Node, Syntax.SubNewStatementSyntax)
    If constructor Is Nothing Then Exit Sub
    Dim st = constructor.SyntaxTree
    Dim names = constructor.ParameterList.Parameters.Select(Function(p) p.Identifier.Identifier.GetIdentifierText)
    For Each statement In DirectCast(constructor.Parent, ConstructorBlockSyntax).Statements.OfType(Of AssignmentStatementSyntax).Where(Function(a) TypeOf a.Left Is SimpleNameSyntax)
      Dim lhs_Id = CType(statement.Left, SimpleNameSyntax).Identifier
      If names.Contains(lhs_Id.GetIdentifierText()) Then context.ReportDiagnostic(Diagnostic.Create(Rule, Location.Create(st, statement.Left.Span), lhs_Id))
    Next
  End Sub
End Class