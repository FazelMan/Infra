namespace Infra.Shared.Cryptography
{
    public interface IEncryptionService
    {
        string EncryptQueryString(string inputText, string key, string salt);
        string DecryptQueryString(string inputText, string key, string salt);

        #region Hash by BCrypt
        string HashPasswordBCrypt(string inputText);
        bool VerifyBCrypt(string inputText, string passwordHash);
        #endregion
    }
}