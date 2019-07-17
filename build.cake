#addin Cake.Coveralls
#tool "nuget:?package=NUnit.ConsoleRunner"
#tool "nuget:?package=OpenCover"
#tool coveralls.net&version=0.7.0
using System.Xml.Linq;

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

Task("RunNugetPack").WithCriteria(string.IsNullOrWhiteSpace(EnvironmentVariable("APPVEYOR_PULL_REQUEST_NUMBER"))).Does(() =>
{
    DeleteFiles("*.nupkg");

    var buildNumber = EnvironmentVariable("appveyor_build_version");
    var nuGetPackSettings = new NuGetPackSettings {
        Version = $"{buildNumber}",
        OutputDirectory = "./"
    };
    var nuspecFilePath = File("./Boleto2.Net/Boleto2.Net.nuspec");
    NuGetPack(nuspecFilePath, nuGetPackSettings);
    var nupkg = GetFiles("*.nupkg").First();
});

Task("RunCoverage").IsDependentOn("Build").Does(() =>
{
    OpenCover(tool => tool.NUnit3("./**/bin/Release/*.Testes.dll"),
              new FilePath("./coverage.xml"),
              new OpenCoverSettings().WithFilter("+[*]Boleto2Net.*").WithFilter("-[*]Boleto2Net.Testes.*")
    );
    CoverallsNet("coverage.xml", CoverallsNetReportType.OpenCover);
});

Task("RunTests").IsDependentOn("Build").Does(() =>
{
    var testAssemblies = GetFiles("./**/bin/Release/*.Testes.dll");
    NUnit3(testAssemblies, new NUnit3Settings { TeamCity = true });
});

Task("RestorePackages").Does(() =>
{
     NuGetRestore(File("Boleto2.Net.sln"), new NuGetRestoreSettings { NoCache = true });
});

Task("Build").IsDependentOn("RestorePackages").Does(() =>
{
    MSBuild("Boleto2.Net.sln", config => config.SetVerbosity(Verbosity.Minimal).SetConfiguration(configuration));
});

Task("Default").IsDependentOn("Build").Does(() => {});
Task("CiBuild").IsDependentOn("RunCoverage").IsDependentOn("RunNugetPack").Does(() => {});

RunTarget(target);
