using System.Text.Json;

using BlazorSignalRBackplaneTest.Components.Config.Models;

using Microsoft.AspNetCore.Components;

namespace BlazorSignalRBackplaneTest.Components.Config
{
    public partial class KeycloakConfig
    {
        [Inject]
        private HttpClient HttpClient { get; set; } = default!;

        private OidcConfiguration? Configuration = default!;

        protected override async Task OnParametersSetAsync()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://keycloak.jaspervannoordenburg.nl/realms/amazingcluster/.well-known/openid-configuration");
            using HttpResponseMessage response = await HttpClient.SendAsync(request);
            Configuration = await JsonSerializer.DeserializeAsync<OidcConfiguration>(response.Content.ReadAsStream());
        }
    }
}
