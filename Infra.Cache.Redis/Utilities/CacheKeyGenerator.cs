using System.Security.Cryptography;
using System.Text;

namespace Infra.Cache.Redis.Utilities
{
    public static class CacheKeyGenerator
    {
        public static string GenerateProductsKey(string input)
        {
            return HashKeyGenerator(string.Format(Models.Constants.Products, input));
        }
        
        public static string GenerateStoreMenuKey()
        {
            return Models.Constants.StoreMenu;
        }

        private static string HashKeyGenerator(string key)
        {
            var r = new StringBuilder();
            using var md5 = MD5.Create();
            var buff = md5.ComputeHash(Encoding.UTF8.GetBytes(key));
            foreach (var b in buff)
                r.Append(b.ToString("x2"));
            return r.ToString();
        }

    }
}
