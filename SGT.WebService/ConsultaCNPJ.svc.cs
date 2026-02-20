using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using CoreWCF;
using System.Text;
using CoreWCF;

namespace SGT.WebService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, IncludeExceptionDetailInFaults = true)]
    public class ConsultaCNPJ(IServiceProvider _serviceProvider) : IConsultaCNPJ
    {
        //private string URLReceitaRequisicao = "http://www.receita.fazenda.gov.br/PessoaJuridica/CNPJ/cnpjreva/Cnpjreva_Solicitacao2.asp";
        private string URLReceitaRequisicao = "http://www.receita.fazenda.gov.br/PessoaJuridica/CNPJ/cnpjreva/Cnpjreva_solicitacao3.asp";
        private string URLReceitaRequisicaoChaptcha = "http://www.receita.fazenda.gov.br/PessoaJuridica/CNPJ/cnpjreva/captcha/gerarCaptcha.asp";

        private string URLValida = "http://www.receita.fazenda.gov.br/PessoaJuridica/CNPJ/cnpjreva/valida.asp";

        public Retorno<Dominio.ObjetosDeValor.WebService.Pessoa.ConsultaReceitaPessoaJuridica> ConsultarPessoaJuridicaFazenda(Dominio.ObjetosDeValor.WebService.Pessoa.RequisicaoFazendaPessoaJuridica requisicaoReceita, string CNPJ, string Captcha)
        {
            ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Servicos.Embarcador.Pessoa.ConsultaReceita serConsultaReceita = new Servicos.Embarcador.Pessoa.ConsultaReceita(unitOfWork);
            Retorno<Dominio.ObjetosDeValor.WebService.Pessoa.ConsultaReceitaPessoaJuridica> retorno = new Retorno<Dominio.ObjetosDeValor.WebService.Pessoa.ConsultaReceitaPessoaJuridica>();
            retorno.Status = true;
            retorno.Objeto = new Dominio.ObjetosDeValor.WebService.Pessoa.ConsultaReceitaPessoaJuridica();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            try
            {
                HttpWebRequest requestWsReceita = (HttpWebRequest)WebRequest.Create(URLValida);

                string postData = String.Format("origem={0}" +
                    "&cnpj={1}" +
                    "&txtTexto_captcha_serpro_gov_br={2}" +
                    "&submit1={3}" +
                    "&search_type={4}",
                    "comprovante",
                    CNPJ,
                    Captcha,
                    "Consultar",
                    "cnpj");

                var data = Encoding.UTF8.GetBytes(postData);

                requestWsReceita.Method = "POST";
                requestWsReceita.AllowAutoRedirect = true;
                requestWsReceita.Headers["User-Agent"] = "Mozilla / 5.0(Windows NT 10.0; WOW64; rv: 44.0) Gecko / 20100101 Firefox / 44.0";
                requestWsReceita.Host = "www.receita.fazenda.gov.br";
                requestWsReceita.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                requestWsReceita.ContentType = "application/x-www-form-urlencoded";

                requestWsReceita.CookieContainer = new CookieContainer();

                foreach (Dominio.ObjetosDeValor.WebService.CookieDinamico cookie in requisicaoReceita.Cookies)
                {
                    requestWsReceita.CookieContainer.Add(new Cookie(cookie.Name, cookie.Value, "", cookie.Domain));
                }

                requestWsReceita.ContentLength = data.Length;
                requestWsReceita.Referer = "http://www.receita.fazenda.gov.br/PessoaJuridica/CNPJ/cnpjreva/Cnpjreva_Solicitacao2.asp";

                using (var stream = requestWsReceita.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)requestWsReceita.GetResponse();
                if (response.ResponseUri.AbsolutePath == "/PessoaJuridica/CNPJ/cnpjreva/Cnpjreva_Comprovante.asp")
                {
                    retorno.Objeto.Pessoa = serConsultaReceita.ProcessarHTMLRetorno(response.GetResponseStream());
                    if (retorno.Objeto.Pessoa != null)
                    {
                        retorno.Objeto.ConsultaValida = true;
                    }
                    else
                    {
                        retorno.Objeto.ConsultaValida = false;
                        retorno.Objeto.MensagemReceita = "Não foi possível consultar os dados no site da Receita";
                    }
                }
                else
                {
                    retorno.Objeto.ConsultaValida = false;
                    if (response.ResponseUri.AbsolutePath == "/PessoaJuridica/CNPJ/cnpjreva/Cnpjreva_Erro.asp")
                    {
                        if (!string.IsNullOrWhiteSpace(response.ResponseUri.ToString().Split('=')[1]))
                            retorno.Objeto.MensagemReceita = response.ResponseUri.ToString().Split('=')[1];
                        else
                            retorno.Objeto.MensagemReceita = serConsultaReceita.VerificarMensagemErro(response.GetResponseStream());
                    }
                    else
                    {
                        retorno.Objeto.MensagemReceita = "Verifique se você informou o Captcha corretamente";
                    }
                }

                response.Close();
                response.Dispose();
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao consultar na nota no Sefaz";
            }
            finally
            {
                unitOfWork.Dispose();
            }
            return retorno;
        }

        public Retorno<Dominio.ObjetosDeValor.WebService.Pessoa.RequisicaoFazendaPessoaJuridica> SolicitarRequisicaoFazendaPessoaJuridica()
        {
            ValidarToken();
            Retorno<Dominio.ObjetosDeValor.WebService.Pessoa.RequisicaoFazendaPessoaJuridica> retorno = new Retorno<Dominio.ObjetosDeValor.WebService.Pessoa.RequisicaoFazendaPessoaJuridica>();
            retorno.Status = true;
            retorno.Objeto = new Dominio.ObjetosDeValor.WebService.Pessoa.RequisicaoFazendaPessoaJuridica();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(URLReceitaRequisicao);
                request.CookieContainer = new CookieContainer();
                var response = (HttpWebResponse)request.GetResponse();

                retorno.Objeto.Cookies = new List<Dominio.ObjetosDeValor.WebService.CookieDinamico>();
                foreach (Cookie cook in response.Cookies)
                {
                    Dominio.ObjetosDeValor.WebService.CookieDinamico cookie = new Dominio.ObjetosDeValor.WebService.CookieDinamico();
                    cookie.Name = cook.Name;
                    cookie.Value = cook.Value;
                    cookie.Domain = cook.Domain;
                    retorno.Objeto.Cookies.Add(cookie);
                }

                var requestCaptcha = (HttpWebRequest)WebRequest.Create(URLReceitaRequisicaoChaptcha);
                requestCaptcha.CookieContainer = new CookieContainer();
                requestCaptcha.CookieContainer.Add(response.Cookies);

                var responseCaptcha = (HttpWebResponse)requestCaptcha.GetResponse();
                retorno.Objeto.Captcha = ReadToEnd(responseCaptcha.GetResponseStream());

                response.Close();
                response.Dispose();
                responseCaptcha.Close();
                responseCaptcha.Dispose();

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao solicitar a requisição ao Sefaz";
            }
            return retorno;
        }

        string urlSintegraInscricao = "https://www.sefaz.rs.gov.br/NFE/NFE-CCC.aspx";

        public Retorno<string> ConsultarInscricaoSintegra(string CNPJ)
        {
            
            ValidarToken();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Servicos.Embarcador.Pessoa.ConsultaReceita serConsultaReceita = new Servicos.Embarcador.Pessoa.ConsultaReceita(unitOfWork);
            Retorno<string> retorno = new Retorno<string>();
            retorno.Status = true;
            retorno.Objeto = "";
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            Servicos.Embarcador.Documentos.Documento serDocumento = new Servicos.Embarcador.Documentos.Documento(unitOfWork);
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(urlSintegraInscricao);
                request.CookieContainer = new CookieContainer();
                var response = (HttpWebResponse)request.GetResponse();

                List<Dominio.ObjetosDeValor.WebService.CookieDinamico> cookies = new List<Dominio.ObjetosDeValor.WebService.CookieDinamico>();
                foreach (Cookie cook in response.Cookies)
                {
                    Dominio.ObjetosDeValor.WebService.CookieDinamico cookie = new Dominio.ObjetosDeValor.WebService.CookieDinamico();
                    cookie.Name = cook.Name;
                    cookie.Value = cook.Value;
                    cookie.Domain = cook.Domain;
                    cookies.Add(cookie);
                }

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                string key = serDocumento.ExtractJavaScript(responseString, "&key=", "\";");

                //var requestCaptcha = (HttpWebRequest)WebRequest.Create(URLReceitaRequisicaoChaptcha);
                //requestCaptcha.CookieContainer = new CookieContainer();
                //requestCaptcha.CookieContainer.Add(response.Cookies);

                //var responseCaptcha = (HttpWebResponse)requestCaptcha.GetResponse();
                //retorno.Objeto.Captcha = ReadToEnd(responseCaptcha.GetResponseStream());

                response.Close();
                response.Dispose();


                HttpWebRequest requestWsReceita = (HttpWebRequest)WebRequest.Create("https://www.sefaz.rs.gov.br/NFE/NFE-CCC_DO.aspx");

                string postData = String.Format("iCodUf={0}" +
                    "&key={1}" +
                    "&lCnpj={2}" +
                    "&pAmbiente={3}",
                    "0",
                    key,
                    CNPJ,
                    "1");

                var data = Encoding.UTF8.GetBytes(postData);

                requestWsReceita.Method = "POST";
                requestWsReceita.AllowAutoRedirect = true;
                requestWsReceita.Headers["User-Agent"] = "Mozilla / 5.0(Windows NT 10.0; WOW64; rv: 44.0) Gecko / 20100101 Firefox / 44.0";
                requestWsReceita.Host = "www.sefaz.rs.gov.br";
                requestWsReceita.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                requestWsReceita.ContentType = "application/x-www-form-urlencoded";

                requestWsReceita.CookieContainer = new CookieContainer();

                foreach (Dominio.ObjetosDeValor.WebService.CookieDinamico cookie in cookies)
                {
                    requestWsReceita.CookieContainer.Add(new Cookie(cookie.Name, cookie.Value, "", cookie.Domain));
                }

                requestWsReceita.ContentLength = data.Length;
                requestWsReceita.Referer = "https://www.sefaz.rs.gov.br/NFE/NFE-CCC.aspx?ErrKey=true&iCodUf=0&lCnpj=" + CNPJ + "&pAmbiente=1";

                using (var stream = requestWsReceita.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var responseSintegra = (HttpWebResponse)requestWsReceita.GetResponse();
                retorno.Objeto = ""; //serConsultaReceita.ProcessarHTMLSintegraRetorno(responseSintegra.GetResponseStream());

                responseSintegra.Close();
                responseSintegra.Dispose();

            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao solicitar a requisição ao Sefaz";
            }
            finally
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


        private byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }


        public Retorno<Dominio.ObjetosDeValor.WebService.Pessoa.ConsultaReceitaPessoaJuridica> ConsultarCadastroCentralizado(string CNPJ)
        {
            ValidarToken();

            CNPJ = Utilidades.String.OnlyNumbers(CNPJ).Trim();
            Repositorio.UnitOfWork unitOfWork = new Repositorio.UnitOfWork(Conexao.createInstance(_serviceProvider).StringConexao);
            Servicos.Embarcador.Pessoa.ConsultaReceita serConsultaReceita = new Servicos.Embarcador.Pessoa.ConsultaReceita(unitOfWork);
            Retorno<Dominio.ObjetosDeValor.WebService.Pessoa.ConsultaReceitaPessoaJuridica> retorno = new Retorno<Dominio.ObjetosDeValor.WebService.Pessoa.ConsultaReceitaPessoaJuridica>();
            retorno.Status = true;
            retorno.Objeto = new Dominio.ObjetosDeValor.WebService.Pessoa.ConsultaReceitaPessoaJuridica();
            retorno.DataRetorno = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            Servicos.Embarcador.Documentos.Documento serDocumento = new Servicos.Embarcador.Documentos.Documento(unitOfWork);
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(urlSintegraInscricao);
                request.CookieContainer = new CookieContainer();
                var response = (HttpWebResponse)request.GetResponse();

                List<Dominio.ObjetosDeValor.WebService.CookieDinamico> cookies = new List<Dominio.ObjetosDeValor.WebService.CookieDinamico>();
                foreach (Cookie cook in response.Cookies)
                {
                    Dominio.ObjetosDeValor.WebService.CookieDinamico cookie = new Dominio.ObjetosDeValor.WebService.CookieDinamico();
                    cookie.Name = cook.Name;
                    cookie.Value = cook.Value;
                    cookie.Domain = cook.Domain;
                    cookies.Add(cookie);
                }

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                string key = serDocumento.ExtractJavaScript(responseString, "&key=", "\";");

                response.Close();
                response.Dispose();


                HttpWebRequest requestWsReceita = (HttpWebRequest)WebRequest.Create("https://www.sefaz.rs.gov.br/NFE/NFE-CCC_DO.aspx");

                string postData = String.Format("iCodUf={0}" +
                    "&key={1}" +
                    "&lCnpj={2}" +
                    "&pAmbiente={3}",
                    "0",
                    key,
                    CNPJ,
                    "1");

                var data = Encoding.UTF8.GetBytes(postData);

                requestWsReceita.Method = "POST";
                requestWsReceita.AllowAutoRedirect = true;
                requestWsReceita.UserAgent = "Mozilla / 5.0(Windows NT 10.0; WOW64; rv: 44.0) Gecko / 20100101 Firefox / 44.0";
                requestWsReceita.Host = "www.sefaz.rs.gov.br";
                requestWsReceita.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                requestWsReceita.ContentType = "application/x-www-form-urlencoded";

                requestWsReceita.CookieContainer = new CookieContainer();

                foreach (Dominio.ObjetosDeValor.WebService.CookieDinamico cookie in cookies)
                {
                    requestWsReceita.CookieContainer.Add(new Cookie(cookie.Name, cookie.Value, "", cookie.Domain));
                }

                requestWsReceita.ContentLength = data.Length;
                requestWsReceita.Referer = "https://www.sefaz.rs.gov.br/NFE/NFE-CCC.aspx?ErrKey=true&iCodUf=0&lCnpj=" + CNPJ + "&pAmbiente=1";

                using (var stream = requestWsReceita.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var responseSintegra = (HttpWebResponse)requestWsReceita.GetResponse();

                List<Dominio.ObjetosDeValor.WebService.Pessoa.InscricaoSintegra> inscricoes = serConsultaReceita.ProcessarHTMLSintegraRetorno(responseSintegra.GetResponseStream());

                responseSintegra.Close();
                responseSintegra.Dispose();

                Dominio.ObjetosDeValor.WebService.Pessoa.InscricaoSintegra inscricaoValida = null;
                if (inscricoes.Count > 0)
                {
                    foreach (Dominio.ObjetosDeValor.WebService.Pessoa.InscricaoSintegra inscricao in inscricoes)
                    {
                        if (Utilidades.String.RemoveDiacritics(inscricao.Situacao).ToLower() == "habilitado")
                        {
                            inscricaoValida = inscricao;
                        }
                    }
                }
                else
                {
                    retorno.Status = false;
                    retorno.Mensagem = "Não foi localizado nenhum cadastro para o CNPJ informado na base centralizada do Sefaz";
                }

                if (inscricaoValida != null)
                {
                    var requestEstab = (HttpWebRequest)WebRequest.Create(inscricaoValida.LinkEstabelecimento);
                    requestEstab.CookieContainer = new CookieContainer();

                    foreach (Dominio.ObjetosDeValor.WebService.CookieDinamico cookie in cookies)
                    {
                        requestEstab.CookieContainer.Add(new Cookie(cookie.Name, cookie.Value, "", cookie.Domain));
                    }
                    requestEstab.Host = "www.sefaz.rs.gov.br";
                    requestEstab.Referer = inscricaoValida.LinkEstabelecimento;
                    var responseEstab = (HttpWebResponse)requestEstab.GetResponse();

                    retorno.Objeto.Pessoa = serConsultaReceita.ProcessarHTMLRetornoSintegra(responseEstab.GetResponseStream());
                    retorno.Objeto.Pessoa.CPFCNPJ = CNPJ;
                    retorno.Objeto.ConsultaValida = true;
                    responseEstab.Close();
                    responseEstab.Dispose();
                }
                else
                {
                    retorno.Status = false;
                    retorno.Mensagem = "O CNPJ consultado consta como não habilitado na base centralizada do Sefaz";
                }
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);
                retorno.Status = false;
                retorno.CodigoMensagem = Dominio.ObjetosDeValor.Embarcador.Enumeradores.CodigoMensagemRetorno.FalhaGenerica;
                retorno.Mensagem = "Ocorreu uma falha ao solicitar a requisição ao Sefaz";
            }
            finally
            {
                unitOfWork.Dispose();
            }
            return retorno;
        }


    }
}
