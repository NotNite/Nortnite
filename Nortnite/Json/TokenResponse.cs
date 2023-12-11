using System.Text.Json.Serialization;

namespace Nortnite.Json;

public class TokenResponse {
    [JsonPropertyName("access_token")] public required string AccessToken { get; init; }
    [JsonPropertyName("refresh_token")] public string? RefreshToken { get; init; }
    [JsonPropertyName("refresh_expires")] public int? RefreshExpires { get; init; }
    [JsonPropertyName("refresh_expires_at")] public DateTime? RefreshExpiresAt { get; init; }
    [JsonPropertyName("account_id")] public required string AccountId { get; init; }
    [JsonPropertyName("displayName")] public required string DisplayName { get; init; }
}
