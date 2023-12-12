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
var userOption = new Option<string>(
    "--username",
    "User to log in as"
) {
    IsRequired = true
};

var rootCommand = new RootCommand("it Norts your Forts™") {dontLaunchOption, logOption, userOption};
rootCommand.SetHandler(async (dontLaunch, log, user) => {
    var epicLogin = new EpicLogin();
    var launcher = new Launcher();
    var dataManager = new PersistedDataManager();

    var tokenResponse = await epicLogin.LoginMaybeCached(
                            user,
                            dataManager.GetCachedResponseForUser(user),
                            log
                        );
    dataManager.SaveCachedResponseForUser(user, tokenResponse);
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
}, dontLaunchOption, logOption, userOption);

await rootCommand.InvokeAsync(args);
