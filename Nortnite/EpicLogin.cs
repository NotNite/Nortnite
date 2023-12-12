using System.CommandLine.Parsing;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using Nortnite.Json;

namespace Nortnite;

public class EpicLogin {
    private HttpClient GetHttpClient() {
        var http = new HttpClient();
        http.BaseAddress = new Uri(Constants.AccountsBaseUrl);

        http.DefaultRequestHeaders.Accept.Clear();
        http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var ver = Assembly.GetExecutingAssembly().GetName().Version!.ToString();
        http.DefaultRequestHeaders.UserAgent.Clear();
        http.DefaultRequestHeaders.UserAgent.Add(new("Nortnite", ver));

        var basic = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{Constants.LauncherClient}:{Constants.LauncherSecret}")
        );
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basic);

        return http;
    }

    public async Task<TokenResponse> Login(bool log = false) {
        Console.WriteLine($"Please go to the following URL: {Constants.RedirectUrl}");
        Console.Write("Enter the provided authorization code: ");
        var authCode = Console.ReadLine()!;

        using var http = this.GetHttpClient();
        var form = new FormUrlEncodedContent(new Dictionary<string, string> {
            {"grant_type", "authorization_code"},
            {"token_type", "eg1"},
            {"code", authCode}
        });

        var resp = await http.PostAsync("/account/api/oauth/token", form);
        if (log) Console.WriteLine(await resp.Content.ReadAsStringAsync());
        resp.EnsureSuccessStatusCode();
        return (await resp.Content.ReadFromJsonAsync<TokenResponse>())!;
    }

    public async Task<TokenResponse> Refresh(string token, bool log = false) {
        using var http = this.GetHttpClient();
        var form = new FormUrlEncodedContent(new Dictionary<string, string> {
            {"grant_type", "refresh_token"},
            {"token_type", "eg1"},
            {"refresh_token", token}
        });

        var resp = await http.PostAsync("/account/api/oauth/token", form);
        if (log) Console.WriteLine(await resp.Content.ReadAsStringAsync());
        resp.EnsureSuccessStatusCode();
        return (await resp.Content.ReadFromJsonAsync<TokenResponse>())!;
    }

    public async Task<TokenResponse> LoginMaybeCached(string user, TokenResponse? cached, bool log = false) {
        if (cached is not null) {
            try {
                var refresh = await this.Refresh(cached.RefreshToken!, log);
                if (refresh.DisplayName != user) {
                    Console.WriteLine($"Expected user {user} but got user {refresh.DisplayName}");
                    Environment.Exit(1);
                }
                return refresh;
            } catch (Exception e) {
                Console.WriteLine("Failed to refresh token, logging in normally");
                if (log) Console.WriteLine(e);
            }
        }

        var tokenResponse = await this.Login(log);
        if (tokenResponse.DisplayName != user) {
            Console.WriteLine($"Expected user {user} but got user {tokenResponse.DisplayName}");
            Environment.Exit(1);
        }
        return tokenResponse;
    }

    public async Task<ExchangeResponse> GetExchange(string token, bool log = false) {
        using var http = this.GetHttpClient();
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var resp = await http.GetAsync("/account/api/oauth/exchange");
        if (log) Console.WriteLine(await resp.Content.ReadAsStringAsync());
        resp.EnsureSuccessStatusCode();
        return (await resp.Content.ReadFromJsonAsync<ExchangeResponse>())!;
    }
}
