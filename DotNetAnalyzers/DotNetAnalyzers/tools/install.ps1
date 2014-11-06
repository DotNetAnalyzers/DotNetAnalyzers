param($installPath, $toolsPath, $package, $project)

$analyzerPath = join-path $toolsPath "analyzers"
$analyzerFilePath = join-path $analyzerPath "DotNetAnalyzers.dll"

$project.Object.AnalyzerReferences.Add("$analyzerFilePath")