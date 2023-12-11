using System.Text.Json.Serialization;

namespace Nortnite.Json;

public class ExchangeResponse {
    [JsonPropertyName("code")] public required string Code { get; init; }
}
