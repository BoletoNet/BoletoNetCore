$rootDir = $env:APPVEYOR_BUILD_FOLDER
$buildNumber = $env:APPVEYOR_BUILD_NUMBER
$solutionFile = "$rootDir\BoletoNetCore\BoletoNetCore.csproj"
$solutionTest = "$rootDir\BoletoNetCore.Testes\BoletoNetCore.Testes.csproj"
$nuspecPath = "$rootDir\BoletoNetCore\BoletoNetCore.nuspec"
$nupkgPath = "$rootDir\NuGet\BoletoNetCore.{0}.nupkg"

[xml]$xml = cat $nuspecPath
$xml.package.metadata.version="3.0.0."+"$buildNumber"
$xml.Save($nuspecPath)

[xml]$xml = cat $nuspecPath
$nupkgPath = $nupkgPath -f $xml.package.metadata.version

<#dotnet test $solutionTest#>
dotnet pack -c Release $solutionFile -o $nupkgPath
appveyor PushArtifact $nupkgPath