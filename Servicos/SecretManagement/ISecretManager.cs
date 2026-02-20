using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.SecretManagement
{
    public interface ISecretManager
    {
        string GetSecretValue(string key);

        string GetUriValue();

        bool SendCertificate(string caminhoCertificado, string senhaCertificado, string nomeDoCertificadoNoKeyVault);

        System.Security.Cryptography.X509Certificates.X509Certificate2 GetCertificate(string nomeDoCertificadoNoKeyVault);

        bool DeleteCertificate(string nomeDoCertificadoNoKeyVault);
    }
}
