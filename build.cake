#tool "nuget:?package=JetBrains.dotCover.CommandLineTools"

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// PARAMETERS
///////////////////////////////////////////////////////////////////////////////

var artifactsFolder = "./artifacts";
var solution = "./CakeTeamCityIntegration.sln";

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
   // Executed BEFORE the first task.
   Information("Running tasks...");
});

Teardown(ctx =>
{
   // Executed AFTER the last task.
   Information("Finished running tasks.");
});

TaskSetup(setupContext =>
{
   if(TeamCity.IsRunningOnTeamCity)
   {
      TeamCity.WriteStartBuildBlock(setupContext.Task.Name);

      TeamCity.WriteStartProgress(setupContext.Task.Name);
   }
});

TaskTeardown(teardownContext =>
{
   if(TeamCity.IsRunningOnTeamCity)
   {
      TeamCity.WriteEndProgress(teardownContext.Task.Name);

      TeamCity.WriteEndBuildBlock(teardownContext.Task.Name);
   }
});

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Default")
.IsDependentOn("Clean")
.IsDependentOn("Restore-NuGet-Packages")
.IsDependentOn("Build")
.IsDependentOn("Analyse-Test-Coverage");

Task("Clean")
.Does(() => {
   if(!DirectoryExists(artifactsFolder))
   {
      CreateDirectory(artifactsFolder);
   }

   CleanDirectory(artifactsFolder);
});

Task("Restore-NuGet-Packages")
.Does(() => {
   NuGetRestore(solution);
});

Task("Build")
.Does(() => {
   MSBuild(solution, settings =>
      settings
         .SetConfiguration(configuration)
         .SetMSBuildPlatform(MSBuildPlatform.x86)
         .SetVerbosity(Verbosity.Minimal)
         .UseToolVersion(MSBuildToolVersion.VS2017));
});

Task("Run-Tests")
.IsDependentOn("Clean")
.IsDependentOn("Build")
.Does(() => {
   var testDllsPattern = string.Format("./**/bin/{0}/*.*Tests.dll", configuration);

   var testDlls = GetFiles(testDllsPattern);

   foreach (var testDll in testDlls)
   {
      Information("\t" + testDll);
   }

   var testResultsFile = System.IO.Path.Combine(artifactsFolder, "testResults.trx");

   MSTest(testDlls, new MSTestSettings() {
      ResultsFile = testResultsFile
   });

   if(TeamCity.IsRunningOnTeamCity)
   {
      TeamCity.ImportData("mstest", testResultsFile);
   }
});

Task("Analyse-Test-Coverage")
.IsDependentOn("Clean")
.IsDependentOn("Build")
.Does(() => {
   var coverageResultFile = System.IO.Path.Combine(artifactsFolder, "coverageResult.dcvr");

   var testDllsPattern = string.Format("./**/bin/{0}/*.*Tests.dll", configuration);

   var testDlls = GetFiles(testDllsPattern);

   foreach (var testDll in testDlls)
   {
      Information("\t" + testDll);
   }

   var testResultsFile = System.IO.Path.Combine(artifactsFolder, "testResults.trx");

   DotCoverCover(tool => {
         tool.MSTest(testDlls, new MSTestSettings() {
            ResultsFile = testResultsFile
         });
      },
      new FilePath(coverageResultFile),
      new DotCoverCoverSettings()
         .WithFilter("+:Application")
         .WithFilter("-:Application.*Tests")
      );

   if(TeamCity.IsRunningOnTeamCity)
   {
      TeamCity.ImportData("mstest", testResultsFile);
   }

   DotCoverReport(coverageResultFile,
      System.IO.Path.Combine(artifactsFolder, "coverageResult.html"),
      new DotCoverReportSettings {
         ReportType = DotCoverReportType.HTML
      });

   if(TeamCity.IsRunningOnTeamCity)
   {
      TeamCity.ImportDotCoverCoverage(coverageResultFile);
   }
});


RunTarget(target);