using System;

using Alko.CodeCakeWrapper;

using Cake.Core;

using static System.IO.Path;

namespace cake
{
    class Build : AlkoBuildBase
    {
        public Build()
        {
            srcDirectory = Combine(moduleDirectory, @"src\CustomersAndStaff");

            ServiceRunnerTasks();
            TcTestTasks();
            PackToNugetTasks();
        }

        private void ServiceRunnerTasks()
        {
            var yarnRunner = new YarnRunner();
            var suites = new[]
                {
                    Combine(cementDirectory, @"alco.global-services\serviceSuites\cassandra3.yaml"),
                    Combine(moduleDirectory, @"serviceSuites\startAll.yaml")
                };

            CakeTask("ServiceRunner").Does(() => { Cake.StartServices(cementDirectory, suites, false); });
            tsStartServices = CakeTask("StartServices").Does(() => { Cake.StartServices(cementDirectory, suites, true); });
            tsStopServices = CakeTask("StopServices").Does(() => { Cake.StopServices(cementDirectory, suites); });
            tsStartYarn = CakeTask("StartYarn").Does(() => { yarnRunner.Start(moduleDirectory, TimeSpan.FromMinutes(5)).Wait(); });
            tsStopYarn = CakeTask("StopYarn").Does(() => { yarnRunner.KillYarnProcesses().Wait(); });
        }

        private void TcTestTasks()
        {
            CakeTask("TcUnitTests")
                .Does(() => { Cake.RunTests(cementDirectory, Combine(srcDirectory, @"UnitTests\bin\Market.CustomersAndStaff.UnitTests.dll")); });

            CakeTask("TcRepositoryTests")
                .Does(() => { Cake.RunTests(cementDirectory, Combine(srcDirectory, @"RepositoriesTests\bin\Market.CustomersAndStaff.RepositoriesTests.dll")); })
                .IsDependentOn(tsStartServices)
                .Finally(() => tsStopServices.Task.Execute(Cake).Wait());

            CakeTask("TcApiTests")
                .Does(() => { Cake.RunTests(cementDirectory, Combine(srcDirectory, @"ApiTests\bin\Market.CustomersAndStaff.ApiTests.dll")); })
                .IsDependentOn(tsStartServices)
                .Finally(() => tsStopServices.Task.Execute(Cake).Wait());

            CakeTask("TcFunctionalTests", false)
                .Does(() => { Cake.RunTests(cementDirectory, Combine(srcDirectory, @"FunctionalTests\bin\Market.CustomersAndStaff.FunctionalTests.dll")); })
                .IsDependentOn(tsStartServices)
                .IsDependentOn(tsStartYarn)
                .Finally(() =>
                    {
                        tsStopServices.Task.Execute(Cake).Wait();
                        tsStopYarn.Task.Execute(Cake).Wait();
                    });
        }

        private void PackToNugetTasks()
        {
            PackAndPublishTasks("ServiceApiClient", @"ServiceApi.Client");
            PackAndPublishTasks("Models", @"Models");
            PackAndPublishTasks("FunctionalTestsComponents", @"FunctionalTests.Components");
        }

        private void PackAndPublishTasks(string taskNameSuffix, string projectName)
        {
            var pack =
                CakeTask($"Pack{taskNameSuffix}")
                    .Does(() => { Cake.PackCsproj(GetCsprojPath(projectName)); });

            CakeTask($"Publish{taskNameSuffix}")
                .IsDependentOn(pack)
                .Does(() => { Cake.PublishProject(GetArtifactsFolderPath(projectName), KonturNugetAddress); });
        }

        private string GetCsprojPath(string projectName)
        {
            return Combine(srcDirectory, projectName, projectName + ".csproj");
        }


        private string GetArtifactsFolderPath(string projectName)
        {
            return Combine(srcDirectory, projectName, ".artifacts");
        }

        private readonly string srcDirectory;
        private CakeTaskBuilder<ActionTask> tsStartServices;
        private CakeTaskBuilder<ActionTask> tsStopServices;
        private CakeTaskBuilder<ActionTask> tsStartYarn;
        private CakeTaskBuilder<ActionTask> tsStopYarn;
    }
}