using System;
using System.Security.Cryptography;

namespace Infra.Shared.Helpers
{
    public static class CryptographyHelper
    {
        public static string GenerateRandomCryptographicKey()
        {
            RNGCryptoServiceProvider rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            byte[] randomBytes = new byte[32];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
