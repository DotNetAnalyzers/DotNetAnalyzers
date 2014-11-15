Imports System.Collections.Immutable
Imports Microsoft.CodeAnalysis.Diagnostics
Imports Microsoft.CodeAnalysis.CSharp.CSharpExtensions
Imports Microsoft.CodeAnalysis.CSharp.Syntax
Imports Microsoft.CodeAnalysis.CSharp.SyntaxExtensions
Imports Microsoft.CodeAnalysis.CSharp

<DiagnosticAnalyzer(LanguageNames.CSharp)>
Public Class MisAssignmentConstructor_Analyzer1Analyzer
  Inherits Diagnostics.DiagnosticAnalyzer



  Public Const DiagnosticId = "NA_001"
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
    context.RegisterSyntaxNodeAction(Of SyntaxKind)(AddressOf AnalyzeNode, SyntaxKind.ConstructorDeclaration)
  End Sub

  Public Sub AnalyzeNode(context As SyntaxNodeAnalysisContext)
    Dim constructor = TryCast(context.Node, Syntax.ConstructorDeclarationSyntax)
    If constructor Is Nothing Then Exit Sub
    Dim st = constructor.SyntaxTree
    Dim names = constructor.ParameterList.Parameters.Select(Function(p) p.Identifier.Text)
    Dim statements = constructor.Body.Statements.OfType(Of ExpressionStatementSyntax).
                                                 Where(Function(s) TypeOf s.Expression Is AssignmentExpressionSyntax).Select(Function(s) s.Expression).Cast(Of AssignmentExpressionSyntax)

    '.Where(Function(s) s.IsKind(SyntaxKind.SimpleAssignmentExpression))
    For Each statement In statements
      Dim lhs_Id = CType(statement.Left, SimpleNameSyntax).Identifier
      If names.Contains(lhs_Id.Text) Then
        context.ReportDiagnostic(Diagnostic.Create(Rule, Location.Create(st, statement.Left.Span), lhs_Id))
      End If
    Next
  End Sub
End Class