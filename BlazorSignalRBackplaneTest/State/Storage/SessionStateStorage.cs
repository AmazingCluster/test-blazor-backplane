using Blazored.LocalStorage;

using Fluxor;
using Fluxor.Persist.Storage;

using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace BlazorSignalRBackplaneTest.State.Storage
{
    public class SessionStateStorage : IStringStateStorage
    {
        private ProtectedSessionStorage SessionStorage { get; }

        public SessionStateStorage(ProtectedSessionStorage sessionStorage)
        {
            SessionStorage = sessionStorage;
        }

        public async ValueTask<string> GetStateJsonAsync(string statename)
        {
            var result = await SessionStorage.GetAsync<string>(statename);
            return result.Value ?? string.Empty;
        }

        public async ValueTask StoreStateJsonAsync(string statename, string json)
        {
            await SessionStorage.SetAsync(statename, json);
        }
    }
}
