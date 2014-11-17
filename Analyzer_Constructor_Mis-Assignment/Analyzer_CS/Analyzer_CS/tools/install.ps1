param($installPath, $toolsPath, $package, $project)

$analyzerPath = join-path $toolsPath "analyzers"
$analyzerFilePath = join-path $analyzerPath "Analyzer_CS.dll"

$project.Object.AnalyzerReferences.Add("$analyzerFilePath")