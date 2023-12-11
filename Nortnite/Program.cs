using System.Diagnostics;
using System.Text.Json;
using Nortnite;
using Nortnite.Json;

var epicLogin = new EpicLogin();
var tokenResponse = await epicLogin.Login();
var exchangeResponse = await epicLogin.GetExchange(tokenResponse.AccessToken);

var manifestsDir = "C:\\ProgramData\\Epic\\EpicGamesLauncher\\Data\\Manifests";
var manifests = Directory.GetFiles(manifestsDir, "*.item");

var manifest = manifests
    .Select(File.ReadAllText)
    .Select(x => JsonSerializer.Deserialize<Manifest>(x))
    .First(x => x?.AppName == "Fortnite")!;

var launcher = Path.Combine(manifest.InstallLocation, manifest.LaunchExecutable);
string[] fortniteArgs = [
    manifest.LaunchCommand,
    "-AUTH_LOGIN=unused",
    $"-AUTH_PASSWORD={exchangeResponse.Code}",
    "-AUTH_TYPE=exchangecode",
    "-epicapp=Fortnite",
    "-epicenv=Prod",
    "-EpicPortal",
    $"-epicusername=\"{tokenResponse.DisplayName}\"",
    $"-epicuserid={tokenResponse.AccountId}",
    "-epiclocale=en",
    "-epicsandboxid=fn"
];

Console.WriteLine(launcher);
Console.WriteLine(string.Join(' ', fortniteArgs));

var startInfo = new ProcessStartInfo {
    FileName = launcher,
    WorkingDirectory = Path.GetDirectoryName(launcher),
    Arguments = string.Join(' ', fortniteArgs)
};
Process.Start(startInfo);
