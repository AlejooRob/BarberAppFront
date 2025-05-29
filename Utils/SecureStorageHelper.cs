using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberAppFront.Utils
{
    public static class SecureStorageHelper
    {
        private const string JwtTokenKey = "jwt_token";

        public static async Task SaveTokenAsync(string token)
        {
            await SecureStorage.SetAsync(JwtTokenKey, token);
        }

        public static async Task<string> GetTokenAsync()
        {
            return await SecureStorage.GetAsync(JwtTokenKey);
        }

        public static void RemoveToken()
        {
            SecureStorage.Remove(JwtTokenKey);
        }
    }
}
