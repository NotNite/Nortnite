using System.Diagnostics;
using System.Text.Json;
using Nortnite.Json;

namespace Nortnite;

public class Launcher {
    public const string ManifestsDir = "C:\\ProgramData\\Epic\\EpicGamesLauncher\\Data\\Manifests";

    public Manifest GetFortniteManifest() {
        var manifests = Directory.GetFiles(ManifestsDir, "*.item");

        return manifests
            .Select(File.ReadAllText)
            .Select(x => JsonSerializer.Deserialize<Manifest>(x))
            .First(x => x?.AppName == "Fortnite")!;
    }

    public void LaunchFortnite(
        Manifest manifest,
        string authPassword,
        string username,
        string id
    ) {
        var launcher = Path.Combine(manifest.InstallLocation, manifest.LaunchExecutable);
        string[] fortniteArgs = [
            manifest.LaunchCommand,
            "-AUTH_LOGIN=unused",
            $"-AUTH_PASSWORD={authPassword}",
            "-AUTH_TYPE=exchangecode",
            "-epicapp=Fortnite",
            "-epicenv=Prod",
            "-EpicPortal",
            $"-epicusername=\"{username}\"",
            $"-epicuserid={id}",
            "-epiclocale=en",
            "-epicsandboxid=fn"
        ];

        var startInfo = new ProcessStartInfo {
            FileName = launcher,
            WorkingDirectory = Path.GetDirectoryName(launcher),
            Arguments = string.Join(' ', fortniteArgs)
        };
        Process.Start(startInfo);
    }
}
