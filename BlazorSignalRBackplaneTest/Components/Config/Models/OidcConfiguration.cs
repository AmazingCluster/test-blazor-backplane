using System.Text.Json.Serialization;

namespace BlazorSignalRBackplaneTest.Components.Config.Models
{
    public class OidcConfiguration
    {
        [JsonPropertyName("issuer")]
        public string Issuer { get; set; } = default!;

        [JsonPropertyName("authorization_endpoint")]
        public string AuthorizationEndpoint { get; set; } = default!;

        [JsonPropertyName("token_endpoint")]
        public string TokenEndpoint { get; set; } = default!;

        [JsonPropertyName("introspection_endpoint")]
        public string IntrospectionEndpoint { get; set; } = default!;

        [JsonPropertyName("userinfo_endpoint")]
        public string UserinfoEndpoint { get; set; } = default!;
    }
}
