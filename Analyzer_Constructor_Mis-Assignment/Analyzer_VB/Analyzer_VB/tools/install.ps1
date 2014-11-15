param($installPath, $toolsPath, $package, $project)

$analyzerPath = join-path $toolsPath "analyzers"
$analyzerFilePath = join-path $analyzerPath "Analyzer_VB.dll"

$project.Object.AnalyzerReferences.Add("$analyzerFilePath")