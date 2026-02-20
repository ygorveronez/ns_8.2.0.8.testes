using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Servicos
{
    public class Criptografia : ServicoBase
    {
        public Criptografia(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        public static string Criptografar(string valor, string chave, bool URL = false)
        {
            byte[] plainText = Encoding.Unicode.GetBytes(valor);
            byte[] salt = Encoding.ASCII.GetBytes(chave);

            Rfc2898DeriveBytes secretKey = new Rfc2898DeriveBytes(chave, salt);

            using (RijndaelManaged rijndaelCipher = new RijndaelManaged())
            {
                ICryptoTransform encriptor = rijndaelCipher.CreateEncryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));

                using (MemoryStream memoryStream = new MemoryStream())
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encriptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainText, 0, plainText.Length);
                    cryptoStream.FlushFinalBlock();

                    byte[] cipherBytes = memoryStream.ToArray();

                    return URL ? Base64UrlEncode(cipherBytes) : Convert.ToBase64String(cipherBytes);
                }
            }
        }

        public static string Descriptografar(string valor, string chave, bool URL = false)
        {
            byte[] encryptedData = URL ? Base64UrlDecode(valor) : Convert.FromBase64String(valor);
            byte[] salt = Encoding.ASCII.GetBytes(chave);

            Rfc2898DeriveBytes secretKey = new Rfc2898DeriveBytes(chave, salt);

            using (RijndaelManaged rijndaelCipher = new RijndaelManaged())
            {
                ICryptoTransform decryptor = rijndaelCipher.CreateDecryptor(secretKey.GetBytes(32), secretKey.GetBytes(16));

                using (MemoryStream memoryStream = new MemoryStream(encryptedData))
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                using (MemoryStream plainTextStream = new MemoryStream())
                {
                    cryptoStream.CopyTo(plainTextStream);

                    byte[] plainTextBytes = plainTextStream.ToArray();

                    return Encoding.Unicode.GetString(plainTextBytes, 0, plainTextBytes.Length);
                }
            }
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

        public static string GerarHashSHA256(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hash = sha256.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < hash.Length; i++)
                    sb.Append(hash[i].ToString("X2"));

                return sb.ToString();
            }
        }

        public static string Base64UrlEncode(byte[] bytes)
        {
            string base64 = Convert.ToBase64String(bytes);
            base64 = base64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
            return base64;
        }

        public static byte[] Base64UrlDecode(string base64Url)
        {
            string base64 = base64Url.Replace('-', '+').Replace('_', '/');

            // Adicione o preenchimento se necessário
            while (base64.Length % 4 != 0)
            {
                base64 += "=";
            }

            byte[] bytes = Convert.FromBase64String(base64);
            return bytes;
        }
    }
}