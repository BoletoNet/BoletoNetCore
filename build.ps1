$rootDir = $env:APPVEYOR_BUILD_FOLDER
$buildNumber = $env:APPVEYOR_BUILD_NUMBER

<#Pacote Principal#>
$solutionFile = "$rootDir\BoletoNetCore\BoletoNetCore.csproj"
$solutionTest = "$rootDir\BoletoNetCore.Testes\BoletoNetCore.Testes.csproj"
$nuspecPath = "$rootDir\BoletoNetCore\BoletoNetCore.nuspec"
$nupkgPath = "$rootDir\NuGet\"

[xml]$xml = cat $nuspecPath
$xml.package.metadata.version="3.0.1."+"$buildNumber"
$xml.Save($nuspecPath)

dotnet publish -f netstandard2.0 -c release $solutionFile
dotnet pack -c release $solutionFile /p:NuspecFile=$nuspecPath -o $nupkgPath
appveyor PushArtifact $nupkgPath

# <#Pacote PDF#>
# $solutionFilePDF = "$rootDir\BoletoNetCore.PDF\BoletoNetCore.PDF.csproj"
# $nuspecPathPDF = "$rootDir\BoletoNetCore.PDF\BoletoNetCore.PDF.nuspec"
# $nupkgPathPDF = "$rootDir\NuGet.PDF\"

# [xml]$xmlPDF = cat $nuspecPathPDF
# $xmlPDF.package.metadata.version="3.0.1."+"$buildNumber"
# $xmlPDF.Save($nuspecPathPDF)

# dotnet pack -c Release $solutionFilePDF /p:NuspecFile=$nuspecPathPDF -o $nupkgPathPDF
# appveyor PushArtifact $nupkgPathPDF
