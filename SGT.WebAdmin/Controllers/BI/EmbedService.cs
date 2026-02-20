using Microsoft.Identity.Client;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;
using SGT.WebAdmin.Models.BI;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace SGT.WebAdmin.Controllers
{
    public class EmbedService
    {
        private static readonly string AuthorityUrl = "https://login.microsoftonline.com/common/";
        private static readonly string ResourceUrl = "https://analysis.windows.net/powerbi/api/.default";
        private static readonly string ApiUrl = "https://api.powerbi.com/";

        private static readonly string AuthenticationType = "MasterUser";

        private EmbedConfig _embedConfig;
        private TokenCredentials _tokenCredentials;
        private EmbedParms _embedParms;


        public EmbedConfig EmbedConfig
        {
            get { return _embedConfig; }
        }

        public EmbedService(EmbedParms embedParms)
        {
            _tokenCredentials = null;
            _embedConfig = new EmbedConfig();
            _embedParms = embedParms;
        }

        public async Task<bool> EmbedReport(int idUsuario, string cnpjClienteFornecedor, string cnpjTransportador, Repositorio.UnitOfWork unitOfWork)
        {
            if (!_embedParms.ReportID.HasValue)
            {
                _embedConfig.ErrorMessage = "ReportID não informado.";
                return false;
            }


            if (GetMinutesUntilExpiration(_embedParms.TokenAutentication) < 1)
            {
                bool getCredentialsResult = await GetTokenCredentials(unitOfWork);

                if (!getCredentialsResult)
                    return false;
            }
            else
            {
                _tokenCredentials = new TokenCredentials(_embedParms.TokenAutentication, "Bearer");
            }
            try
            {
                using (PowerBIClient client = new PowerBIClient(new Uri(ApiUrl), _tokenCredentials))
                {
                    Report report = await client.Reports.GetReportAsync(_embedParms.WorkspaceId, _embedParms.ReportID.Value);

                    if (report == null)
                    {
                        _embedConfig.ErrorMessage = "Não existe relatório com o id informado.";
                        return false;
                    }

                    HttpOperationResponse<Dataset> datasets = await
                        client.Datasets.GetDatasetInGroupWithHttpMessagesAsync(_embedParms.WorkspaceId,
                        report.DatasetId);
                    _embedConfig.IsEffectiveIdentityRequired = datasets.Body.IsEffectiveIdentityRequired;
                    _embedConfig.IsEffectiveIdentityRolesRequired = datasets.Body.IsEffectiveIdentityRolesRequired;

                    if (_embedParms.MinutesToExpiration < 10)
                    {
                        Repositorio.Embarcador.BI.ConfigracaoBIReport repConfiguracaoBIReport = new Repositorio.Embarcador.BI.ConfigracaoBIReport(unitOfWork);

                        GenerateTokenRequest generateTokenRequestParameters = new GenerateTokenRequest(accessLevel: "view");

                        _embedParms.EmbedToken = await client.Reports.GenerateTokenInGroupAsync(_embedParms.WorkspaceId, report.Id, generateTokenRequestParameters);

                        if (_embedParms.EmbedToken == null)
                        {
                            _embedConfig.ErrorMessage = "Erro ao gerar token.";
                            return false;
                        }

                        repConfiguracaoBIReport.SetarDadosAcessoFormulario(_embedParms.CodigoFormulario,
                            _embedParms.EmbedToken.Token,
                            _embedParms.EmbedToken.TokenId,
                            _embedParms.EmbedToken.Expiration);
                    }

                    _embedConfig.EmbedToken = _embedParms.EmbedToken;
                    _embedConfig.EmbedUrl = report.EmbedUrl + "&filter=FatoOperador/CodigoOperador eq " + idUsuario.ToString();

                    if (!string.IsNullOrWhiteSpace(cnpjClienteFornecedor))
                        _embedConfig.EmbedUrl += @" and FatoCNPJFornecedor/CNPJFornecedor eq '" + cnpjClienteFornecedor + "'";

                    if (!string.IsNullOrWhiteSpace(cnpjTransportador))
                    {
                        _embedConfig.EmbedUrl += @" and FatoCNPJTransportador/CNPJTransportador eq '" + cnpjTransportador + "'";
                    }

                    _embedConfig.Id = report.Id.ToString();
                }
            }
            catch (HttpOperationException exc)
            {
                _embedConfig.ErrorMessage = string.Format("Status: {0} ({1})\r\nResponse: {2}\r\nRequestId: {3}", exc.Response.StatusCode, (int)exc.Response.StatusCode, exc.Response.Content, exc.Response.Headers["RequestId"].FirstOrDefault());
                return false;
            }

            return true;
        }

        private int GetMinutesUntilExpiration(string jwtToken)
        {
            if (string.IsNullOrWhiteSpace(jwtToken))
                return 0;

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);

            DateTime utcNow = DateTime.UtcNow;
            DateTime expiration = token.ValidTo;

            if (expiration <= utcNow)
                return 0;

            TimeSpan timeRemaining = expiration - utcNow;
            return (int)Math.Floor(timeRemaining.TotalMinutes);
        }

        private async Task<AuthenticationResult> DoAuthentication()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            AuthenticationResult authenticationResult = null;

            int tentativas = 1;

            while (tentativas < 5)
            {
                try
                {
                    var app = PublicClientApplicationBuilder
                            .Create(_embedParms.ApplicationId)
                            .WithAuthority(AzureCloudInstance.AzurePublic, AadAuthorityAudience.AzureAdMultipleOrgs)
                            .Build();

                    authenticationResult = await app.AcquireTokenByUsernamePassword(new[] { ResourceUrl }, _embedParms.Username, _embedParms.Password).ExecuteAsync();
                    break;
                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }

                if (!string.IsNullOrEmpty(authenticationResult?.AccessToken ?? string.Empty))
                    break;

                System.Threading.Thread.Sleep(500);
                tentativas++;
            }

            if (string.IsNullOrEmpty(authenticationResult?.AccessToken ?? string.Empty))
            {
                var msg = "Erro ao obter token";
                Servicos.Log.TratarErro(msg);
                throw new ArgumentException("Erro ao autenticar", msg);
            }

            return authenticationResult;
        }

        private async Task<bool> GetTokenCredentials(Repositorio.UnitOfWork unitOfWork)
        {
            var error = GetWebConfigErrors();
            if (error != null)
            {
                Servicos.Log.TratarErro("GetWebConfigErrors: " + error, "BI");
                _embedConfig.ErrorMessage = error;
                return false;
            }

            AuthenticationResult authenticationResult = null;
            try
            {
                authenticationResult = await DoAuthentication();
            }
            catch (AggregateException exc)
            {
                Servicos.Log.TratarErro("DoAuthentication: " + exc, "BI");
                _embedConfig.ErrorMessage = exc?.InnerException?.Message ?? "Erro ao autenticar";
                return false;
            }
            catch (Exception exc)
            {
                Servicos.Log.TratarErro("DoAuthentication: " + exc, "BI");
                _embedConfig.ErrorMessage = exc?.Message ?? "Erro ao autenticar";
                return false;
            }

            if (authenticationResult == null)
            {
                _embedConfig.ErrorMessage = "Authentication Failed.";
                return false;
            }

            Repositorio.Embarcador.BI.ConfigracaoBIReport repConfiguracaoBIReport = new Repositorio.Embarcador.BI.ConfigracaoBIReport(unitOfWork);
            repConfiguracaoBIReport.SetarTokenAutentication(authenticationResult.AccessToken);

            _tokenCredentials = new TokenCredentials(authenticationResult.AccessToken, "Bearer");
            return true;
        }

        private string GetWebConfigErrors()
        {
            if (string.IsNullOrWhiteSpace(_embedParms.ApplicationId))
                return "Application ID deve ser informado.";

            Guid result;
            if (!Guid.TryParse(_embedParms.ApplicationId, out result))
                return "Application ID deve ser informado.";

            if (string.IsNullOrWhiteSpace(_embedParms.Username))
                return "Usuario deve ser informado.";

            if (string.IsNullOrWhiteSpace(_embedParms.Password))
                return "Senha deve ser informada.";

            return null;
        }
    }
}