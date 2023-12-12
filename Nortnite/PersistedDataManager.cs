using System.Text.Json;
using Nortnite.Json;

namespace Nortnite;

public class PersistedDataManager {
    public static string PersistedDataFolder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Nortnite"
    );

    public PersistedDataManager() {
        if (!Directory.Exists(PersistedDataFolder)) {
            Directory.CreateDirectory(PersistedDataFolder);
        }
    }

    public TokenResponse? GetCachedResponseForUser(string username) {
        var path = Path.Combine(PersistedDataFolder, $"{username}.json");
        if (File.Exists(path)) {
            var str = File.ReadAllText(path);
            var obj = JsonSerializer.Deserialize<TokenResponse>(str)!;
            if (!obj.Expired()) return obj;
        }

        return null;
    }

    public void SaveCachedResponseForUser(string username, TokenResponse response) {
        var path = Path.Combine(PersistedDataFolder, $"{username}.json");
        var str = JsonSerializer.Serialize(response);
        File.WriteAllText(path, str);
    }
}
