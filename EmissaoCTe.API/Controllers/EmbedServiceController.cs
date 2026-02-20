using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.PowerBI.Api.V2;
using Microsoft.PowerBI.Api.V2.Models;
using Microsoft.Rest;
using System;
using System.Linq;
using System.Collections.Specialized;
using System.Configuration;
using System.Threading.Tasks;
using System.Net;

namespace EmissaoCTe.API.Controllers
{
    public class EmbedService
    {
        private static readonly string AuthorityUrl = "https://login.microsoftonline.com/common/";
        private static readonly string ResourceUrl = "https://analysis.windows.net/powerbi/api";
        private static readonly string ApiUrl = "https://api.powerbi.com/";

        private static readonly string AuthenticationType = "MasterUser";
        private static readonly NameValueCollection sectionConfig = ConfigurationManager.GetSection(AuthenticationType) as NameValueCollection;
        private Models.BI.EmbedConfig _embedConfig;
        private TokenCredentials _tokenCredentials;
        private Models.BI.EmbedParms _embedParms;


        public Models.BI.EmbedConfig EmbedConfig
        {
            get { return _embedConfig; }
        }



        public EmbedService(Models.BI.EmbedParms embedParms)
        {
            _tokenCredentials = null;
            _embedConfig = new Models.BI.EmbedConfig();
            _embedParms = embedParms;
        }

        public async Task<bool> EmbedReport()
        {
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.StringConexao);
            try
            {

                if (string.IsNullOrWhiteSpace(_embedParms.ReportID))
                {
                    _embedConfig.ErrorMessage = "ReportID não informado.";
                    return false;
                }

                var getCredentialsResult = GetTokenCredentials();
                if (!getCredentialsResult)
                {
                    return false;
                }

                try
                {

                    using (var client = new PowerBIClient(new Uri(ApiUrl), _tokenCredentials))
                    {
                        var report = await client.Reports.GetReportAsync(_embedParms.WorkspaceId, _embedParms.ReportID);


                        if (report == null)
                        {
                            _embedConfig.ErrorMessage = "Não existe relatório com o id informado.";
                            return false;
                        }

                        var datasets = await client.Datasets.GetDatasetByIdInGroupAsync(_embedParms.WorkspaceId, report.DatasetId);
                        _embedConfig.IsEffectiveIdentityRequired = datasets.IsEffectiveIdentityRequired;
                        _embedConfig.IsEffectiveIdentityRolesRequired = datasets.IsEffectiveIdentityRolesRequired;

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

                            repConfiguracaoBIReport.SetarDadosAcessoFormulario(1, _embedParms.EmbedToken.Token, _embedParms.EmbedToken.TokenId, _embedParms.EmbedToken.Expiration);
                        }

                        _embedConfig.EmbedToken = _embedParms.EmbedToken;
                        _embedConfig.EmbedUrl = report.EmbedUrl;

                        _embedConfig.Id = report.Id;


                        //GenerateTokenRequest generateTokenRequestParameters;

                        //generateTokenRequestParameters = new GenerateTokenRequest(accessLevel: "view");

                        //var tokenResponse = await client.Reports.GenerateTokenInGroupAsync(_embedParms.WorkspaceId, report.Id, generateTokenRequestParameters);

                        //if (tokenResponse == null)
                        //{
                        //    _embedConfig.ErrorMessage = "Erro ao gerar token.";
                        //    return false;
                        //}


                        //_embedConfig.EmbedToken = tokenResponse;
                        //_embedConfig.EmbedUrl = report.EmbedUrl;
                        //_embedConfig.Id = report.Id;
                    }
                }
                catch (HttpOperationException exc)
                {
                    _embedConfig.ErrorMessage = string.Format("Status: {0} ({1})\r\nResponse: {2}\r\nRequestId: {3}", exc.Response.StatusCode, (int)exc.Response.StatusCode, exc.Response.Content, exc.Response.Headers["RequestId"].FirstOrDefault());
                    return false;
                }

                return true;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }


        private AuthenticationResult DoAuthentication()
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            AuthenticationResult authenticationResult = null;


            int tentativas = 1;

            while (tentativas < 5)
            {

                try
                {
                    var authenticationContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(AuthorityUrl);
                    var credential = new Microsoft.IdentityModel.Clients.ActiveDirectory.UserPasswordCredential(_embedParms.Username, _embedParms.Password);
                    authenticationResult = authenticationContext.AcquireTokenAsync(ResourceUrl, _embedParms.ApplicationId, credential).Result;
                    break;

                }
                catch (Exception ex)
                {
                    Servicos.Log.TratarErro(ex);
                }

                // deu erro mas obteve o token contiuna
                if (!string.IsNullOrEmpty(authenticationResult?.AccessToken ?? string.Empty))
                    break;


                System.Threading.Thread.Sleep(500);

                tentativas++;
            }

            if (string.IsNullOrEmpty(authenticationResult?.AccessToken ?? string.Empty))
            {
                var msg = "Erro ao obter token";
                Servicos.Log.TratarErro(msg);
                throw new System.ArgumentException("Erro ao autenticar", msg);

            }


            return authenticationResult;
        }

        private bool GetTokenCredentials()
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
                authenticationResult = DoAuthentication();
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

            _tokenCredentials = new TokenCredentials(authenticationResult.AccessToken, "Bearer");
            return true;
        }


        private string GetWebConfigErrors()
        {
            if (string.IsNullOrWhiteSpace(_embedParms.ApplicationId))
                return "Application ID deve ser informado.";

            Guid result;
            if (!Guid.TryParse(_embedParms.ApplicationId, out result))
                return "Aplication ID deve ser informado.";


            if (string.IsNullOrWhiteSpace(_embedParms.WorkspaceId))
                return "Workspace deve ser informado.";

            if (!Guid.TryParse(_embedParms.WorkspaceId, out result))
                return "Workspace deve ser informado.";

            if (string.IsNullOrWhiteSpace(_embedParms.Username))
                return "Usuario deve ser informado.";


            if (string.IsNullOrWhiteSpace(_embedParms.Password))
                return "senha deve ser infomrada";



            return null;
        }

    }
}