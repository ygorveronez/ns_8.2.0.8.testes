using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Servicos
{
    public class Criptografia : ServicoBase
    {
        public Criptografia(string stringConexao) : base(stringConexao) { }
        public static string Criptografar(string valor, string chave)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            Byte[] plainText = Encoding.Unicode.GetBytes(valor);
            Byte[] salt = Encoding.ASCII.GetBytes(chave);
            Rfc2898DeriveBytes secretKey = new Rfc2898DeriveBytes(chave, salt);
            ICryptoTransform encriptor = rijndaelCipher.CreateEncryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encriptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainText, 0, plainText.Length);
            cryptoStream.FlushFinalBlock();
            Byte[] cipherBytes = memoryStream.ToArray();
            cryptoStream.Close();
            string encryptedData = Convert.ToBase64String(cipherBytes);
            return encryptedData;
        }

        public static string Descriptografar(string valor, string chave)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            Byte[] encryptedData = Convert.FromBase64String(valor);
            Byte[] salt = Encoding.ASCII.GetBytes(chave);
            Rfc2898DeriveBytes secretKey = new Rfc2898DeriveBytes(chave, salt);
            ICryptoTransform decryptor = rijndaelCipher.CreateDecryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));
            MemoryStream memoryStream = new MemoryStream(encryptedData);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            Byte[] plainText = new Byte[encryptedData.Length - 1];
            int decryptedCount = cryptoStream.Read(plainText, 0, plainText.Length);
            cryptoStream.Close();
            string decryptedData = Encoding.Unicode.GetString(plainText, 0, decryptedCount);
            return decryptedData;
        }

        public static string DescriptografarDES(string valor, string senha)
        {
            if (String.IsNullOrEmpty(valor))
                throw new ArgumentNullException("A string não pode ser nula para ser descriptografada.");

            DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider();
            desProvider.Mode = CipherMode.ECB;
            desProvider.Key = Encoding.ASCII.GetBytes(senha);
            desProvider.Padding = PaddingMode.None;

            using (MemoryStream stream = new MemoryStream(FromHex(valor)))
            {
                using (CryptoStream cs = new CryptoStream(stream, desProvider.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs, Encoding.ASCII))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }

        public static byte[] FromHex(string hex)
        {
            hex = hex.Replace("-", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }

        public static string GerarHashMD5(string input)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }

    }
}