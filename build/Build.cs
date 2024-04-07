using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Components;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.AzurePipelines;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.Docker;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.Tools.Docker.DockerTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
[GitHubActions(
    "gh-actions",
    GitHubActionsImage.UbuntuLatest,
    GitHubActionsImage.WindowsLatest,
    GitHubActionsImage.MacOsLatest,
    FetchDepth = 0,
    Submodules = GitHubActionsSubmodules.Recursive,
    OnPushBranches = [MainBranch],
    ImportSecrets = [nameof(NuGetApiKey)],
    PublishArtifacts = true,
    EnableGitHubToken = true,
    InvokedTargets = [nameof(Test), nameof(PushPackages)],
    CacheKeyFiles = ["global.json", "source/**/*.csproj"])]
[AzurePipelines(
    AzurePipelinesImage.UbuntuLatest,
    AzurePipelinesImage.WindowsLatest,
    AzurePipelinesImage.MacOsLatest,
    Submodules = true,
    InvokedTargets = [nameof(PushPackages)],
    NonEntryTargets = [nameof(Clean), nameof(Restore), nameof(Compile), nameof(Pack), nameof(PushPackages)],
    ImportSecrets = [nameof(NuGetApiKey)])]
[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild, IHaveGit
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    const string MainBranch = "main";

    public static int Main() => Execute<Build>(
        c => c.Clean,
        x => x.PushPackages);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter][Secret] string NuGetApiKey;

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
            Serilog.Log.Information("{RootDirectoryName}:\t {RootDirectory}", nameof(RootDirectory), RootDirectory);
            Serilog.Log.Information("{TestsDirectoryName}:\t {TestsDirectory}", nameof(TestsDirectory), TestsDirectory);
            Serilog.Log.Information("{OutputDirectoryName}:\t {OutputDirectory}", nameof(OutputDirectory), OutputDirectory);
            Serilog.Log.Information("{DockerfileName}:\t {Dockerfile}", nameof(Dockerfile), Dockerfile);

            Console.WriteLine();

            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(dir => dir.DeleteDirectory());
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(dir => dir.DeleteDirectory());
            OutputDirectory.CreateOrCleanDirectory();
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
    IEnumerable<Project> TestProjects => TestPartition.GetCurrent(Solution.GetAllProjects("*.tests"));

    Target Test => _ => _
        .DependsOn(Compile)
        .Produces(TestResultDirectory / "*.trx")
        .Produces(TestResultDirectory / "*.xml")
        .Produces(TestResultDirectory / "*.html")
        .Executes(() =>
        {
            TestResultDirectory.CreateDirectory();

            try
            {
                var logger = IsLocalBuild ? "html" : "trx";

                DotNetTest(_ => _
                        .SetConfiguration(Configuration)
                        .SetFilter("TestCategory!=failing")
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
                            .AddLoggers($"{logger};LogFileName={p.Name}.{logger}")), completeOnFailure: true);
            }
            finally
            {
                ReportTestResults();
                ReportTestCount();
            }
        });

    void ReportTestResults()
    {
        TestResultDirectory.GlobFiles("*.trx").ForEach(x =>
            AzurePipelines.Instance?.PublishTestResults(
                type: AzurePipelinesTestResultsType.VSTest,
                title: $"{Path.GetFileNameWithoutExtension(x)} ({AzurePipelines.Instance.StageDisplayName})",
                files: new string[] { x }));
    }

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
            PackagesDirectory.CreateDirectory();

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
            Serilog.Log.Information("Pushing packages here...");
        });

    Target BuildDockerImage => _ => _
        .Executes(() =>
        {
            DockerBuild(_ => _
                .AddTag("awesome")
                .SetFile(Dockerfile)
                .SetPath(RootDirectory));
        });

    T From<T>()
        where T : INukeBuild
        => (T)(object)this;
}
