$rootDir = $env:APPVEYOR_BUILD_FOLDER
$buildVersion = $Env:APPVEYOR_BUILD_VERSION

<#Pacote Principal#>
$solutionFile = "$rootDir\BoletoNetCore\BoletoNetCore.csproj"
$nupkgPath = "$rootDir\NuGet\"

dotnet build -c Release $solutionFile /p:Version=$buildVersion
dotnet pack -c Release $solutionFile -o $nupkgPath

<#Pacote PDF#>
$solutionFilePDF = "$rootDir\BoletoNetCore.PDF\BoletoNetCore.PDF.csproj"
$nupkgPathPDF = "$rootDir\NuGet.PDF\"

dotnet build -c Release $solutionFilePDF /p:Version=$buildVersion
dotnet pack -c Release $solutionFilePDF -o $nupkgPathPDF