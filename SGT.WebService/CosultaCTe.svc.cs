using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using CoreWCF;
using System.Text;
using System.Web;


namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class CosultaCTe(IServiceProvider _serviceProvider) : ICosultaCTe
    {
        private string URLReceita = "http://www.cte.fazenda.gov.br/portal/consulta.aspx?tipoConsulta=completa&tipoConteudo=mCK/KoCqru0=";

        public Retorno<Dominio.ObjetosDeValor.WebService.CTe.ConsultaSefaz> ConsultarSefaz(Dominio.ObjetosDeValor.WebService.CTe.RequisicaoSefaz requisicaoSefaz, string ChaveCTe, string Capcha)
        {
            ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);

            Retorno<Dominio.ObjetosDeValor.WebService.CTe.ConsultaSefaz> retorno = new Retorno<Dominio.ObjetosDeValor.WebService.CTe.ConsultaSefaz>();
            retorno.Status = true;
            retorno.Objeto = new Dominio.ObjetosDeValor.WebService.CTe.ConsultaSefaz();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            try
            {
                HttpWebRequest requestWsReceita = (HttpWebRequest)WebRequest.Create(URLReceita);

                string viewstate = HttpUtility.UrlEncode(requisicaoSefaz.VIEWSTATE);
                string eventvalidator = HttpUtility.UrlEncode(requisicaoSefaz.EVENTVALIDATION);

                string postData = String.Format("__EVENTTARGET={0}" +
                    "&__EVENTARGUMENT={1}" +
                    "&__VIEWSTATE={2}" +
                    "&__EVENTVALIDATION={3}" +
                    "&ctl00$txtPalavraChave={4}" +
                    "&ctl00$ContentPlaceHolder1$txtChaveAcessoCompleta={5}" +
                    "&ctl00$ContentPlaceHolder1$txtCaptcha={6}" +
                    "&ctl00$ContentPlaceHolder1$btnConsultar={7}" +
                    "&ctl00$ContentPlaceHolder1$token={8}" +
                    "&ctl00$ContentPlaceHolder1$captchaSom={9}" +
                    "&hiddenInputToUpdateATBuffer_CommonToolkitScripts={10}",
                    "",
                    "",
                    viewstate,
                    eventvalidator,
                    "",
                    ChaveCTe,
                    Capcha,
                    "Continuar",
                    HttpUtility.UrlEncode(requisicaoSefaz.TokenCaptcha),
                    "",
                    "1");

                var data = Encoding.UTF8.GetBytes(postData);

                requestWsReceita.Method = "POST";
                requestWsReceita.AllowAutoRedirect = true;
                requestWsReceita.UserAgent = "Mozilla / 5.0(Windows NT 10.0; WOW64; rv: 44.0) Gecko / 20100101 Firefox / 44.0";
                requestWsReceita.Host = "www.cte.fazenda.gov.br";
                requestWsReceita.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                requestWsReceita.ContentType = "application/x-www-form-urlencoded";
                requestWsReceita.CookieContainer = new CookieContainer();
                requestWsReceita.CookieContainer.Add(new Cookie("ASP.NET_SessionId", requisicaoSefaz.SessionID, "", "www.cte.fazenda.gov.br"));
                requestWsReceita.ContentLength = data.Length;
                requestWsReceita.Referer = URLReceita;

                using (var stream = requestWsReceita.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)requestWsReceita.GetResponse();
                Servicos.Embarcador.CTe.ConsultaReceita serConsultaReceita = new Servicos.Embarcador.CTe.ConsultaReceita(unitOfWork);

                if (response.ResponseUri.AbsolutePath == "/portal/consultaCompleta.aspx")
                {
                    retorno.Objeto.CTe = serConsultaReceita.ProcessarHTMLRetorno(response.GetResponseStream());
                    if (retorno.Objeto.CTe != null)
                    {
                        retorno.Objeto.CTe.Chave = ChaveCTe;
                        retorno.Objeto.ConsultaValida = true;
                        retorno.Objeto.MensagemSefaz = "";
                    }
                    else
                    {
                        retorno.Objeto.ConsultaValida = false;
                        retorno.Objeto.MensagemSefaz = "CT-e INEXISTENTE na base nacional, favor consultar este CT-e no site da SEFAZ de origem.";
                    }
                }
                else
                {
                    List<String> erros = serConsultaReceita.VerificarMensagemErro(response.GetResponseStream());
                    string msg = "";
                    foreach (string erro in erros)
                    {
                        msg += " " + erro;
                    }
                    retorno.Objeto.ConsultaValida = false;
                    retorno.Objeto.MensagemSefaz = msg;
                }
                
                response.Close();
                response.Dispose();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Status = false;
                retorno.Mensagem = "Ocorreu uma falha ao consultar na nota no Sefaz";
            } 
            finally {
                unitOfWork.Dispose();
            }
            return retorno;
        }



        public Retorno<Dominio.ObjetosDeValor.WebService.CTe.RequisicaoSefaz> SolicitarRequisicaoSefaz()
        {
            ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Servicos.Embarcador.NFe.ConsultaReceita serConsultaReceita = new Servicos.Embarcador.NFe.ConsultaReceita(unitOfWork);
            Servicos.Embarcador.Documentos.Documento serDocumento = new Servicos.Embarcador.Documentos.Documento(unitOfWork);

            Retorno<Dominio.ObjetosDeValor.WebService.CTe.RequisicaoSefaz> retorno = new Retorno<Dominio.ObjetosDeValor.WebService.CTe.RequisicaoSefaz>();
            retorno.Status = true;
            retorno.Objeto = new Dominio.ObjetosDeValor.WebService.CTe.RequisicaoSefaz();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(URLReceita);
                request.CookieContainer = new CookieContainer();
                var response = (HttpWebResponse)request.GetResponse();
                foreach (Cookie cook in response.Cookies)
                {
                    if (cook.Name == "ASP.NET_SessionId")
                        retorno.Objeto.SessionID = cook.Value;
                }

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                retorno.Objeto.VIEWSTATE = serDocumento.ExtractCampo(responseString, "__VIEWSTATE", "value");
                retorno.Objeto.EVENTVALIDATION = serDocumento.ExtractCampo(responseString, "__EVENTVALIDATION", "value");
                retorno.Objeto.Captcha = serDocumento.ExtractCampo(responseString, "ContentPlaceHolder1_imgCaptcha", "src");
                retorno.Objeto.TokenCaptcha = serDocumento.ExtractCampo(responseString, "ContentPlaceHolder1_token", "value");

                response.Close();
                response.Dispose();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao solicitar a requisição ao Sefaz";
            } finally
            {
                unitOfWork.Dispose();
            }
            return retorno;
        }
        public static bool ValidarToken() //todo: ver regra para esses metodos, pois o token aqui é diferente
        {
            if (OperationContext.Current.IncomingMessageHeaders.FindHeader("Token", "Token") == -1)
                throw new FaultException("Token inválido. Adicione a tag do token no header (cabeçalho) da requisição, conforme exemplo: <Token xmlns='Token'>seu token</Token>");


            string token = Convert.ToString(OperationContext.Current.IncomingMessageHeaders.GetHeader<string>("Token", "Token"));
            if (token == "4ed60154d2f04201ab8b57ed4198da32")
            {
                return true;
            }
            else
            {
                throw new FaultException("Token inválido. Verifique se o token informado é o mesmo informado autorizado para a integração.");
            }
        }

    }
}
