using Dominio.ObjetosDeValor.Embarcador.Auditoria;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util;
using Google.Apis.Util.Store;
using Infrastructure.Services.HttpClientFactory;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using Repositorio;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.Gmail
{
    public class IntegracaoGmail
    {
        private UnitOfWork unitOfWork;

        public IntegracaoGmail(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<bool> EnviarEmailViaGmailApiAsync(string clientId, string clientSecret, string caminhoTokenResposta, int codigoConfigEmailDocTransporte,  string fromEmail, string to, string[] cc, string[] bcc, string subject, string htmlBody, List<Attachment> attachments, string urlenvio, string signature = "")
        {
            try
            {
                var accessToken = await ObterAccessTokenAsync(clientId, clientSecret, caminhoTokenResposta);

                if (string.IsNullOrWhiteSpace(accessToken.accessToken))
                    throw new Exception("Não foi possível obter o access_token");

                if (!string.IsNullOrWhiteSpace(accessToken.updatedTokenBase64))
                {
                        var repConfigEmailDocTransporte = new Repositorio.Embarcador.Email.ConfigEmailDocTransporte(unitOfWork);
                        var configEmailDocTransporte = repConfigEmailDocTransporte.BuscarPorCodigo(codigoConfigEmailDocTransporte, true);

                        configEmailDocTransporte.caminhoTokenResposta = accessToken.updatedTokenBase64;
                        repConfigEmailDocTransporte.Atualizar(configEmailDocTransporte, null);    
                }

                var mimeBuilder = new StringBuilder();
                string boundary = "----=_Part_" + Guid.NewGuid().ToString("N");

                mimeBuilder.AppendLine($"From: {fromEmail}");
                mimeBuilder.AppendLine($"To: {to}");

                if (cc?.Length > 0)
                    mimeBuilder.AppendLine($"Cc: {string.Join(",", cc)}");

                if (bcc?.Length > 0)
                    mimeBuilder.AppendLine($"Bcc: {string.Join(",", bcc)}");

                mimeBuilder.AppendLine($"Subject: {subject}");
                mimeBuilder.AppendLine("MIME-Version: 1.0");
                mimeBuilder.AppendLine($"Content-Type: multipart/mixed; boundary=\"{boundary}\"");
                mimeBuilder.AppendLine();
                mimeBuilder.AppendLine($"--{boundary}");

                mimeBuilder.AppendLine("Content-Type: text/html; charset=\"UTF-8\"");
                mimeBuilder.AppendLine("Content-Transfer-Encoding: 7bit");
                mimeBuilder.AppendLine();

                string corpoFinal = htmlBody;
                if (!string.IsNullOrWhiteSpace(signature))
                    corpoFinal += $"<br><br>--<br>{signature.Replace(Environment.NewLine, "<br>")}";

                mimeBuilder.AppendLine(corpoFinal);
                mimeBuilder.AppendLine();

                // Anexos
                if (attachments != null)
                {
                    foreach (var attachment in attachments)
                    {
                        using var ms = new MemoryStream();
                        attachment.ContentStream.Position = 0;
                        await attachment.ContentStream.CopyToAsync(ms);
                        var fileBytes = ms.ToArray();
                        var base64File = Convert.ToBase64String(fileBytes);

                        mimeBuilder.AppendLine($"--{boundary}");
                        mimeBuilder.AppendLine($"Content-Type: {attachment.ContentType.MediaType}; name=\"{attachment.Name}\"");
                        mimeBuilder.AppendLine("Content-Transfer-Encoding: base64");
                        mimeBuilder.AppendLine($"Content-Disposition: attachment; filename=\"{attachment.Name}\"");
                        mimeBuilder.AppendLine();
                        mimeBuilder.AppendLine(Base64Chunked(base64File));
                        mimeBuilder.AppendLine();
                    }
                }

                mimeBuilder.AppendLine($"--{boundary}--");
                string rawMessage = Convert.ToBase64String(Encoding.UTF8.GetBytes(mimeBuilder.ToString()))
                    .Replace("+", "-")
                    .Replace("/", "_")
                    .Replace("=", "");

                var json = $"{{\"raw\": \"{rawMessage}\"}}";

                // Envia para Gmail API
                using var client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoGmail));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.accessToken);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(urlenvio, content);

                if (response.IsSuccessStatusCode)
                    return true;

                var responseText = await response.Content.ReadAsStringAsync();
                Servicos.Log.TratarErro($"Erro Gmail API: {response.StatusCode} - {responseText}");
                return false;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                return false;
            }
        }
        public async Task<(string accessToken, string updatedTokenBase64)> ObterAccessTokenAsync(string clientId, string clientSecret, string? tokenBase64)
        {
            try
            {
                string tokenFilePath;

                if (!string.IsNullOrWhiteSpace(tokenBase64))
                {
                    tokenFilePath = TokenResponseFileConverter.DecodeBase64ToFile(tokenBase64);
                }
                else
                {
                    string tokenDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "GoogleTokenStoreTemp");
                    Directory.CreateDirectory(tokenDir);
                    tokenFilePath = Path.Combine(tokenDir, "Google.Apis.Auth.OAuth2.Responses.TokenResponse-user");
                }

                string dataStorePath = Path.GetDirectoryName(tokenFilePath)!;

                var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    new ClientSecrets { ClientId = clientId, ClientSecret = clientSecret },
                    new[] { GmailService.Scope.GmailSend },
                    "user",
                    CancellationToken.None,
                    new FileDataStore(dataStorePath, true)
                );

                if (credential.Token.IsStale)
                {
                    bool renovado = await credential.RefreshTokenAsync(CancellationToken.None);
                    if (!renovado)
                    {
                        Servicos.Log.TratarErro("Falha ao renovar token.");
                    }
                }

                string tokenDataFile = Path.Combine(dataStorePath, "Google.Apis.Auth.OAuth2.Responses.TokenResponse-user");
                string updatedBase64 = TokenResponseFileConverter.EncodeFileToBase64(tokenDataFile);

                return (credential.Token.AccessToken, updatedBase64);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"Erro ao obter token: {ex}");
                return (string.Empty, string.Empty); 
            }
        }

        private string Base64Chunked(string input, int chunkSize = 76)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < input.Length; i += chunkSize)
                sb.AppendLine(input.Substring(i, Math.Min(chunkSize, input.Length - i)));
            return sb.ToString();
        }
    }
}