using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace NetAPIGrid.Service
{
    public class AES
    {

        private readonly IConfiguration _configuration;

        private readonly byte[] _key;
        private readonly byte[] _iv;

        public AES(IConfiguration configuration)
        {
            _configuration = configuration;
            _key = Convert.FromBase64String(_configuration["AES:AESKey"]);
            _iv = Convert.FromBase64String(_configuration["AES:AESIV"]);

            if (_key.Length != 16)
            {
                throw new ArgumentException("AES key must be 16 bytes (128 bits) for AES-128.");
            }

            if (_iv.Length != 16)
            {
                throw new ArgumentException("AES IV must be 16 bytes (128 bits).");
            }

        }

        // Encrypt 
        public string Encrypt(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.KeySize = 128;
                aesAlg.Key = _key;
                aesAlg.IV = _iv;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        // Decrypt 
        public string Decrypt(string cipherText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.KeySize = 128;
                aesAlg.Key = _key;
                aesAlg.IV = _iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

    }
}
