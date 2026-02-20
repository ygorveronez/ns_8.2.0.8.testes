using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Utilidades
{
    public class Calc
    {
        public static int Modulo11(string param)
        {
            // Cálculo do módulo 11.
            // DV = Digito verificador.

            // O peso é o multiplicador da expressão, deve ser somente de 2 à 9, então já iniciamos com 2.
            int peso = 2;
            // Somatória do resultado.
            int soma = 0;

            try
            {
                // Passa número a número da chave pegando da direita pra esquerda (para isso o Reverse()).
                param.ToCharArray()
                    .Reverse()
                    .ToList()
                    .ForEach(f =>
                    {
                        // Acumula valores da soma gerada das multiplicações (peso).
                        soma += (Convert.ToInt32(f.ToString()) * peso);
                        // Como o peso pode ir somente até 9 é feito essa validação.
                        peso = (peso == 9) ? 2 : peso + 1;
                    });

                int resto = (11 - (soma % 11));

                // Só permite dígito de 0 a 9
                if (resto > 9)
                    resto = 0;

                return resto;
            }
            catch
            {
                throw new Exception("Módulo 11: O parâmetro deve conter apenas números.");
            }
        }

        public static string ObterBase64Sha1DeString(string s)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(s);

            var sha1 = SHA1.Create();
            var hashBytes = sha1.ComputeHash(bytes);

            return Convert.ToBase64String(hashBytes);
        }

        public static string HmacSHA256(string key, string data)
        {
            //HMAC(Hash-based message authentication code)

            string hash;
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] code = encoder.GetBytes(key);
            using (HMACSHA256 hmac = new HMACSHA256(code))
            {
                byte[] hmBytes = hmac.ComputeHash(encoder.GetBytes(data));
                hash = ToHexString(hmBytes);
            }
            return hash;
        }

        public static string ToHexString(byte[] array)
        {
            StringBuilder hex = new StringBuilder(array.Length * 2);
            foreach (byte b in array)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        public static string OaepSHA512(string password, string directoryCert)
        {
            var passwordByte = System.Text.Encoding.UTF8.GetBytes(password);


            var certificate = new X509Certificate2(directoryCert);

            var rsa = certificate.GetRSAPublicKey();

            byte[] encrypted = rsa.Encrypt(passwordByte, RSAEncryptionPadding.OaepSHA512);

            string passwordEncrypted = Convert.ToBase64String(encrypted);

            return passwordEncrypted;
        }
    }
}
