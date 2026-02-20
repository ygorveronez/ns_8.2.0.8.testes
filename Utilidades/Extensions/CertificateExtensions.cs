using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Security.Cryptography.X509Certificates
{
    public static class CertificateExtensions
    {
        public static string ObterCnpj(this X509Certificate2 certificado)
        {
            string cnpj = string.Empty;
            foreach (X509Extension extension in certificado.Extensions)
            {
                string s1 = extension.Format(true);
                string[] lines = s1.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (!lines[i].Trim().StartsWith("2.16.76.1.3.3")) continue;//ICP-BRASIL Pessoa Juridica and Equipment
                    string value = lines[i].Substring(lines[i].IndexOf('=') + 1);
                    string[] elements = value.Split(' ');
                    byte[] cnpjBytes = new byte[14];
                    for (int j = 0; j < cnpjBytes.Length; j++)
                        cnpjBytes[j] = Convert.ToByte(elements[j + 2], 16);
                    cnpj = Encoding.UTF8.GetString(cnpjBytes);
                    break;
                }
                if (!string.IsNullOrEmpty(cnpj))
                    break;
            }

            if (string.IsNullOrEmpty(cnpj))
            {
                var match = Regex.Match(certificado.Subject, @"CN=.*?:\s*(\d{14})");
                return match.Success ? match.Groups[1].Value.Substring(0, 8) : string.Empty;
            }

            return !string.IsNullOrWhiteSpace(cnpj) && cnpj.Length > 0 ? cnpj.Substring(0, 8) : string.Empty;
        }

        public static string ObterCpf(this X509Certificate2 certificado)
        {
            string cpf = string.Empty;
            foreach (X509Extension extension in certificado.Extensions)
            {
                string s1 = extension.Format(true);
                string[] lines = s1.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (!lines[i].Trim().StartsWith("2.16.76.1.3.1")) continue;//ICP-BRASIL Pessoa Fisica
                    string value = lines[i].Substring(lines[i].IndexOf('=') + 1);
                    string[] elements = value.Split(' ');
                    byte[] cpfBytes = new byte[11];
                    for (int j = 0; j < cpfBytes.Length; j++)
                        cpfBytes[j] = Convert.ToByte(elements[j + 10], 16);
                    cpf = Encoding.UTF8.GetString(cpfBytes);
                    break;
                }
                if (!string.IsNullOrEmpty(cpf))
                    break;
            }

            return cpf;
        }
    }
}
