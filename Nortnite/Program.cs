using System.CommandLine;
using Nortnite;

var dontLaunchOption = new Option<bool>(
    "--dont-launch",
    () => false,
    "Go through the auth process but don't start Fortnite"
);
var logOption = new Option<bool>(
    "--log",
    () => false,
    "Log the token and exchange response"
);

var rootCommand = new RootCommand("it Norts your Forts™") {dontLaunchOption, logOption};
var parsed = rootCommand.Parse(args);
var dontLaunch = parsed.GetValueForOption(dontLaunchOption);
var log = parsed.GetValueForOption(logOption);

var epicLogin = new EpicLogin();
var launcher = new Launcher();

var tokenResponse = await epicLogin.Login(log);
var exchangeResponse = await epicLogin.GetExchange(tokenResponse.AccessToken, log);

if (!dontLaunch) {
    var manifest = launcher.GetFortniteManifest();
    launcher.LaunchFortnite(
        manifest,
        exchangeResponse.Code,
        tokenResponse.DisplayName,
        tokenResponse.AccountId
    );
}
