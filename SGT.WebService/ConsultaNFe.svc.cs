using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using CoreWCF;
using Repositorio;

namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class ConsultaNFe(IServiceProvider _serviceProvider) : IConsultaNFe
    {

        private string URLReceita = "http://www.nfe.fazenda.gov.br/portal/consultaResumoCompletaAntiga.aspx?tipoConsulta=completa&tipoConteudo=XbSeqxE8pl8=";
        //private string URLReceita = "http://www.nfe.fazenda.gov.br/portal/consultaRecaptcha.aspx?tipoConsulta=completa&tipoConteudo=XbSeqxE8pl8=";
        private string googleRecptcha = "https://www.google.com/recaptcha/api2/bframe?hl=pt-BR&v=r20171025115245&k=";

        private string fsist = "https://www.fsist.com.br/";

        public string sortearServer()
        {
            List<string> servidores = new List<string>();
            servidores.Add("https://www.fsist.com.br/baixarxml.ashx?m=WEB");
            servidores.Add("https://server2.fsist.com.br/baixarxml.ashx?m=WEB");
            servidores.Add("https://server4.fsist.com.br/baixarxml.ashx?m=WEB");
            servidores.Add("https://server5.fsist.com.br/baixarxml.ashx?m=WEB");
            servidores.Add("https://server6.fsist.com.br/baixarxml.ashx?m=WEB");
            servidores.Add("https://server7.fsist.com.br/baixarxml.ashx?m=WEB");
            servidores.Add("https://server8.fsist.com.br/baixarxml.ashx?m=WEB");
            servidores.Add("https://server9.fsist.com.br/baixarxml.ashx?m=WEB");

            Random randNum = new Random();
            int rando = randNum.Next(0, 7);
            if (rando == 0)
                rando = randNum.Next(0, 7);

            return servidores[rando];
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.NFe.RequisicaoSefaz> SolicitarRequisicaoSefaz()
        {
            ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Retorno<Dominio.ObjetosDeValor.WebService.NFe.RequisicaoSefaz> retorno = new Retorno<Dominio.ObjetosDeValor.WebService.NFe.RequisicaoSefaz>();
            retorno.Status = true;
            retorno.Objeto = new Dominio.ObjetosDeValor.WebService.NFe.RequisicaoSefaz();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            Servicos.Embarcador.NFe.ConsultaReceita serConsultaReceita = new Servicos.Embarcador.NFe.ConsultaReceita(unitOfWork);
            Servicos.Embarcador.Documentos.Documento serDocumento = new Servicos.Embarcador.Documentos.Documento(unitOfWork);
            Random randNum = new Random();

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(fsist);

                request.Method = "GET";
                request.AllowAutoRedirect = true;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36";
                request.Host = "www.fsist.com.br";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                request.ContentType = "application/x-www-form-urlencoded";
                int userID = randNum.Next(100000000, 999999999);
                retorno.Objeto.TokenCaptcha = userID.ToString();
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(new Cookie("UsuarioID", retorno.Objeto.TokenCaptcha, "", "www.fsist.com.br"));
                var response = (HttpWebResponse)request.GetResponse();

                foreach (Cookie cook in response.Cookies)
                {
                    if (cook.Name == "FSistSessao")
                        retorno.Objeto.SessionID = cook.Value;
                }
                string server = sortearServer();
                int rando = randNum.Next(100000000, 999999999);
                retorno.Objeto.VIEWSTATE = server;
                string urlImagem = server + "&UsuarioID=" + retorno.Objeto.TokenCaptcha + "&cte=0&pub=11&com=&t=captcha&chave=&r=" + rando;
                response.Close();
                response.Dispose();
                var lxRequest = (HttpWebRequest)WebRequest.Create(urlImagem);
                var responseIMG = (HttpWebResponse)lxRequest.GetResponse();
                byte[] imageBytes = ReadFully(responseIMG.GetResponseStream());
                string base64String = Convert.ToBase64String(imageBytes);
                retorno.Objeto.Captcha = "data:image/png;base64," + base64String;
                responseIMG.Close();
                responseIMG.Dispose();

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Sefaz indisponível para consulta de NFe, favor fazer importação via XML.";
            } 
            finally
            {
                unitOfWork.Dispose();
            }
            return retorno;
        }

        public byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.NFe.ConsultaSefaz> ConsultarSefaz(Dominio.ObjetosDeValor.WebService.NFe.RequisicaoSefaz requisicaoSefaz, string ChaveNFe, string Capcha)
        {
            ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Retorno<Dominio.ObjetosDeValor.WebService.NFe.ConsultaSefaz> retorno = new Retorno<Dominio.ObjetosDeValor.WebService.NFe.ConsultaSefaz>();
            retorno.Status = true;
            retorno.Objeto = new Dominio.ObjetosDeValor.WebService.NFe.ConsultaSefaz();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            try
            {
                string url = requisicaoSefaz.VIEWSTATE + "&UsuarioID=" + requisicaoSefaz.TokenCaptcha + "&cte=0&pub=a8b1abb06bae&com=a8b1abb06bae&t=consulta&chave=" + ChaveNFe + "&captcha=" + Capcha + "&qtd=27";

                var request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "GET";
                request.AllowAutoRedirect = true;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36";
                request.Host = "www.fsist.com.br";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                request.ContentType = "application/x-www-form-urlencoded";

                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(new Cookie("UsuarioID", requisicaoSefaz.TokenCaptcha, "", "www.fsist.com.br"));
                request.CookieContainer.Add(new Cookie("FSistSessao", requisicaoSefaz.SessionID, "", "www.fsist.com.br"));

                var response = (HttpWebResponse)request.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                response.Close();
                response.Dispose();

                if (responseString == "OK")
                {
                    string urlNF = requisicaoSefaz.VIEWSTATE + "&UsuarioID=" + requisicaoSefaz.TokenCaptcha + "&cte=0&pub=a8b1abb06bae&com=a8b1abb06bae&t=visualizar&chave=" + ChaveNFe;

                    var requestReceita = (HttpWebRequest)WebRequest.Create(urlNF);

                    requestReceita.Method = "GET";
                    requestReceita.AllowAutoRedirect = true;
                    requestReceita.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36";
                    requestReceita.Host = "www.fsist.com.br";
                    requestReceita.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                    requestReceita.ContentType = "application/x-www-form-urlencoded";

                    requestReceita.CookieContainer = new CookieContainer();
                    requestReceita.CookieContainer.Add(new Cookie("UsuarioID", requisicaoSefaz.TokenCaptcha, "", "www.fsist.com.br"));
                    requestReceita.CookieContainer.Add(new Cookie("FSistSessao", requisicaoSefaz.SessionID, "", "www.fsist.com.br"));

                    var responseReceita = (HttpWebResponse)requestReceita.GetResponse();

                    Servicos.Embarcador.NFe.ConsultaReceita serConsultaReceita = new Servicos.Embarcador.NFe.ConsultaReceita(unitOfWork);
                    retorno.Objeto.NotaFiscal = serConsultaReceita.ProcessarHTMLRetorno(responseReceita.GetResponseStream(), "");
                    if (retorno.Objeto.NotaFiscal != null)
                    {
                        retorno.Objeto.NotaFiscal.Chave = ChaveNFe;
                        retorno.Objeto.ConsultaValida = true;
                        retorno.Objeto.MensagemSefaz = "";
                    }
                    else
                    {
                        retorno.Objeto.ConsultaValida = false;
                        retorno.Objeto.MensagemSefaz = "NF-e INEXISTENTE na base nacional, favor consultar esta NF-e no site da SEFAZ de origem.";
                    }

                    responseReceita.Close();
                    responseReceita.Dispose();
                }
                else
                {
                    retorno.Objeto.ConsultaValida = false;
                    retorno.Objeto.MensagemSefaz = responseString;
                }

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Sefaz indisponível para consulta de NFe, favor fazer importação via XML.";
            } 
            finally
            {
                unitOfWork.Dispose();
            }

            return retorno;
        }


        //public Retorno<Dominio.ObjetosDeValor.WebService.NFe.ConsultaSefaz> ConsultarSefaz(Dominio.ObjetosDeValor.WebService.NFe.RequisicaoSefaz requisicaoSefaz, string ChaveNFe, string Capcha)
        //{
        //    ValidarToken();

        //    Retorno<Dominio.ObjetosDeValor.WebService.NFe.ConsultaSefaz> retorno = new Retorno<Dominio.ObjetosDeValor.WebService.NFe.ConsultaSefaz>();
        //    retorno.Status = true;
        //    retorno.Objeto = new Dominio.ObjetosDeValor.WebService.NFe.ConsultaSefaz();
        //    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

        //    try
        //    {
        //        HttpWebRequest requestWsReceita = (HttpWebRequest)WebRequest.Create(URLReceita);

        //        //if (!string.IsNullOrWhiteSpace(requisicaoSefaz.Proxy))
        //        //{
        //        //    AdminMultisoftware.Dominio.Entidades.Proxy.Proxy proxy = ObterProxyUtilizado(requisicaoSefaz.Proxy);
        //        //    if(proxy != null)
        //        //    {
        //        //        WebProxy proxyObject = new WebProxy(proxy.IP, proxy.Porta);
        //        //        requestWsReceita.Proxy = proxyObject;
        //        //    }
        //        //}



        //        string viewstate = HttpUtility.UrlEncode(requisicaoSefaz.VIEWSTATE);
        //        string eventvalidator = HttpUtility.UrlEncode(requisicaoSefaz.EVENTVALIDATION);

        //        AdminMultisoftware.Dominio.Entidades.Proxy.Proxy proxy = ObterProxyChave(requisicaoSefaz.EVENTVALIDATION);
        //        if (proxy != null)
        //        {
        //            WebProxy proxyObject = new WebProxy(proxy.IP, proxy.Porta);
        //            requestWsReceita.Proxy = proxyObject;
        //        }

        //        string postData = String.Format("__EVENTTARGET={0}" +
        //            "&__EVENTARGUMENT={1}" +
        //            "&__VIEWSTATE={2}" +
        //            "&__VIEWSTATEGENERATOR={3}" +
        //            "&__EVENTVALIDATION={4}" +
        //            "&ctl00$txtPalavraChave={5}" +
        //            "&ctl00$ContentPlaceHolder1$txtChaveAcessoCompleta={6}" +
        //            "&ctl00$ContentPlaceHolder1$txtCaptcha={7}" +
        //            "&ctl00$ContentPlaceHolder1$btnConsultar={8}" +
        //            "&ctl00$ContentPlaceHolder1$token={9}" +
        //            "&ctl00$ContentPlaceHolder1$captchaSom={10}" +
        //            "&hiddenInputToUpdateATBuffer_CommonToolkitScripts={11}",
        //            "",
        //            "",
        //            viewstate,
        //            "",
        //            eventvalidator,
        //            "",
        //            ChaveNFe,
        //            Capcha,
        //            "Continuar",
        //            HttpUtility.UrlEncode(requisicaoSefaz.TokenCaptcha),
        //            "",
        //            "1");

        //        var data = Encoding.UTF8.GetBytes(postData);

        //        requestWsReceita.Method = "POST";
        //        requestWsReceita.AllowAutoRedirect = true;
        //        requestWsReceita.UserAgent = "Mozilla / 5.0(Windows NT 10.0; WOW64; rv: 44.0) Gecko / 20100101 Firefox / 44.0";
        //        requestWsReceita.Host = "www.nfe.fazenda.gov.br";
        //        requestWsReceita.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
        //        requestWsReceita.ContentType = "application/x-www-form-urlencoded";
        //        requestWsReceita.CookieContainer = new CookieContainer();
        //        requestWsReceita.CookieContainer.Add(new Cookie("ASP.NET_SessionId", requisicaoSefaz.SessionID, "", "www.nfe.fazenda.gov.br"));
        //        requestWsReceita.ContentLength = data.Length;
        //        requestWsReceita.Referer = URLReceita;

        //        using (var stream = requestWsReceita.GetRequestStream())
        //        {
        //            stream.Write(data, 0, data.Length);
        //        }

        //        var response = (HttpWebResponse)requestWsReceita.GetResponse();

        //        Servicos.Embarcador.NFe.ConsultaReceita serConsultaReceita = new Servicos.Embarcador.NFe.ConsultaReceita(Conexao.createInstance(_serviceProvider).StringConexao);
        //        if (response.ResponseUri.AbsolutePath == "/portal/consultaCompleta.aspx")
        //        {
        //            retorno.Objeto.NotaFiscal = serConsultaReceita.ProcessarHTMLRetorno(response.GetResponseStream());
        //            if (retorno.Objeto.NotaFiscal != null)
        //            {
        //                retorno.Objeto.NotaFiscal.Chave = ChaveNFe;
        //                retorno.Objeto.ConsultaValida = true;
        //                retorno.Objeto.MensagemSefaz = "";
        //            }
        //            else
        //            {
        //                retorno.Objeto.ConsultaValida = false;
        //                retorno.Objeto.MensagemSefaz = "NF-e INEXISTENTE na base nacional, favor consultar esta NF-e no site da SEFAZ de origem.";
        //            }
        //        }
        //        else
        //        {
        //            List<String> erros = serConsultaReceita.VerificarMensagemErro(response.GetResponseStream());
        //            string msg = "";
        //            foreach (string erro in erros)
        //            {
        //                msg += " " + erro;
        //            }
        //            retorno.Objeto.ConsultaValida = false;
        //            retorno.Objeto.MensagemSefaz = msg;
        //        }

        //        response.Close();
        //        response.Dispose();
        //    }
        //    catch (Exception ex)
        //    {
        //        Servicos.Log.TratarErro(ex);
        //        retorno.Status = false;
        //        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
        //        retorno.Mensagem = "Ocorreu uma falha ao consultar na nota no Sefaz";
        //    }
        //    return retorno;
        //}

        //private AdminMultisoftware.Dominio.Entidades.Proxy.Proxy ObterProximoProxy()
        //{
        //    AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).AdminStringConexao);
        //    try
        //    {
        //        AdminMultisoftware.Repositorio.Proxy.Proxy repProxy = new AdminMultisoftware.Repositorio.Proxy.Proxy(unitOfWork);
        //        AdminMultisoftware.Dominio.Entidades.Proxy.Proxy proxy = repProxy.BuscarProximo();
        //        return proxy;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        unitOfWork.Dispose();
        //    }
        //}

        //private AdminMultisoftware.Dominio.Entidades.Proxy.Proxy ObterProxyUtilizado(string IP)
        //{

        //    AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).AdminStringConexao);
        //    try
        //    {
        //        AdminMultisoftware.Repositorio.Proxy.Proxy repProxy = new AdminMultisoftware.Repositorio.Proxy.Proxy(unitOfWork);
        //        AdminMultisoftware.Dominio.Entidades.Proxy.Proxy proxy = repProxy.BuscarPorIP(IP);
        //        return proxy;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        unitOfWork.Dispose();
        //    }

        //}

        //private AdminMultisoftware.Dominio.Entidades.Proxy.Proxy ObterProxyChave(string chave)
        //{

        //    AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).AdminStringConexao);
        //    try
        //    {
        //        AdminMultisoftware.Repositorio.Proxy.Proxy repProxy = new AdminMultisoftware.Repositorio.Proxy.Proxy(unitOfWork);
        //        AdminMultisoftware.Dominio.Entidades.Proxy.Proxy proxy = repProxy.BuscarPorChave(chave);
        //        return proxy;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        unitOfWork.Dispose();
        //    }

        //}

        //private void SalvarProxy(AdminMultisoftware.Dominio.Entidades.Proxy.Proxy proxy)
        //{
        //    AdminMultisoftware.Repositorio.UnitOfWork unitOfWork = new AdminMultisoftware.Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).AdminStringConexao);
        //    try
        //    {
        //        AdminMultisoftware.Repositorio.Proxy.Proxy repProxy = new AdminMultisoftware.Repositorio.Proxy.Proxy(unitOfWork);
        //        repProxy.Atualizar(proxy);

        //        if (proxy.EmBloqueio)
        //        {
        //            AdminMultisoftware.Dominio.Entidades.Proxy.Proxy proxyBloqueado = repProxy.BuscarProximoBloqueado(1);
        //            if (proxyBloqueado != null)
        //            {
        //                proxyBloqueado.EmBloqueio = false;
        //                proxyBloqueado.DataUltimaRequisicaoValida = DateTime.Now;
        //                repProxy.Atualizar(proxyBloqueado);
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        unitOfWork.Dispose();
        //    }
        //}

        //public Retorno<Dominio.ObjetosDeValor.WebService.NFe.RequisicaoSefaz> SolicitarRequisicaoSefaz()
        //{
        //    ValidarToken();

        //    Retorno<Dominio.ObjetosDeValor.WebService.NFe.RequisicaoSefaz> retorno = new Retorno<Dominio.ObjetosDeValor.WebService.NFe.RequisicaoSefaz>();
        //    retorno.Status = true;
        //    retorno.Objeto = new Dominio.ObjetosDeValor.WebService.NFe.RequisicaoSefaz();
        //    retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        //    Servicos.Embarcador.NFe.ConsultaReceita serConsultaReceita = new Servicos.Embarcador.NFe.ConsultaReceita(Conexao.createInstance(_serviceProvider).StringConexao);
        //    Servicos.Embarcador.Documentos.Documento serDocumento = new Servicos.Embarcador.Documentos.Documento(Conexao.createInstance(_serviceProvider).StringConexao);

        //    AdminMultisoftware.Dominio.Entidades.Proxy.Proxy proxy = ObterProximoProxy();
        //    try
        //    {
        //        var request = (HttpWebRequest)WebRequest.Create(URLReceita);
        //        if (proxy != null)
        //        {
        //            WebProxy proxyObject = new WebProxy(proxy.IP, proxy.Porta);
        //            request.Proxy = proxyObject;
        //            retorno.Objeto.Proxy = proxy.IP;

        //        }

        //        request.CookieContainer = new CookieContainer();
        //        var response = (HttpWebResponse)request.GetResponse();

        //        foreach (Cookie cook in response.Cookies)
        //        {
        //            if (cook.Name == "ASP.NET_SessionId")
        //                retorno.Objeto.SessionID = cook.Value;
        //        }

        //        var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
        //        retorno.Objeto.VIEWSTATE = serDocumento.ExtractCampo(responseString, "__VIEWSTATE", "value");
        //        retorno.Objeto.EVENTVALIDATION = serDocumento.ExtractCampo(responseString, "__EVENTVALIDATION", "value");
        //        if(proxy != null)
        //            proxy.ChaveEmConsulta = retorno.Objeto.EVENTVALIDATION;

        //        retorno.Objeto.Captcha = serDocumento.ExtractCampo(responseString, "ctl00_ContentPlaceHolder1_imgCaptcha", "src");
        //        retorno.Objeto.TokenCaptcha = serDocumento.ExtractCampo(responseString, "ctl00$ContentPlaceHolder1$token", "value");

        //        proxy.DataUltimaRequisicaoValida = DateTime.Now;
        //        proxy.Uso++;
        //        response.Close();
        //        response.Dispose();
        //    }
        //    catch (Exception ex)
        //    {
        //        proxy.EmBloqueio = true;
        //        proxy.Databloqueio = DateTime.Now;
        //        Servicos.Log.TratarErro(ex);
        //        retorno.Status = false;
        //        retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
        //        retorno.Mensagem = "Ocorreu uma falha ao solicitar a requisição ao Sefaz";
        //    }
        //    SalvarProxy(proxy);
        //    return retorno;
        //}

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
