$rootDir = $env:APPVEYOR_BUILD_FOLDER
$buildVersion = $Env:APPVEYOR_BUILD_VERSION
Write-Host "APPVEYOR_BUILD_VERSION: $buildVersion"

<#Pacote Principal#>
$solutionFile = Join-Path $rootDir "BoletoNetCore\BoletoNetCore.csproj"
$nupkgPath = Join-Path $rootDir "NuGet"

dotnet build -c Release $solutionFile /p:Version=$buildVersion
dotnet pack -c Release $solutionFile -o $nupkgPath

<#Pacote PDF#>
$solutionFilePDF = Join-Path $rootDir "BoletoNetCore.PDF\BoletoNetCore.PDF.csproj"
$nupkgPathPDF = Join-Path $rootDir "NuGet.PDF"

dotnet build -c Release $solutionFilePDF /p:Version=$buildVersion
dotnet pack -c Release $solutionFilePDF -o $nupkgPathPDF