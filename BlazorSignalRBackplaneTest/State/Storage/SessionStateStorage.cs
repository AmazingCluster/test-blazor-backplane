using System.Security.Cryptography;
using System.Text;

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
            var result = await SessionStorage.GetAsync<string>(GetHashedName(statename));
            return result.Value ?? string.Empty;
        }

        public async ValueTask StoreStateJsonAsync(string statename, string json)
        {
            await SessionStorage.SetAsync(GetHashedName(statename), json);
        }

        /// Hashing is done purely to mask the state names, as they include the entire namespace
        private static string GetHashedName(string name)
        {
            byte[] hashBytes = MD5.HashData(Encoding.UTF8.GetBytes(name));
            return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
        }
    }
}
