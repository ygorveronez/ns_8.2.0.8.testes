using Infrastructure.Services.HttpClientFactory;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Servicos
{
    public class SMS : ServicoBase
    {        
        public SMS(Repositorio.UnitOfWork unitOfWork) : base(unitOfWork) { }

        /*
            sms.comtele.com.br
            https://sms.comtele.com.br/api/v2/customconfig/a189c6fc-711b-481d-9b8b-40f99f4673cc
            https://comtele.com.br/
            https://comtele.com.br/arquivos/ManualSMSComteleAPI.pdf
        */

        #region Métodos Públicos

        /// <summary>
        /// Utiliza serviço da COMTELE para envio de mensagens
        /// </summary>
        public string EnviarMensagem(Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal notaFiscal, string urlAcesso, Repositorio.UnitOfWork unitOfWork)
        {
            if (!notaFiscal.Empresa.AtivarEnvioDanfeSMS)
                return "Empresa não habilitada para envio de SMS.";
            else if (string.IsNullOrWhiteSpace(notaFiscal.Empresa.TokenSMS))
                return "Favor cadastrar o Número do Token da Comtele no Cadastro da Empresa.";
            else if (string.IsNullOrWhiteSpace(urlAcesso))
                return "Url para o link do SMS não foi encontrado.";
            else if (string.IsNullOrWhiteSpace(notaFiscal.Cliente.Celular))
                return "Favor cadastrar o Número do Celular no cadastro do Cliente.";

            string msgRetorno;
            string qtdSaldo = ObterSaldo(notaFiscal.Empresa.TokenSMS, notaFiscal, unitOfWork, out msgRetorno);
            if (!string.IsNullOrWhiteSpace(msgRetorno))
                return msgRetorno;
            else if (qtdSaldo.ToInt() <= 0)
            {
                msgRetorno = "Você não possui mais crédito disponível para envio de SMS! Favor contratar mais, caso queria continuar utilizado o serviço.";
                LogEnvioSMS.SalvarLogEnvioSMS("", false, msgRetorno, notaFiscal, unitOfWork);
                return msgRetorno;
            }

            string linkDownload = urlAcesso + "/NotaFiscalEletronica/DanfeSMS?Codigo=" + notaFiscal.Empresa.Codigo + "&ChaveNFe=" + notaFiscal.Chave;
            linkDownload = EncurtadorUrl(linkDownload);
            if (string.IsNullOrWhiteSpace(linkDownload))
                return "Problemas ao gerar link! Favor contatar o suporte do sistema.";

            string urlWebService = "https://sms.comtele.com.br/api/" + notaFiscal.Empresa.TokenSMS +
                "/sendmessage?sender=" + notaFiscal.Empresa.CNPJ_SemFormato +
                "&receivers=" + notaFiscal.Cliente.Celular +
                "&content=Baixe a DANFE numero " + notaFiscal.Numero.ToString() + " da empresa " + notaFiscal.Empresa.NomeFantasia + ": " + linkDownload;

            var client = HttpClientFactoryWrapper.GetClient(nameof(SMS));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var result = client.GetAsync(urlWebService).Result;
                if (result.IsSuccessStatusCode)
                {
                    var retorno = (string)(result.Content.ReadAsStringAsync().Result);
                    if (!string.IsNullOrWhiteSpace(retorno))
                    {
                        LogEnvioSMS.SalvarLogEnvioSMS(linkDownload, true, retorno.TrimStart('"').TrimEnd('"').Trim(), notaFiscal, unitOfWork);
                        return string.Empty;
                    }
                    else
                    {
                        msgRetorno = "Problemas ao comunicar com o serviço da COMTELE, favor tente novamente mais tarde.";
                        LogEnvioSMS.SalvarLogEnvioSMS(linkDownload, false, msgRetorno, notaFiscal, unitOfWork);
                        return msgRetorno;
                    }
                }
                else
                {
                    msgRetorno = "Problemas ao comunicar com o serviço da COMTELE, favor tente novamente mais tarde.";
                    LogEnvioSMS.SalvarLogEnvioSMS(linkDownload, false, msgRetorno, notaFiscal, unitOfWork);
                    return msgRetorno;
                }
            }
            catch (Exception ex)
            {
                msgRetorno = ex.Message;
                LogEnvioSMS.SalvarLogEnvioSMS(linkDownload, false, msgRetorno, notaFiscal, unitOfWork);
                return msgRetorno;
            }
        }

        public bool EnviarSMS(string tokenSMS, string CNPJremetente, string para, string mensagem, Repositorio.UnitOfWork unitOfWork, out string msgRetorno)
        {
            if (string.IsNullOrWhiteSpace(tokenSMS))
            {
                msgRetorno = "Favor cadastrar o Número do Token da Comtele no Cadastro da Empresa.";
                return false;
            }
            else if (string.IsNullOrWhiteSpace(para))
            {
                msgRetorno = "Favor cadastrar o Número do Celular no cadastro do Cliente.";
                return false;
            }

            string qtdSaldo = ObterSaldo(tokenSMS, null, unitOfWork, out msgRetorno);

            if (!string.IsNullOrWhiteSpace(msgRetorno))
                return false;

            else if (qtdSaldo.ToInt() <= 0)
            {
                msgRetorno = "Você não possui mais crédito disponível para envio de SMS! Favor contratar mais, caso queria continuar utilizado o serviço.";
                LogEnvioSMS.SalvarLogEnvioSMS("", false, msgRetorno, null, unitOfWork);
                return false;
            }

            string urlWebService = string.Format("https://sms.comtele.com.br/api/{0}/sendmessage?sender={1}&receivers={2}&content={3}", new string[] { tokenSMS, CNPJremetente, para, mensagem });

            var client = HttpClientFactoryWrapper.GetClient(nameof(SMS));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var result = client.GetAsync(urlWebService).Result;
                if (!result.IsSuccessStatusCode)
                {
                    msgRetorno = "Problemas ao comunicar com o serviço da COMTELE, favor tente novamente mais tarde.";
                    LogEnvioSMS.SalvarLogEnvioSMS("", false, msgRetorno, null, unitOfWork);
                    return false;
                }

                var retorno = (result.Content.ReadAsStringAsync().Result);
                if (string.IsNullOrWhiteSpace(retorno))
                {
                    msgRetorno = "Problemas ao comunicar com o serviço da COMTELE, favor tente novamente mais tarde.";
                    LogEnvioSMS.SalvarLogEnvioSMS("", false, msgRetorno, null, unitOfWork);
                    return false;
                }

                LogEnvioSMS.SalvarLogEnvioSMS("", true, retorno.TrimStart('"').TrimEnd('"').Trim(), null, unitOfWork);
                return true;
            }
            catch (Exception ex)
            {
                msgRetorno = ex.Message;
                LogEnvioSMS.SalvarLogEnvioSMS("", false, msgRetorno, null, unitOfWork);
                return false;
            }
        }

        #endregion

        #region Métodos Privados

        private string ObterSaldo(string tokenSMS, Dominio.Entidades.Embarcador.NotaFiscal.NotaFiscal notaFiscal, Repositorio.UnitOfWork unitOfWork, out string msgRetorno)
        {
            msgRetorno = "";

            string urlWebService = "https://sms.comtele.com.br/api/" + tokenSMS + "/balance";

            var client = HttpClientFactoryWrapper.GetClient(nameof(SMS));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var result = client.GetAsync(urlWebService).Result;
                if (result.IsSuccessStatusCode)
                {
                    var retorno = (string)(result.Content.ReadAsStringAsync().Result);
                    if (!string.IsNullOrWhiteSpace(retorno))
                    {
                        return retorno;
                    }
                    else
                    {
                        msgRetorno = "Problemas ao comunicar com o serviço de Saldo da COMTELE, favor tente novamente mais tarde.";
                        LogEnvioSMS.SalvarLogEnvioSMS("", false, msgRetorno, notaFiscal, unitOfWork);
                        return string.Empty;
                    }
                }
                else
                {
                    msgRetorno = "Problemas ao comunicar com o serviço de Saldo da COMTELE, favor tente novamente mais tarde.";
                    LogEnvioSMS.SalvarLogEnvioSMS("", false, msgRetorno, notaFiscal, unitOfWork);
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                msgRetorno = ex.Message;
                LogEnvioSMS.SalvarLogEnvioSMS("", false, msgRetorno, notaFiscal, unitOfWork);
                return string.Empty;
            }
        }

        private string EncurtadorUrl(string url)
        {
            try
            {
                string urlMontada = string.Format("http://tinyurl.com/api-create.php?url={0}", url);

                var client = new WebClient();

                string response = client.DownloadString(urlMontada);

                client.Dispose();

                return response;
            }
            catch (WebException ex)
            {
                Log.TratarErro(ex);
                return string.Empty;
            }
        }

        #endregion
    }
}
