using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using TimeZoneConverter;

namespace Infra.Shared.Extensions
{
    public static class StringExtensions
    {
        public static string ToMd5(this string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;

            var loweredBytes = Encoding.Default.GetBytes(s.ToLower());
            var buffer = new MD5CryptoServiceProvider().ComputeHash(loweredBytes);
            var sb = new StringBuilder(buffer.Length * 2);
            for (var i = 0; i < buffer.Length; i++)
            {
                sb.Append(buffer[i].ToString("X2"));
            }

            return sb.ToString().ToLower();
        }

        public static string EncryptString(this string text, string keyString)
        {
            var key = Encoding.UTF8.GetBytes(keyString);

            using (var aesAlg = Aes.Create())
            {
                using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }

                        var iv = aesAlg.IV;

                        var decryptedContent = msEncrypt.ToArray();

                        var result = new byte[iv.Length + decryptedContent.Length];

                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                        return Convert.ToBase64String(result);
                    }
                }
            }
        }

        public static string DecryptString(this string cipherText, string keyString)
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            var iv = new byte[16];
            var cipher = new byte[fullCipher.Length - iv.Length];


            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, fullCipher.Length - iv.Length);

            var key = Encoding.UTF8.GetBytes(keyString);

            using (var aesAlg = Aes.Create())
            {
                using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                {
                    string result;
                    using (var msDecrypt = new MemoryStream(cipher))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                result = srDecrypt.ReadToEnd();
                            }
                        }
                    }

                    return result;
                }
            }
        }

        public static bool ContainsExact(this string text, string toCheck,
            RegexOptions options = RegexOptions.IgnoreCase)
        {
            return Regex.IsMatch(text, $@"\b{toCheck}\b", options);
        }

        public static string InsertSpace(this string text)
        {
            string[] words = Regex.Matches(
                    text,
                    "(^[a-z]+|[A-Z]+(?![a-z])|[A-Z][a-z]+)")
                .OfType<Match>()
                .Select(m => m.Value)
                .ToArray();
            return string.Join(" ", words);
        }

        public static DateTime ConvertToUtcDateTime(this string strDateTime, string timezoneId)
        {
            var dateTimeUnspec = DateTime.SpecifyKind(DateTime.Parse(strDateTime), DateTimeKind.Unspecified);

            return TimeZoneInfo.ConvertTimeToUtc(dateTimeUnspec, TZConvert.GetTimeZoneInfo(timezoneId));
        }

        public static string RemoveWhiteSpaces(this string text)
        {
            return text?.Trim()
                .Replace(" ", "")
                .Replace("‌", ""); // second char is mid-space in persian lang
        }

        public static string[] SplitWithWhiteSpace(this string text)
        {
            return text?.Split(' ', '‌'); // second char is mid-space in persian lang
        }

        public static string ToSlug(this string inputString)
        {
            if (!string.IsNullOrWhiteSpace(inputString))
            {
                inputString = inputString.ToLower();
                inputString = inputString.Trim();
                inputString = CleanWhiteSpace(inputString, true);
                inputString = ApplyReplacements(inputString, new Dictionary<string, string>
                {
                    {" ", "-"}
                });
                inputString = RemoveDiacritics(inputString);
                inputString = DeleteCharacters(inputString, @"[^a-zA-Z0-9آ-ی\-\._]");
                inputString = Regex.Replace(inputString, "--+", "-");
            }

            return inputString;
        }

        private static string CleanWhiteSpace(string str, bool collapse)
        {
            return Regex.Replace(str, collapse ? @"\s+" : @"\s", " ");
        }

        private static string RemoveDiacritics(string str)
        {
            var stFormD = str.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        private static string ApplyReplacements(string str, Dictionary<string, string> replacements)
        {
            var sb = new StringBuilder(str);

            foreach (KeyValuePair<string, string> replacement in replacements)
            {
                sb = sb.Replace(replacement.Key, replacement.Value);
            }

            return sb.ToString();
        }

        private static string DeleteCharacters(string str, string regex)
        {
            return Regex.Replace(str, regex, "");
        }

        /// <summary>
        /// Attempts to pluralize the specified text according to the rules of the English language.
        /// </summary>
        /// <remarks>
        /// This function attempts to pluralize as many words as practical by following these rules:
        /// <list type="bullet">
        ///		<item><description>Words that don't follow any rules (e.g. "mouse" becomes "mice") are returned from a dictionary.</description></item>
        ///		<item><description>Words that end with "y" (but not with a vowel preceding the y) are pluralized by replacing the "y" with "ies".</description></item>
        ///		<item><description>Words that end with "us", "ss", "x", "ch" or "sh" are pluralized by adding "es" to the end of the text.</description></item>
        ///		<item><description>Words that end with "f" or "fe" are pluralized by replacing the "f(e)" with "ves".</description></item>
        ///	</list>
        /// </remarks>
        /// <param name="text">The text to pluralize.</param>
        /// <returns>A string that consists of the text in its pluralized form.</returns>
        public static string ToPluralString(this string text)
        {
            // Create a dictionary of exceptions that have to be checked first
            // This is very much not an exhaustive list!
            Dictionary<string, string> exceptions = new Dictionary<string, string>()
            {
                {"man", "men"},
                {"woman", "women"},
                {"child", "children"},
                {"tooth", "teeth"},
                {"foot", "feet"},
                {"mouse", "mice"},
                {"belief", "beliefs"}
            };

            if (exceptions.ContainsKey(text.ToLowerInvariant()))
            {
                return exceptions[text.ToLowerInvariant()];
            }
            else if (text.EndsWith("y", StringComparison.OrdinalIgnoreCase) &&
                     !text.EndsWith("ay", StringComparison.OrdinalIgnoreCase) &&
                     !text.EndsWith("ey", StringComparison.OrdinalIgnoreCase) &&
                     !text.EndsWith("iy", StringComparison.OrdinalIgnoreCase) &&
                     !text.EndsWith("oy", StringComparison.OrdinalIgnoreCase) &&
                     !text.EndsWith("uy", StringComparison.OrdinalIgnoreCase))
            {
                return text.Substring(0, text.Length - 1) + "ies";
            }
            else if (text.EndsWith("us", StringComparison.InvariantCultureIgnoreCase))
            {
                // http://en.wikipedia.org/wiki/Plural_form_of_words_ending_in_-us
                return text + "es";
            }
            else if (text.EndsWith("ss", StringComparison.InvariantCultureIgnoreCase))
            {
                return text + "es";
            }
            else if (text.EndsWith("s", StringComparison.InvariantCultureIgnoreCase))
            {
                return text;
            }
            else if (text.EndsWith("x", StringComparison.InvariantCultureIgnoreCase) ||
                     text.EndsWith("ch", StringComparison.InvariantCultureIgnoreCase) ||
                     text.EndsWith("sh", StringComparison.InvariantCultureIgnoreCase))
            {
                return text + "es";
            }
            else if (text.EndsWith("f", StringComparison.InvariantCultureIgnoreCase) && text.Length > 1)
            {
                return text.Substring(0, text.Length - 1) + "ves";
            }
            else if (text.EndsWith("fe", StringComparison.InvariantCultureIgnoreCase) && text.Length > 2)
            {
                return text.Substring(0, text.Length - 2) + "ves";
            }
            else
            {
                return text + "s";
            }
        }

        /// <summary>
        ///     Checks if date with dateFormat is parse-able to System.DateTime format returns boolean value if true else false
        /// </summary>
        /// <param name="data">String date</param>
        /// <param name="dateFormat">date format example dd/MM/yyyy HH:mm:ss</param>
        /// <returns>boolean True False if is valid System.DateTime</returns>
        public static bool IsDateTime(this string data, string dateFormat)
        {
            // ReSharper disable once RedundantAssignment
            DateTime dateVal = default(DateTime);
            return DateTime.TryParseExact(data, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out dateVal);
        }

        /// <summary>
        ///     Converts the string representation of a number to its 32-bit signed integer equivalent
        /// </summary>
        /// <param name="value">string containing a number to convert</param>
        /// <returns>System.Int32</returns>
        /// <remarks>
        ///     The conversion fails if the string parameter is null, is not of the correct format, or represents a number
        ///     less than System.Int32.MinValue or greater than System.Int32.MaxValue
        /// </remarks>
        public static int ToInt32(this string value)
        {
            int number;
            Int32.TryParse(value, out number);
            return number;
        }

        /// <summary>
        ///     Converts the string representation of a number to its 64-bit signed integer equivalent
        /// </summary>
        /// <param name="value">string containing a number to convert</param>
        /// <returns>System.Int64</returns>
        /// <remarks>
        ///     The conversion fails if the string parameter is null, is not of the correct format, or represents a number
        ///     less than System.Int64.MinValue or greater than System.Int64.MaxValue
        /// </remarks>
        public static long ToInt64(this string value)
        {
            long number;
            Int64.TryParse(value, out number);
            return number;
        }

        /// <summary>
        ///     Converts the string representation of a number to its 16-bit signed integer equivalent
        /// </summary>
        /// <param name="value">string containing a number to convert</param>
        /// <returns>System.Int16</returns>
        /// <remarks>
        ///     The conversion fails if the string parameter is null, is not of the correct format, or represents a number
        ///     less than System.Int16.MinValue or greater than System.Int16.MaxValue
        /// </remarks>
        public static short ToInt16(this string value)
        {
            short number;
            Int16.TryParse(value, out number);
            return number;
        }

        /// <summary>
        ///     Converts the string representation of a number to its System.Decimal equivalent
        /// </summary>
        /// <param name="value">string containing a number to convert</param>
        /// <returns>System.Decimal</returns>
        /// <remarks>
        ///     The conversion fails if the s parameter is null, is not a number in a valid format, or represents a number
        ///     less than System.Decimal.MinValue or greater than System.Decimal.MaxValue
        /// </remarks>
        public static Decimal ToDecimal(this string value)
        {
            Decimal number;
            Decimal.TryParse(value, out number);
            return number;
        }

        /// <summary>
        ///     Converts string to its boolean equivalent
        /// </summary>
        /// <param name="value">string to convert</param>
        /// <returns>boolean equivalent</returns>
        /// <remarks>
        ///     <exception cref="ArgumentException">
        ///         thrown in the event no boolean equivalent found or an empty or whitespace
        ///         string is passed
        ///     </exception>
        /// </remarks>
        public static bool ToBoolean(this string value)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("value");
            }

            string val = value.ToLower().Trim();
            switch (val)
            {
                case "false":
                    return false;
                case "f":
                    return false;
                case "true":
                    return true;
                case "t":
                    return true;
                case "yes":
                    return true;
                case "no":
                    return false;
                case "y":
                    return true;
                case "n":
                    return false;
                default:
                    throw new ArgumentException("Invalid boolean");
            }
        }

        /// <summary>
        ///     Gets empty String if passed value is of type Null/Nothing
        /// </summary>
        /// <param name="val">val</param>
        /// <returns>System.String</returns>
        /// <remarks></remarks>
        public static string GetEmptyStringIfNull(this string val)
        {
            return (val != null ? val.Trim() : "");
        }

        /// <summary>
        ///     Checks if a string is null and returns String if not Empty else returns null/Nothing
        /// </summary>
        /// <param name="myValue">String value</param>
        /// <returns>null/nothing if String IsEmpty</returns>
        /// <remarks></remarks>
        public static string GetNullIfEmptyString(this string myValue)
        {
            if (myValue == null || myValue.Length <= 0)
            {
                return null;
            }

            myValue = myValue.Trim();
            if (myValue.Length > 0)
            {
                return myValue;
            }

            return null;
        }

        /// <summary>
        ///     IsInteger Function checks if a string is a valid int32 value
        /// </summary>
        /// <param name="val">val</param>
        /// <returns>Boolean True if isInteger else False</returns>
        public static bool IsInteger(this string val)
        {
            // Variable to collect the Return value of the TryParse method.

            // Define variable to collect out parameter of the TryParse method. If the conversion fails, the out parameter is zero.
            int retNum;

            // The TryParse method converts a string in a specified style and culture-specific format to its double-precision floating point number equivalent.
            // The TryParse method does not generate an exception if the conversion fails. If the conversion passes, True is returned. If it does not, False is returned.
            bool isNum = Int32.TryParse(val, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        /// <summary>
        ///     Read in a sequence of words from standard input and capitalize each
        ///     one (make first letter uppercase; make rest lowercase).
        /// </summary>
        /// <param name="s">string</param>
        /// <returns>Word with capitalization</returns>
        public static string Capitalize(this string s)
        {
            if (s.Length == 0)
            {
                return s;
            }

            return s.Substring(0, 1).ToUpper() + s.Substring(1).ToLower();
        }

        /// <summary>
        ///     Validate email address
        /// </summary>
        /// <param name="email">string email address</param>
        /// <returns>true or false if email if valid</returns>
        public static bool IsEmailAddress(this string email)
        {
            string pattern =
                "^[a-zA-Z][\\w\\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\\w\\.-]*[a-zA-Z0-9]\\.[a-zA-Z][a-zA-Z\\.]*[a-zA-Z]$";
            return Regex.Match(email, pattern).Success;
        }

        /// <summary>
        ///     IsNumeric checks if a string is a valid floating value
        /// </summary>
        /// <param name="val"></param>
        /// <returns>Boolean True if isNumeric else False</returns>
        /// <remarks></remarks>
        public static bool IsNumeric(this string val)
        {
            // Variable to collect the Return value of the TryParse method.

            // Define variable to collect out parameter of the TryParse method. If the conversion fails, the out parameter is zero.
            double retNum;

            // The TryParse method converts a string in a specified style and culture-specific format to its double-precision floating point number equivalent.
            // The TryParse method does not generate an exception if the conversion fails. If the conversion passes, True is returned. If it does not, False is returned.
            bool isNum = Double.TryParse(val, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        /// <summary>
        ///     Truncate String and append ... at end
        /// </summary>
        /// <param name="s">String to be truncated</param>
        /// <param name="maxLength">number of chars to truncate</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string Truncate(this string s, int maxLength)
        {
            if (String.IsNullOrEmpty(s) || maxLength <= 0)
            {
                return String.Empty;
            }

            if (s.Length > maxLength)
            {
                return s.Substring(0, maxLength) + "...";
            }

            return s;
        }

        /// <summary>
        ///     Function returns a default String value if given value is null or empty
        /// </summary>
        /// <param name="myValue">String value to check if isEmpty</param>
        /// <param name="defaultValue">default value to return if String value isEmpty</param>
        /// <returns>returns either String value or default value if IsEmpty</returns>
        /// <remarks></remarks>
        public static string GetDefaultIfEmpty(this string myValue, string defaultValue)
        {
            if (!String.IsNullOrEmpty(myValue))
            {
                myValue = myValue.Trim();
                return myValue.Length > 0 ? myValue : defaultValue;
            }

            return defaultValue;
        }

        /// <summary>
        ///     Convert a string to its equivalent byte array
        /// </summary>
        /// <param name="val">string to convert</param>
        /// <returns>System.byte array</returns>
        public static byte[] ToBytes(this string val)
        {
            var bytes = new byte[val.Length * sizeof(char)];
            Buffer.BlockCopy(val.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>
        ///     Reverse string
        /// </summary>
        /// <param name="val">string to reverse</param>
        /// <returns>System.string</returns>
        public static string Reverse(this string val)
        {
            var chars = new char[val.Length];
            for (int i = val.Length - 1, j = 0; i >= 0; --i, ++j)
            {
                chars[j] = val[i];
            }

            val = new String(chars);
            return val;
        }

        /// <summary>
        ///     Encrypt a string using the supplied key. Encoding is done using RSA encryption.
        /// </summary>
        /// <param name="stringToEncrypt">String that must be encrypted.</param>
        /// <param name="key">Encryption key</param>
        /// <returns>A string representing a byte array separated by a minus sign.</returns>
        /// <exception cref="ArgumentException">Occurs when stringToEncrypt or key is null or empty.</exception>
        public static string Encrypt(this string stringToEncrypt, string key)
        {
            var cspParameter = new CspParameters {KeyContainerName = key};
            var rsaServiceProvider = new RSACryptoServiceProvider(cspParameter) {PersistKeyInCsp = true};
            byte[] bytes = rsaServiceProvider.Encrypt(Encoding.UTF8.GetBytes(stringToEncrypt), true);
            return BitConverter.ToString(bytes);
        }


        /// <summary>
        ///     Decrypt a string using the supplied key. Decoding is done using RSA encryption.
        /// </summary>
        /// <param name="stringToDecrypt">String that must be decrypted.</param>
        /// <param name="key">Decryption key.</param>
        /// <returns>The decrypted string or null if decryption failed.</returns>
        /// <exception cref="ArgumentException">Occurs when stringToDecrypt or key is null or empty.</exception>
        public static string Decrypt(this string stringToDecrypt, string key)
        {
            var cspParamters = new CspParameters {KeyContainerName = key};
            var rsaServiceProvider = new RSACryptoServiceProvider(cspParamters) {PersistKeyInCsp = true};
            string[] decryptArray = stringToDecrypt.Split(new[] {"-"}, StringSplitOptions.None);
            byte[] decryptByteArray = Array.ConvertAll(decryptArray,
                (s => Convert.ToByte(byte.Parse(s, NumberStyles.HexNumber))));
            byte[] bytes = rsaServiceProvider.Decrypt(decryptByteArray, true);
            string result = Encoding.UTF8.GetString(bytes);
            return result;
        }

        /// <summary>
        ///     Convert string to Hash using Sha512
        /// </summary>
        /// <param name="val">string to hash</param>
        /// <returns>Hashed string</returns>
        /// <exception cref="ArgumentException"></exception>
        public static string CreateHashSha512(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                throw new ArgumentException("val");
            }

            var sb = new StringBuilder();
            using (SHA512 hash = SHA512.Create())
            {
                byte[] data = hash.ComputeHash(val.ToBytes());
                foreach (byte b in data)
                {
                    sb.Append(b.ToString("x2"));
                }
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Convert string to Hash using Sha256
        /// </summary>
        /// <param name="val">string to hash</param>
        /// <returns>Hashed string</returns>
        public static string CreateHashSha256(string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                throw new ArgumentException("val");
            }

            var sb = new StringBuilder();
            using (SHA256 hash = SHA256.Create())
            {
                byte[] data = hash.ComputeHash(val.ToBytes());
                foreach (byte b in data)
                {
                    sb.Append(b.ToString("x2"));
                }
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Convert url query string to IDictionary value key pair
        /// </summary>
        /// <param name="queryString">query string value</param>
        /// <returns>IDictionary value key pair</returns>
        public static IDictionary<string, string> QueryStringToDictionary(this string queryString)
        {
            if (string.IsNullOrWhiteSpace(queryString))
            {
                return null;
            }

            if (!queryString.Contains("?"))
            {
                return null;
            }

            string query = queryString.Replace("?", "");
            if (!query.Contains("="))
            {
                return null;
            }

            return query.Split('&').Select(p => p.Split('=')).ToDictionary(
                key => key[0].ToLower().Trim(), value => value[1]);
        }

        /// <summary>
        ///     Reverse back or forward slashes
        /// </summary>
        /// <param name="val">string</param>
        /// <param name="direction">
        ///     0 - replace forward slash with back
        ///     1 - replace back with forward slash
        /// </param>
        /// <returns></returns>
        public static string ReverseSlash(this string val, int direction)
        {
            switch (direction)
            {
                case 0:
                    return val.Replace(@"/", @"\");
                case 1:
                    return val.Replace(@"\", @"/");
                default:
                    return val;
            }
        }

        /// <summary>
        ///     Validates if a string is valid IPv4
        ///     Regular expression taken from <a href="http://regexlib.com/REDetails.aspx?regexp_id=2035">Regex reference</a>
        /// </summary>
        /// <param name="val">string IP address</param>
        /// <returns>true if string matches valid IP address else false</returns>
        public static bool IsValidIPv4(this string val)
        {
            if (string.IsNullOrEmpty(val))
            {
                return false;
            }

            return Regex.Match(val,
                    @"(?:^|\s)([a-z]{3,6}(?=://))?(://)?((?:25[0-5]|2[0-4]\d|[01]?\d\d?)\.(?:25[0-5]|2[0-4]\d|[01]?\d\d?)\.(?:25[0-5]|2[0-4]\d|[01]?\d\d?)\.(?:25[0-5]|2[0-4]\d|[01]?\d\d?))(?::(\d{2,5}))?(?:\s|$)")
                .Success;
        }

        /// <summary>
        ///     Extracts the left part of the input string limited with the length parameter
        /// </summary>
        /// <param name="val">The input string to take the left part from</param>
        /// <param name="length">The total number characters to take from the input string</param>
        /// <returns>The substring starting at startIndex 0 until length</returns>
        /// <exception cref="System.ArgumentNullException">input is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Length is smaller than zero or higher than the length of input</exception>
        public static string Left(this string val, int length)
        {
            if (string.IsNullOrEmpty(val))
            {
                throw new ArgumentNullException("val");
            }

            if (length < 0 || length > val.Length)
            {
                throw new ArgumentOutOfRangeException("length",
                    "length cannot be higher than total string length or less than 0");
            }

            return val.Substring(0, length);
        }

        /// <summary>
        ///     Extracts the right part of the input string limited with the length parameter
        /// </summary>
        /// <param name="val">The input string to take the right part from</param>
        /// <param name="length">The total number characters to take from the input string</param>
        /// <returns>The substring taken from the input string</returns>
        /// <exception cref="System.ArgumentNullException">input is null</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Length is smaller than zero or higher than the length of input</exception>
        public static string Right(this string val, int length)
        {
            if (string.IsNullOrEmpty(val))
            {
                throw new ArgumentNullException("val");
            }

            if (length < 0 || length > val.Length)
            {
                throw new ArgumentOutOfRangeException("length",
                    "length cannot be higher than total string length or less than 0");
            }

            return val.Substring(val.Length - length);
        }

        /// <summary>
        ///     Checks if a string is null
        /// </summary>
        /// <param name="val">string to evaluate</param>
        /// <returns>true if string is null else false</returns>
        public static bool IsNull(this string val)
        {
            return val == null;
        }

        /// <summary>
        ///     Checks if a string is null or empty
        /// </summary>
        /// <param name="val">string to evaluate</param>
        /// <returns>true if string is null or is empty else false</returns>
        public static bool IsNullOrEmpty(this string val)
        {
            return String.IsNullOrEmpty(val);
        }

        /// <summary>
        ///     Checks if string length is a certain minimum number of characters, does not ignore leading and trailing
        ///     white-space.
        ///     null strings will always evaluate to false.
        /// </summary>
        /// <param name="val">string to evaluate minimum length</param>
        /// <param name="minCharLength">minimum allowable string length</param>
        /// <returns>true if string is of specified minimum length</returns>
        public static bool IsMinLength(this string val, int minCharLength)
        {
            return val != null && val.Length >= minCharLength;
        }

        /// <summary>
        ///     Checks if string length is consists of specified allowable maximum char length. does not ignore leading and
        ///     trailing white-space.
        ///     null strings will always evaluate to false.
        /// </summary>
        /// <param name="val">string to evaluate maximum length</param>
        /// <param name="maxCharLength">maximum allowable string length</param>
        /// <returns>true if string has specified maximum char length</returns>
        public static bool IsMaxLength(this string val, int maxCharLength)
        {
            return val != null && val.Length <= maxCharLength;
        }

        public static string JoinAndNullIgnore(this string val, params string[] values)
        {
            var appendedValue = values.Append(val);
            return string.Join('-', appendedValue.Where(x => !x.IsNullOrEmpty()));
        }

        public static T[] ToEnums<T>(this string delimitedValues)
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException();
            }

            if (delimitedValues == string.Empty)
            {
                return new T[0];
            }

            string[] parts = delimitedValues.Split(',');
            T[] converted = Array.ConvertAll(parts, x => (T) Enum.Parse(typeof(T), x));
            return converted;
        }

        public static string EnumsToString<TEnum>(this IEnumerable<TEnum> source) where TEnum : struct
        {
            if (!typeof(TEnum).IsEnum)
                throw new InvalidOperationException("TEnum must be an enumeration type. ");

            if (source == null)
                throw new ArgumentNullException();

            var strings = source.Where(e => Enum.IsDefined(typeof(TEnum), e))
                .Select(n => Enum.GetName(typeof(TEnum), n));
            return string.Join(",", strings);
        }
    }
}