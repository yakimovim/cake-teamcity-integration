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

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Default")
.IsDependentOn("Clean")
.IsDependentOn("Restore-NuGet-Packages")
.IsDependentOn("Build");

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
         .SetVerbosity(Verbosity.Minimal)
         .UseToolVersion(MSBuildToolVersion.VS2017));
});

RunTarget(target);