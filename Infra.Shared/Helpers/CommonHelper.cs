using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Infra.Shared.Helpers
{
    public class CommonHelper
    {
        public static bool IsValidNationalId(string NationalId, byte id_Nationality = 1)
        {
            if (string.IsNullOrEmpty(NationalId) && id_Nationality == 2)
                return true;

            if (string.IsNullOrEmpty(NationalId))
                return true;

            if (NationalId.Length != 10 && id_Nationality == 1)
                return false;

            var regex = new Regex(@"\d{10}");
            if (!regex.IsMatch(NationalId) && id_Nationality == 1)
                return false;

            var allDigitEqual = new[] { "0000000000", "1111111111", "2222222222", "3333333333", "4444444444", "5555555555", "6666666666", "7777777777", "8888888888", "9999999999" };
            if (allDigitEqual.Contains(NationalId)) return false;

            if (NationalId.Length != 9 && id_Nationality != 1)
                return false;

            if (NationalId.Length == 9 && id_Nationality != 1)
                return true;

            var chArray = NationalId.ToCharArray();
            var num0 = Convert.ToInt32(chArray[0].ToString()) * 10;
            var num2 = Convert.ToInt32(chArray[1].ToString()) * 9;
            var num3 = Convert.ToInt32(chArray[2].ToString()) * 8;
            var num4 = Convert.ToInt32(chArray[3].ToString()) * 7;
            var num5 = Convert.ToInt32(chArray[4].ToString()) * 6;
            var num6 = Convert.ToInt32(chArray[5].ToString()) * 5;
            var num7 = Convert.ToInt32(chArray[6].ToString()) * 4;
            var num8 = Convert.ToInt32(chArray[7].ToString()) * 3;
            var num9 = Convert.ToInt32(chArray[8].ToString()) * 2;
            var a = Convert.ToInt32(chArray[9].ToString());

            var b = (((((((num0 + num2) + num3) + num4) + num5) + num6) + num7) + num8) + num9;
            var c = b % 11;

            return (((c < 2) && (a == c)) || ((c >= 2) && ((11 - c) == a)));
        }

        public static string GetEncrypted(string nationalId, string password)
        {
            nationalId = GetEnglishNumber(nationalId);

            password = $"{nationalId}#$%RGTJITY^%&$TE{password}";

            StringBuilder builder = new StringBuilder();

            using (MD5 md5 = MD5.Create())
            {
                byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < data.Length; i++)
                {
                    builder.Append(data[i].ToString("x2"));
                }
            }

            return builder.ToString();
        }
        public static string GetEnglishNumber(string data)
        {
            string englishNumber = "";

            foreach (char ch in data)
            {
                if ((ch >= '۰' && ch <= '۹') || (ch >= '٠' && ch <= '٩'))
                    englishNumber += char.GetNumericValue(ch);
                else
                    englishNumber += ch;
            }

            return englishNumber;
        }

        public static bool IsNumber(string testVar)
        {
            double myNum = 0;

            if (Double.TryParse(testVar, out myNum))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
