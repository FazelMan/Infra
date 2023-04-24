using System.Linq;
using HashidsNet;
using Infra.Shared.Helpers;

namespace Infra.Shared.Cryptography
{
    public class HashIdsHelper
    {
        public static string Encrypt(string template, int value)
        {
            var length = Host.Config["Cryptography:HashIds:" + template + ":Length"];

            var hashIdKey = Host.Config["Cryptography:HashIds:" + template + ":Key"];
            var hashIdLength = length != null ? int.Parse(length) : 0;
            var hashIdAcceptedAlphabet = Host.Config["Cryptography:HashIds:" + template + ":AcceptedAlphabet"];

            if (hashIdKey == null || hashIdLength == 0 || hashIdAcceptedAlphabet == null)
            {
                hashIdKey = Host.Config["Cryptography:HashIds:Default:Key"];
                hashIdLength = int.Parse(Host.Config["Cryptography:HashIds:Default:Length"]);
                hashIdAcceptedAlphabet = Host.Config["Cryptography:HashIds:Default:AcceptedAlphabet"];
            }

            var hashids = new Hashids(hashIdKey, hashIdLength, hashIdAcceptedAlphabet);
            return hashids.Encode(value);
        }

        public static int Decrypt(string template, string value)
        {
            var length = Host.Config["Cryptography:HashIds:" + template + ":Length"];

            var hashIdKey = Host.Config["Cryptography:HashIds:" + template + ":Key"];
            var hashIdLength = length != null ? int.Parse(length) : 0;
            var hashIdAcceptedAlphabet = Host.Config["Cryptography:HashIds:" + template + ":AcceptedAlphabet"];

            if (hashIdKey == null || hashIdLength == 0 || hashIdAcceptedAlphabet == null)
            {
                hashIdKey = Host.Config["Cryptography:HashIds:Default:Key"];
                hashIdLength = int.Parse(Host.Config["Cryptography:HashIds:Default:Length"]);
                hashIdAcceptedAlphabet =
                    Host.Config["Cryptography:HashIds:Default:AcceptedAlphabet"];
            }

            var hashids = new Hashids(hashIdKey, hashIdLength, hashIdAcceptedAlphabet);
            return hashids.Decode(value).FirstOrDefault();
        }
    }
}