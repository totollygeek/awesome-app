using System;
using System.Collections.Generic;
using System.Linq;
using Components;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.Docker;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.Docker.DockerTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[GitHubActions(
    "continuous",
    GitHubActionsImage.WindowsLatest,
    GitHubActionsImage.UbuntuLatest,
    GitHubActionsImage.MacOsLatest,
    OnPushBranches = new[] { MainBranch },
    ImportSecrets = new[] { "NuGetApiKey" },
    PublishArtifacts = true,
    InvokedTargets = new[] { nameof(Test), nameof(PushPackages) },
    CacheKeyFiles = new[] { "global.json", "source/**/*.csproj" })]
[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild, IHaveGit
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    const string MainBranch = "main";
    
    public static int Main () => Execute<Build>(
        c => c.Clean, 
        x => x.PushPackages);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;
    
    [Parameter] [Secret] string NuGetApiKey;

    [Solution] readonly Solution Solution;
    GitVersion GitVersion => From<IHaveGit>().Versioning;
    GitRepository GitRepository => From<IHaveGit>().GitRepository;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath OutputDirectory => RootDirectory / "output";
    AbsolutePath TestResultDirectory => OutputDirectory / "test-results";
    AbsolutePath PackagesDirectory => OutputDirectory / "packages";
    AbsolutePath Dockerfile => SourceDirectory / "awesome.app" / "Dockerfile";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            Logger.Info($"{nameof(RootDirectory)}:\t {RootDirectory}");
            Logger.Info($"{nameof(TestsDirectory)}:\t {TestsDirectory}");
            Logger.Info($"{nameof(OutputDirectory)}: {OutputDirectory}");
            Logger.Info($"{nameof(Dockerfile)}:\t {Dockerfile}");
            
            Console.WriteLine();
            
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(OutputDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(_ => _
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(_ => _
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());
        });
    
    [Partition(2)] readonly Partition TestPartition;
    IEnumerable<Project> TestProjects => TestPartition.GetCurrent(Solution.GetProjects("*.tests"));

    Target Test => _ => _
        .DependsOn(Compile)  
        .Produces(TestResultDirectory / "*.trx")
        .Produces(TestResultDirectory / "*.xml")
        .Produces(TestResultDirectory / "*.html")
        .Executes(() =>
        {
            EnsureExistingDirectory(TestResultDirectory);
            
            try
            {
                var logger = IsLocalBuild ? "html" : "trx";
                
                DotNetTest(_ => _
                        .SetConfiguration(Configuration)
                    .SetNoBuild(SucceededTargets.Contains(Compile))
                    .ResetVerbosity()
                    .SetResultsDirectory(TestResultDirectory)
                    .When(IsServerBuild, _ => _
                        .EnableCollectCoverage()
                        .SetCoverletOutputFormat(CoverletOutputFormat.cobertura)
                        .SetExcludeByFile("*.Generated.cs")
                        .SetCoverletOutputFormat(
                            $"\\\"{CoverletOutputFormat.cobertura},{CoverletOutputFormat.json}\\\"")
                        .EnableUseSourceLink())
                    .CombineWith(TestProjects, (_, p) => _
                            .SetProjectFile(p)
                            .SetLogger($"{logger};LogFileName={p.Name}.{logger}")),
                    completeOnFailure: true);
            }
            finally
            {
                ReportTestCount();
            }
        });

    void ReportTestCount()
    {
        IEnumerable<string> GetOutcomes(AbsolutePath file)
            => XmlTasks.XmlPeek(
                file,
                "/xn:TestRun/xn:Results/xn:UnitTestResult/@outcome",
                ("xn", "http://microsoft.com/schemas/VisualStudio/TeamTest/2010"));

        var resultFiles = TestResultDirectory.GlobFiles("*.trx");
        var outcomes = resultFiles.SelectMany(GetOutcomes).ToList();
        var passedTests = outcomes.Count(x => x == "Passed");
        var failedTests = outcomes.Count(x => x == "Failed");
        var skippedTests = outcomes.Count(x => x == "NotExecuted");

        ReportSummary(_ => _
            .When(failedTests > 0, _ => _
                .AddPair("Failed", failedTests.ToString()))
            .AddPair("Passed", passedTests.ToString())
            .When(skippedTests > 0, _ => _
                .AddPair("Skipped", skippedTests.ToString())));
    }

    Target Pack => _ => _
        .DependsOn(Test)
        .Produces(PackagesDirectory / "*.nupkg")
        .Executes(() =>
        {
            EnsureExistingDirectory(PackagesDirectory);

            DotNetPack(_ => _
                .SetProject(Solution)
                .SetConfiguration(Configuration)
                .SetNoBuild(SucceededTargets.Contains(Compile))
                .SetOutputDirectory(PackagesDirectory)
                .SetRepositoryUrl(GitRepository.HttpsUrl)
                .SetVersion(GitVersion.NuGetVersionV2));
        });

    Target PushPackages => _ => _
        .DependsOn(Test, Pack)
        .Requires(() => NuGetApiKey)
        .Executes(() =>
        {
            Logger.Info("");
        });

    Target BuildDockerImage => _ => _
        .Executes(() =>
        {
            DockerBuild(_ => _
                .SetFile(Dockerfile));
        });

    T From<T>()
        where T : INukeBuild
        => (T) (object) this;
}
