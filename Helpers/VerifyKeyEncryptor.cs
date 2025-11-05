using System.Security.Cryptography;
using System.Text;

namespace JobExchangeMvc.Helpers;

/// <summary>
/// Helper class for encrypting and decrypting VerifyKey
/// </summary>
public static class VerifyKeyEncryptor
{
    /// <summary>
    /// Encrypt VerifyKey using AES encryption
    /// </summary>
    public static string Encrypt(string plainText, string key)
    {
        if (string.IsNullOrEmpty(plainText))
            throw new ArgumentNullException(nameof(plainText));

        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        byte[] iv = new byte[16];
        byte[] array;

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(PadKey(key));
            aes.IV = iv;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                    {
                        streamWriter.Write(plainText);
                    }

                    array = memoryStream.ToArray();
                }
            }
        }

        return Convert.ToBase64String(array);
    }

    /// <summary>
    /// Decrypt VerifyKey using AES decryption
    /// </summary>
    public static string Decrypt(string cipherText, string key)
    {
        if (string.IsNullOrEmpty(cipherText))
            throw new ArgumentNullException(nameof(cipherText));

        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

        byte[] iv = new byte[16];
        byte[] buffer = Convert.FromBase64String(cipherText);

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(PadKey(key));
            aes.IV = iv;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream(buffer))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader streamReader = new StreamReader(cryptoStream))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Pad or truncate key to 32 bytes (256 bits for AES-256)
    /// </summary>
    private static string PadKey(string key)
    {
        if (key.Length >= 32)
            return key.Substring(0, 32);

        return key.PadRight(32, '0');
    }
}
