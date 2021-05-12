using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.Tools.GitVersion;
using static Nuke.Common.ValueInjection.ValueInjectionUtility;

namespace Components
{
    public interface IHaveGit : INukeBuild
    {
        [GitRepository] 
        [Required] 
        GitRepository GitRepository => TryGetValue(() => GitRepository);
        
        [GitVersion(Framework = "net5.0", NoFetch = true)]
        [Required]
        GitVersion Versioning => TryGetValue(() => Versioning);
    }
}