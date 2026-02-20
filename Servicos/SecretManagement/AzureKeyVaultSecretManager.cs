using Azure.Identity;
using Dominio.Excecoes.Embarcador;
using System;
using System.Net;
using Azure.Security.KeyVault.Certificates;

namespace Servicos.SecretManagement
{
    public class AzureKeyVaultSecretManager : ISecretManager
    {
        private const string _azureKeyVaultUriEnvironmentVariable = "AZURE_KEYVAULT_URI";
        private readonly string _keyVaultUri;
        private readonly string _tenantId;
        private readonly string _clientSecret;
        private readonly string _clientId;

        public AzureKeyVaultSecretManager()
        {
            _keyVaultUri = Environment.GetEnvironmentVariable(_azureKeyVaultUriEnvironmentVariable, EnvironmentVariableTarget.Machine);

            if (string.IsNullOrWhiteSpace(_keyVaultUri))
                _keyVaultUri = Environment.GetEnvironmentVariable(_azureKeyVaultUriEnvironmentVariable, EnvironmentVariableTarget.Process);

            if (string.IsNullOrWhiteSpace(_keyVaultUri))
                throw new CustomException($"Environment variable '{_azureKeyVaultUriEnvironmentVariable}' not defined.");
        }

        public AzureKeyVaultSecretManager(string keyVaultUri)
        {
            _keyVaultUri = keyVaultUri;
        }

        public AzureKeyVaultSecretManager(string keyVaultUri, string tenantId, string clientId, string clientSecret)
        {
            _keyVaultUri = keyVaultUri;
            _tenantId = tenantId;
            _clientSecret = clientSecret;
            _clientId = clientId;
        }

        public string GetUriValue()
        {
            return _keyVaultUri;
        }
        public string GetSecretValue(string key)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Uri uri = new(_keyVaultUri);
            Azure.Security.KeyVault.Secrets.SecretClient client;

            if (string.IsNullOrWhiteSpace(_clientId) ||
                string.IsNullOrWhiteSpace(_tenantId) ||
                string.IsNullOrWhiteSpace(_clientSecret))
                client = new(uri, new Azure.Identity.DefaultAzureCredential());
            else
                client = new(uri, new Azure.Identity.ClientSecretCredential(tenantId: _tenantId, clientId: _clientId, clientSecret: _clientSecret));

            Azure.Response<Azure.Security.KeyVault.Secrets.KeyVaultSecret> keyVaultSecret = client.GetSecret(key);

            return keyVaultSecret?.Value?.Value;
        }

        public bool SendCertificate(string caminhoCertificado, string senhaCertificado, string nomeDoCertificadoNoKeyVault)
        {
            bool sucesso = true;

            // Autenticação com o Azure
            Azure.Security.KeyVault.Certificates.CertificateClient client;

            if (string.IsNullOrWhiteSpace(_clientId) ||
                string.IsNullOrWhiteSpace(_tenantId) ||
                string.IsNullOrWhiteSpace(_clientSecret))
                client = new CertificateClient(new Uri(_keyVaultUri), new DefaultAzureCredential());
            else
                client = new(new Uri(_keyVaultUri), new Azure.Identity.ClientSecretCredential(tenantId: _tenantId, clientId: _clientId, clientSecret: _clientSecret));

            // Ler o conteúdo do arquivo PFX
            byte[] certificadoBytes = Utilidades.IO.FileStorageService.Storage.ReadAllBytes(caminhoCertificado);

            // Importar o certificado para o Key Vault
            var importacao = new ImportCertificateOptions(nomeDoCertificadoNoKeyVault, certificadoBytes)
            {
                Password = senhaCertificado // Se o certificado PFX tiver senha, forneça aqui
            };

            // Criar a política de certificado com chave exportável
            var certificatePolicy = new CertificatePolicy("Self", "CN=" + nomeDoCertificadoNoKeyVault)
            { 
                Exportable = true  // Tornar a chave exportável
            };

            try
            {
                // Importar o certificado
                var resposta = client.ImportCertificate(importacao);
                //Console.WriteLine($"Certificado '{nomeDoCertificadoNoKeyVault}' importado com sucesso!");
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Erro ao importar o certificado: {ex.Message}");
                sucesso = false;
            }

            return sucesso;
        }

