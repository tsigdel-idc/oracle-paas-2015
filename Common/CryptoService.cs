using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Configuration;

namespace IDC.Common
{
    public class CryptoService : ICryptoService
    {
        private static string _stdPwd = "$tdP4$$w0rD";
        private static byte[] _salt = new byte[] { 0x6e, 0x20, 0x61, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x49, 0x76, 0x65, 0x76, 0x76, 0x65, 0x64 };
        private const int _keySize = 256;
        private const int _blockSize = 128;

        private byte[] passwordBytes;
        private byte[] _key;
        private byte[] _iv;

        private static bool disabled = ConfigurationManager.AppSettings["EncryptionEnabled"] != bool.TrueString;

        public CryptoService(EncryptionPolicy policy)
        {
            _stdPwd = policy.Password;
            _salt = policy.Salt;
            InitService();
        }

        public CryptoService()
        {
            InitService();
        }

        public string Encrypt(string clearText)
        {
            if (string.IsNullOrEmpty(clearText)) return clearText;

            if (disabled) return clearText;

            byte[] clearBytes = Encoding.UTF8.GetBytes(clearText);
            byte[] encryptedData = Encrypt(clearBytes);
            return Convert.ToBase64String(encryptedData);
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return cipherText;

            if (disabled) return cipherText;

            try
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                byte[] decryptedData = Decrypt(cipherBytes);
                return Encoding.UTF8.GetString(decryptedData);
            }
            catch (Exception)
            {
                return cipherText;
            }
        }

        #region private methods

        private void InitService()
        {
            passwordBytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(_stdPwd));
            var key = new Rfc2898DeriveBytes(passwordBytes, _salt, 1000);
            _key = key.GetBytes(_keySize / 8);
            _iv = key.GetBytes(_blockSize / 8);
        }

        private byte[] Encrypt(byte[] clearData)
        {
            byte[] encryptedBytes = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (AesManaged alg = new AesManaged())
                {
                    alg.KeySize = _keySize;
                    alg.BlockSize = _blockSize;

                    alg.Key = _key;
                    alg.IV = _iv;

                    alg.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearData, 0, clearData.Length);
                        cs.Close();
                    }

                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }

        private byte[] Decrypt(byte[] cipherData)
        {
            byte[] decryptedBytes = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (AesManaged alg = new AesManaged())
                {
                    alg.KeySize = _keySize;
                    alg.BlockSize = _blockSize;

                    alg.Key = _key;
                    alg.IV = _iv;

                    alg.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherData, 0, cipherData.Length);
                        cs.Close();
                    }

                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }

        #endregion

        #region properties

        public static string Password
        {
            get { return _stdPwd; }
            set { _stdPwd = value; }
        }

        public static byte[] Salt
        {
            get { return _salt; }
            set { _salt = value; }
        }

        #endregion
    }

     public class EncryptionPolicy
     {
         public string Password { get; set; }
         public byte[] Salt { get; set; }
     }

    public class CryptoTest : ICryptoService
    {
        public string Encrypt(string clearText)
        {
            return clearText;
        }

        public string Decrypt(string cipherText)
        {
            return cipherText;
        }
    }

}