        public System.Security.Cryptography.X509Certificates.X509Certificate2 GetCertificate(string nomeDoCertificadoNoKeyVault)
        {
#if DEBUG
            return null;
#endif

            System.Security.Cryptography.X509Certificates.X509Certificate2 cert = null;
            DateTime? dataExpiracao = null;

            // Autenticação com o Azure
            Azure.Security.KeyVault.Certificates.CertificateClient client;

            if (string.IsNullOrWhiteSpace(_clientId) ||
                string.IsNullOrWhiteSpace(_tenantId) ||
                string.IsNullOrWhiteSpace(_clientSecret))
                client = new CertificateClient(new Uri(_keyVaultUri), new DefaultAzureCredential());
            else
                client = new(new Uri(_keyVaultUri), new Azure.Identity.ClientSecretCredential(tenantId: _tenantId, clientId: _clientId, clientSecret: _clientSecret));

            try
            {
                // Buscar o certificado do Key Vault
                var certificado = client.GetCertificate(nomeDoCertificadoNoKeyVault);
                if (certificado?.Value.Properties.ExpiresOn != null)
                    dataExpiracao = ((DateTimeOffset)certificado.Value.Properties.ExpiresOn).DateTime;

                // A versão do certificado
                var certificadoVersion = certificado.Value.Properties.Version;

                // Exportar o certificado em formato .pfx
                var certificadoComPfx = client.DownloadCertificate(nomeDoCertificadoNoKeyVault, certificadoVersion);

                //Console.WriteLine($"Certificado '{nomeDoCertificado}' encontrado com sucesso.");
                //Console.WriteLine($"Versão: {certificado.Value.Properties.Version}");
                //Console.WriteLine($"Data de expiração: {certificado.Value.Properties.ExpiresOn}");

                cert = certificadoComPfx.Value;
            }
            catch (Azure.RequestFailedException ex)
            {
                // Verifica se o erro é um 404 Not Found
                if (ex.Status == 404)
                {
                    Console.WriteLine($"Erro 404: O certificado '{nomeDoCertificadoNoKeyVault}' não foi encontrado no Key Vault.");
                }
                else
                {
                    // Outro tipo de erro
                    Console.WriteLine($"Erro inesperado: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Erro ao buscar o certificado: {ex.Message}");
            }

            return cert;
        }

        public bool DeleteCertificate(string nomeDoCertificadoNoKeyVault)
        {
#if DEBUG
            return true;
#endif

            bool sucesso = true;

            // Autenticação com o Azure
            Azure.Security.KeyVault.Certificates.CertificateClient client;

            if (string.IsNullOrWhiteSpace(_clientId) ||
                string.IsNullOrWhiteSpace(_tenantId) ||
                string.IsNullOrWhiteSpace(_clientSecret))
                client = new CertificateClient(new Uri(_keyVaultUri), new DefaultAzureCredential());
            else
                client = new(new Uri(_keyVaultUri), new Azure.Identity.ClientSecretCredential(tenantId: _tenantId, clientId: _clientId, clientSecret: _clientSecret));

            try
            {
                // Remover o certificado do Key Vault
                var resposta = client.StartDeleteCertificate(nomeDoCertificadoNoKeyVault);

                // Excluir permanentemente o certificado
                client.PurgeDeletedCertificate(nomeDoCertificadoNoKeyVault);

                // Exibir informações sobre o certificado deletado
                //Console.WriteLine($"Certificado '{nomeDoCertificadoNoKeyVault}' removido com sucesso!");
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Erro ao remover o certificado: {ex.Message}");
                sucesso = false;
            }

            return sucesso;
        }
    }
}
