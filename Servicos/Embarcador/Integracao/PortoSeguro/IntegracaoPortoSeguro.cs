using Dominio.Entidades;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.PortoSeguro
{
    public class IntegracaoPortoSeguro
    {
        private static string BaseAddressPortoSeguro = "https://apis.averbeporto.com.br"; //"https://wws.averbeporto.com.br";//"http://www.averbeporto.com.br";
        private static string URLPortoSeguro = $"{BaseAddressPortoSeguro}/php/conn.php"; //$"{BaseAddressPortoSeguro}/websys/php/conn.php";

        #region Métodos Globais

        public static void CancelarAverbacaoDocumento(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apolice, Dominio.Entidades.AverbacaoCTe averbacao, ref int tentativas, Repositorio.UnitOfWork unitOfWork, string usuarioPorto, string senhaPorto)
        {
            Repositorio.Embarcador.Seguros.AverbacaoPortoSeguro repAverbacaoPortoSeguro = new Repositorio.Embarcador.Seguros.AverbacaoPortoSeguro(unitOfWork);
            Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);
            Repositorio.Embarcador.Averbacao.AverbacaoIntegracaoArquivo repAverbacaoIntegracaoArquivo = new Repositorio.Embarcador.Averbacao.AverbacaoIntegracaoArquivo(unitOfWork);

            Dominio.Entidades.Embarcador.Seguros.AverbacaoPortoSeguro averbacaoPortoSeguro = apolice != null ? repAverbacaoPortoSeguro.BuscarPorApolice(apolice.Codigo) : null;

            string xml = ObterXMLCancelamento(averbacao.CTe, averbacao.Protocolo, unitOfWork);

            try
            {
                if (averbacao.CTe != null && averbacao.CTe.OcorreuSinistroAvaria)
                {
                    averbacao.CodigoRetorno = "";
                    averbacao.MensagemRetorno = "Conhecimento com sinitro/avaria registrado, não é possível cancelar a sua averbação.";
                    averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso;
                    averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento;
                }
                else
                {
                    string sessionCookie = averbacaoPortoSeguro != null ? ObterSessionCookie(averbacaoPortoSeguro.Usuario, averbacaoPortoSeguro.Senha) : ObterSessionCookie(usuarioPorto, senhaPorto);

                    string jsonResult = EnviarXMLParaAverbacao(sessionCookie, xml);

                    Servicos.Log.TratarErro(jsonResult);

                    dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonResult);

                    if (retorno.success == 1)
                    {
                        if (retorno.S.P == 1 || retorno.S.D == 1)
                        {
                            averbacao.MensagemRetorno = "Cancelado com sucesso.";
                            averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Cancelado;
                            averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento;
                        }
                        else
                        {
                            averbacao.CodigoRetorno = (string)retorno.error.code;
                            averbacao.MensagemRetorno = (string)retorno.error.msg;
                            averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso;
                            averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento;
                        }
                    }
                    else
                    {
                        averbacao.CodigoRetorno = string.Empty;
                        averbacao.MensagemRetorno = (string)retorno.error.msg;
                        averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso;
                        averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento;
                    }

                    Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo averbacaoIntegracaoArquivo = new Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo
                    {
                        ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao("", "xml", unitOfWork),
                        ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResult, "xml", unitOfWork),
                        Data = DateTime.Now,
                        Mensagem = $"{averbacao.CodigoRetorno} - {averbacao.MensagemRetorno}",
                        Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
                    };

                    repAverbacaoIntegracaoArquivo.Inserir(averbacaoIntegracaoArquivo);
                    
                    if (averbacao.ArquivosTransacao != null)
                        averbacao.ArquivosTransacaoCancelamento.Add(averbacaoIntegracaoArquivo);
                }

                tentativas = 0;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                if (averbacao.tentativasIntegracao >= 1)
                {
                    averbacao.CodigoRetorno = "999";
                    averbacao.MensagemRetorno = "O serviço da Porto Seguro não está disponível no momento.";
                    averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso;
                    averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Cancelamento;
                    averbacao.tentativasIntegracao = 0;
                }
                else
                {
                    averbacao.tentativasIntegracao++;
                    tentativas++;
                }
            }

            averbacao.DataRetorno = DateTime.Now;

            repAverbacaoCTe.Atualizar(averbacao);
        }

        public static void AverbarDocumento(Dominio.Entidades.Embarcador.Seguros.ApoliceSeguro apolice, Dominio.Entidades.AverbacaoCTe averbacao, ref int tentativas, Repositorio.UnitOfWork unitOfWork, string usuarioPorto, string senhaPorto)
        {
            Repositorio.Embarcador.Seguros.AverbacaoPortoSeguro repAverbacaoPortoSeguro = new Repositorio.Embarcador.Seguros.AverbacaoPortoSeguro(unitOfWork);
            Repositorio.AverbacaoCTe repAverbacaoCTe = new Repositorio.AverbacaoCTe(unitOfWork);
            Repositorio.Embarcador.Averbacao.AverbacaoIntegracaoArquivo repAverbacaoIntegracaoArquivo = new Repositorio.Embarcador.Averbacao.AverbacaoIntegracaoArquivo(unitOfWork);

            Dominio.Entidades.Embarcador.Seguros.AverbacaoPortoSeguro averbacaoPortoSeguro = apolice != null ? repAverbacaoPortoSeguro.BuscarPorApolice(apolice.Codigo) : null;

            string xml = ObterXMLAutorizacao(averbacao.CTe, unitOfWork, averbacao.Forma == Dominio.Enumeradores.FormaAverbacaoCTE.Provisoria);
            string jsonResult = string.Empty;

            try
            {
                string sessionCookie = averbacaoPortoSeguro != null ? ObterSessionCookie(averbacaoPortoSeguro.Usuario, averbacaoPortoSeguro.Senha) : ObterSessionCookie(usuarioPorto, senhaPorto);

                jsonResult = EnviarXMLParaAverbacao(sessionCookie, xml);

                dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonResult);

                if (retorno?.success == 1)
                {
                    if (retorno.S?.P == 1 || retorno.S?.D == 1)
                    {
                        averbacao.Protocolo = (string)retorno.prot;
                        averbacao.Averbacao = (string)retorno.prot;
                        averbacao.CodigoRetorno = "0";
                        averbacao.MensagemRetorno = "Averbado com sucesso.";
                        averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Sucesso;
                        averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao;
                    }
                    else
                    {
                        Servicos.Log.TratarErro("Retorno: " + jsonResult, "PortoSeguro");

                        averbacao.CodigoRetorno = (string)retorno.error.code;
                        averbacao.MensagemRetorno = (string)retorno.error.msg;
                        averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Rejeicao;
                        averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao;
                    }
                }
                else
                {
                    Servicos.Log.TratarErro("Retorno: " + jsonResult, "PortoSeguro");

                    averbacao.CodigoRetorno = string.Empty;
                    averbacao.MensagemRetorno = (string)retorno.error.msg;
                    averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Rejeicao;
                    averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao;
                }

                Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo averbacaoIntegracaoArquivo = new Dominio.Entidades.Embarcador.Averbacao.AverbacaoIntegracaoArquivo
                {
                    ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao("", "json", unitOfWork),
                    ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResult, "json", unitOfWork),
                    Data = DateTime.Now,
                    Mensagem = $"{averbacao.CodigoRetorno} - {averbacao.MensagemRetorno}",
                    Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento
                };

                repAverbacaoIntegracaoArquivo.Inserir(averbacaoIntegracaoArquivo);

                if (averbacao.ArquivosTransacao != null)
                    averbacao.ArquivosTransacao.Add(averbacaoIntegracaoArquivo);

                tentativas = 0;
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrWhiteSpace(jsonResult))
                    Servicos.Log.TratarErro("Retorno: " + jsonResult, "PortoSeguro");
                Servicos.Log.TratarErro(ex, "PortoSeguro");

                if (averbacao.tentativasIntegracao >= 1)
                {
                    averbacao.CodigoRetorno = "999";
                    averbacao.MensagemRetorno = "O serviço da Porto Seguro não está disponível no momento.";
                    averbacao.Status = Dominio.Enumeradores.StatusAverbacaoCTe.Rejeicao;
                    averbacao.Tipo = Dominio.Enumeradores.TipoAverbacaoCTe.Autorizacao;
                    averbacao.tentativasIntegracao = 0;
                }
                else
                {
                    averbacao.tentativasIntegracao++;
                    tentativas++;
                }
            }

            averbacao.DataRetorno = DateTime.Now;

            repAverbacaoCTe.Atualizar(averbacao);
        }

        #endregion

        #region Métodos Privados

        private static string ObterSessionCookie(string usuario, string senha)
        {
            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoPortoSeguro));
            client.BaseAddress = new Uri(URLPortoSeguro);
            client.DefaultRequestHeaders.Add("cache-control", "no-cache");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("mod", "login"),
                new KeyValuePair<string, string>("comp", "5"),
                new KeyValuePair<string, string>("user", usuario),
                new KeyValuePair<string, string>("pass", senha)
            });

            HttpResponseMessage result = client.PostAsync(URLPortoSeguro, content).Result;

            string sessionCookie = string.Empty;

            if (result.IsSuccessStatusCode)
            {
                IEnumerable<string> cookies = result.Headers.FirstOrDefault(header => header.Key == "Set-Cookie").Value;

                if (cookies != null && cookies.Count() > 0)
                {
                    string[] splittedCookies = cookies.ElementAt(0).Split(';');

                    sessionCookie = splittedCookies.FirstOrDefault(o => o.StartsWith("portal[ses]"))?.Replace("portal[ses]=", "") ?? string.Empty;
                }
            }

            return sessionCookie;
        }

        private static string EnviarXMLParaAverbacao(string sessionCookie, string xml)
        {
            Uri baseAddress = new Uri(BaseAddressPortoSeguro);
            CookieContainer cookieContainer = new CookieContainer();
            // monta o header Cookie a partir do container
            cookieContainer.Add(baseAddress, new Cookie("portal[ses]", sessionCookie));
            string cookieHeader = cookieContainer.GetCookieHeader(baseAddress);

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoPortoSeguro));
            client.BaseAddress = baseAddress;

            StreamContent streamContent = new StreamContent(Utilidades.String.ToStream(xml));
            streamContent.Headers.Add("Content-Type", "application/xml");

            MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
            multipartFormDataContent.Add(streamContent, "file", "data.xml");

            var request = new HttpRequestMessage(HttpMethod.Post, URLPortoSeguro + "?comp=5&mod=Upload&path=eguarda/php/&dump=1")
            {
                Content = multipartFormDataContent
            };
            request.Headers.Add("Cookie", cookieHeader);

            HttpResponseMessage result = client.SendAsync(request).Result;

            return result.Content?.ReadAsStringAsync().Result;
        }

        private static string ObterXMLCancelamento(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, string protocolo, Repositorio.UnitOfWork unitOfWork)
        {
            if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe)
            {
                string modelo = "94";

                int.TryParse(cte.Numero.ToString() + "9", out int codigoCTe);

                string chaveCTe = ObterChaveCTe(out _, cte, modelo, codigoCTe);

                string xml = $@"<procEventoCTe versao=""3.00"" xmlns=""http://www.portalfiscal.inf.br/cte"">
	                                <eventoCTe versao=""3.00"">
		                                <infEvento Id=""ID110111{chaveCTe}01"">
			                                <cOrgao>35</cOrgao>
			                                <tpAmb>{cte.TipoAmbiente:D}</tpAmb>
			                                <CNPJ>{cte.Empresa.CNPJ}</CNPJ>
			                                <chCTe>{chaveCTe}</chCTe>
			                                <dhEvento>{cte.DataCancelamento:yyyy-MM-ddTHH:mm:ss}</dhEvento>
			                                <tpEvento>110111</tpEvento>
			                                <nSeqEvento>1</nSeqEvento>
			                                <detEvento versaoEvento=""3.00"">
				                                <evCancCTe>
					                                <descEvento>Cancelamento</descEvento>
					                                <nProt>{protocolo}</nProt>
					                                <xJust>{cte.ObservacaoCancelamento}</xJust>
				                                </evCancCTe>
			                                </detEvento>
		                                </infEvento>
	                                </eventoCTe>
                                </procEventoCTe>";

                return xml.ToString();
            }
            else
            {
                Servicos.CTe svcCTE = new Servicos.CTe(unitOfWork);
                return svcCTE.ObterStringXMLCancelamento(cte, unitOfWork);
            }
        }

        private static string ObterChaveCTe(out string digito, Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, string modelo, int codigoCTe)
        {
            StringBuilder chave = new StringBuilder();
            chave.Append(cte.Empresa.Localidade.Estado.CodigoIBGE);
            chave.Append(cte.DataEmissao.Value.ToString("yyMM"));
            chave.Append(Utilidades.String.OnlyNumbers(cte.Empresa.CNPJ));
            chave.Append(modelo);
            chave.Append(string.Format("{0:000}", cte.Serie.Numero));
            chave.Append(string.Format("{0:000000000}", cte.Numero));
            chave.Append("1");
            chave.Append(string.Format("{0:00000000}", codigoCTe));

            digito = Utilidades.Calc.Modulo11(chave.ToString()).ToString();

            chave.Append(digito);

            return chave.ToString();
        }

        private static string ObterXMLAutorizacao(Dominio.Entidades.ConhecimentoDeTransporteEletronico cte, Repositorio.UnitOfWork unitOfWork, bool averbacaoProvisoria)
        {
            if (cte.ModeloDocumentoFiscal.TipoDocumentoEmissao != Dominio.Enumeradores.TipoDocumento.CTe || averbacaoProvisoria)
            {
                string modelo = "94";

                int.TryParse(cte.Numero.ToString() + "9", out int codigoCTe);

                StringBuilder stXML = new StringBuilder();

                stXML.Append("<cteProc xmlns=\"http://www.portalfiscal.inf.br/cte\" versao=\"3.00\">");
                stXML.Append("<CTe xmlns=\"http://www.portalfiscal.inf.br/cte\">");
                stXML.Append($"<infCte Id=\"{ObterChaveCTe(out string digito, cte, modelo, codigoCTe)}\" versao=\"3.00\"><ide>");
                stXML.Append($"<cUF>{(cte.Empresa?.Localidade?.Estado?.CodigoIBGE.ToString("D") ?? "")}</cUF>");
                stXML.Append($"<cCT>{codigoCTe}</cCT>");
                stXML.Append("<CFOP>9999</CFOP>");
                stXML.Append("<natOp>PRESTACAO DE SERVICO DE TRANSPORTE</natOp>");
                stXML.Append("<forPag>2</forPag>");
                stXML.Append($"<mod>{modelo}</mod>");
                stXML.Append($"<serie>{cte.Serie.Numero}</serie>");
                stXML.Append($"<nCT>{cte.Numero}</nCT>");
                stXML.Append($"<dhEmi>{cte.DataEmissao.Value:yyyy-MM-ddTHH:mm:ss}</dhEmi>");
                stXML.Append("<tpImp>1</tpImp>");
                stXML.Append("<tpEmis>1</tpEmis>");
                stXML.Append($"<cDV>{digito}</cDV>");
                stXML.Append("<tpAmb>" + (int)cte.TipoAmbiente + "</tpAmb>");
                stXML.Append("<tpCTe>" + (int)cte.TipoCTE + "</tpCTe>");
                stXML.Append("<procEmi>0</procEmi>");
                stXML.Append("<verProc>3.00</verProc>");
                stXML.Append("<cMunEnv>" + (cte.Empresa?.Localidade?.CodigoIBGE.ToString("D") ?? "") + "</cMunEnv>");
                stXML.Append("<xMunEnv>" + (cte.Empresa?.Localidade?.Descricao ?? "") + "</xMunEnv>");
                stXML.Append("<UFEnv>" + (cte.Empresa?.Localidade?.Estado?.Sigla ?? "") + "</UFEnv>");
                stXML.Append("<modal>01</modal>");
                stXML.Append("<tpServ>" + (int)cte.TipoServico + "</tpServ>");
                stXML.Append("<cMunIni>" + cte.LocalidadeInicioPrestacao.CodigoIBGE + "</cMunIni>");
                stXML.Append("<xMunIni>" + cte.LocalidadeInicioPrestacao.Descricao + "</xMunIni>");
                stXML.Append("<UFIni>" + cte.LocalidadeInicioPrestacao.Estado.Sigla + "</UFIni>");
                stXML.Append("<cMunFim>" + cte.LocalidadeTerminoPrestacao.CodigoIBGE + "</cMunFim>");
                stXML.Append("<xMunFim>" + cte.LocalidadeTerminoPrestacao.Descricao + "</xMunFim>");
                stXML.Append("<UFFim>" + cte.LocalidadeTerminoPrestacao.Estado.Sigla + "</UFFim>");
                stXML.Append("<retira>1</retira>");
                stXML.Append("<indIEToma>1</indIEToma>");

                if (cte.TipoTomador == Dominio.Enumeradores.TipoTomador.Outros)
                {
                    stXML.Append("<toma4>");
                    stXML.Append("<toma>4</toma>");

                    if (cte.TomadorPagador.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica)
                        stXML.Append("<CPF>" + cte.TomadorPagador.CPF_CNPJ_SemFormato + "</CPF>");
                    else
                        stXML.Append("<CNPJ>" + cte.TomadorPagador.CPF_CNPJ_SemFormato + "</CNPJ>");

                    stXML.Append("<enderToma>");

                    if (cte.TomadorPagador.Exterior)
                    {
                        stXML.Append("<cMun>9999999</cMun>");
                        stXML.Append("<UF>EX</UF>");
                        stXML.Append("<cPais>" + cte.TomadorPagador.Pais.Codigo + "</cPais>");
                    }
                    else
                    {
                        stXML.Append("<cMun>" + cte.TomadorPagador.Localidade.CodigoIBGE + "</cMun>");
                        stXML.Append("<UF>" + cte.TomadorPagador.Localidade.Estado.Sigla + "</UF>");
                        stXML.Append("<cPais>" + cte.TomadorPagador.Localidade.Pais.Codigo + "</cPais>");
                    }

                    stXML.Append("</enderToma>");
                    stXML.Append("</toma4>");
                }
                else
                    stXML.Append("<toma03><toma>" + (int)cte.TipoTomador + "</toma></toma03>");

                stXML.Append("</ide>");

                stXML.Append("<emit>");
                stXML.Append("<CNPJ>" + cte.Empresa.CNPJ_SemFormato + "</CNPJ>");
                stXML.Append("<enderEmit>");
                stXML.Append("<cMun>" + cte.Empresa.Localidade.CodigoIBGE + "</cMun>");
                stXML.Append("<UF>" + cte.Empresa.Localidade.Estado.Sigla + "</UF>");
                stXML.Append("</enderEmit>");
                stXML.Append("</emit>");

                stXML.Append("<rem>");

                if (cte.Remetente.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica)
                    stXML.Append("<CPF>" + cte.Remetente.CPF_CNPJ_SemFormato + "</CPF>");
                else
                    stXML.Append("<CNPJ>" + cte.Remetente.CPF_CNPJ_SemFormato + "</CNPJ>");

                stXML.Append("<enderReme>");

                if (cte.Remetente.Exterior)
                {
                    stXML.Append("<cMun>9999999</cMun>");
                    stXML.Append("<UF>EX</UF>");
                    stXML.Append("<cPais>" + cte.Remetente.Pais.Codigo + "</cPais>");
                }
                else
                {
                    stXML.Append("<cMun>" + cte.Remetente.Localidade.CodigoIBGE + "</cMun>");
                    stXML.Append("<UF>" + cte.Remetente.Localidade.Estado.Sigla + "</UF>");
                    stXML.Append("<cPais>" + cte.Remetente.Localidade.Pais.Codigo + "</cPais>");
                }

                stXML.Append("</enderReme>");
                stXML.Append("</rem>");

                stXML.Append("<dest>");

                if (cte.Destinatario.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica)
                    stXML.Append("<CPF>" + cte.Destinatario.CPF_CNPJ_SemFormato + "</CPF>");
                else
                    stXML.Append("<CNPJ>" + cte.Destinatario.CPF_CNPJ_SemFormato + "</CNPJ>");

                stXML.Append("<enderDest>");

                if (cte.Destinatario.Exterior)
                {
                    stXML.Append("<cMun>9999999</cMun>");
                    stXML.Append("<UF>EX</UF>");
                    stXML.Append("<cPais>" + cte.Destinatario.Pais.Codigo + "</cPais>");
                }
                else
                {
                    stXML.Append("<cMun>" + cte.Destinatario.Localidade.CodigoIBGE + "</cMun>");
                    stXML.Append("<UF>" + cte.Destinatario.Localidade.Estado.Sigla + "</UF>");
                    stXML.Append("<cPais>" + cte.Destinatario.Localidade.Pais.Codigo + "</cPais>");
                }

                stXML.Append("</enderDest>");
                stXML.Append("</dest>");

                if (cte.Expedidor != null)
                {
                    stXML.Append("<exped>");

                    if (cte.Expedidor.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica)
                        stXML.Append("<CPF>" + cte.Expedidor.CPF_CNPJ_SemFormato + "</CPF>");
                    else
                        stXML.Append("<CNPJ>" + cte.Expedidor.CPF_CNPJ_SemFormato + "</CNPJ>");

                    stXML.Append("<enderExped>");

                    if (cte.Expedidor.Exterior)
                    {
                        stXML.Append("<cMun>9999999</cMun>");
                        stXML.Append("<UF>EX</UF>");
                        stXML.Append("<cPais>" + cte.Expedidor.Pais.Codigo + "</cPais>");
                    }
                    else
                    {
                        stXML.Append("<cMun>" + cte.Expedidor.Localidade.CodigoIBGE + "</cMun>");
                        stXML.Append("<UF>" + cte.Expedidor.Localidade.Estado.Sigla + "</UF>");
                        stXML.Append("<cPais>" + cte.Expedidor.Localidade.Pais.Codigo + "</cPais>");
                    }

                    stXML.Append("</enderExped>");
                    stXML.Append("</exped>");
                }

                if (cte.Recebedor != null)
                {
                    stXML.Append("<receb>");

                    if (cte.Recebedor.Tipo == Dominio.Enumeradores.TipoPessoa.Fisica)
                        stXML.Append("<CPF>" + cte.Recebedor.CPF_CNPJ_SemFormato + "</CPF>");
                    else
                        stXML.Append("<CNPJ>" + cte.Recebedor.CPF_CNPJ_SemFormato + "</CNPJ>");

                    stXML.Append("<enderReceb>");

                    if (cte.Recebedor.Exterior)
                    {
                        stXML.Append("<cMun>9999999</cMun>");
                        stXML.Append("<UF>EX</UF>");
                        stXML.Append("<cPais>" + cte.Recebedor.Pais.Codigo + "</cPais>");
                    }
                    else
                    {
                        stXML.Append("<cMun>" + cte.Recebedor.Localidade.CodigoIBGE + "</cMun>");
                        stXML.Append("<UF>" + cte.Recebedor.Localidade.Estado.Sigla + "</UF>");
                        stXML.Append("<cPais>" + cte.Recebedor.Localidade.Pais.Codigo + "</cPais>");
                    }

                    stXML.Append("</enderReceb>");
                    stXML.Append("</receb>");
                }

                stXML.Append("<infCTeNorm><infCarga>");

                string userInfo = "en-US";
                stXML.Append("<vCarga>" + cte.ValorTotalMercadoria.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture(userInfo)).Replace(",", "") + "</vCarga>");

                stXML.Append("<proPred>" + cte.ProdutoPredominante + "</proPred>");

                if (cte.QuantidadesCarga != null && cte.QuantidadesCarga.Count > 0)
                {
                    foreach (InformacaoCargaCTE volume in cte.QuantidadesCarga)
                    {
                        stXML.Append("<infQ>");
                        stXML.Append("<cUnid>" + volume.UnidadeMedida + "</cUnid>");
                        stXML.Append("<tpMed>" + volume.Tipo + "</tpMed>");
                        stXML.Append("<qCarga>" + volume.Quantidade.ToString("N4", System.Globalization.CultureInfo.CreateSpecificCulture(userInfo)).Replace(",", "") + "</qCarga>");
                        stXML.Append("</infQ>");
                    }
                }

                stXML.Append("</infCarga>");

                if (cte.Seguros.Count > 0)
                {
                    Dominio.Entidades.SeguroCTE seguro = cte.Seguros.FirstOrDefault();
                    stXML.Append("<seg>");
                    stXML.Append("<respSeg>" + (int)seguro.Tipo + "</respSeg>");
                    stXML.Append("<vCarga>" + seguro.Valor.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture(userInfo)).Replace(",", "") + "</vCarga>");
                    stXML.Append("</seg>");
                }

                stXML.Append("</infCTeNorm>");
                stXML.Append("</infCte></CTe>");
                stXML.Append("</cteProc>");

                return stXML.ToString();
            }
            else
            {
                Servicos.CTe svcCTE = new Servicos.CTe(unitOfWork);
                return svcCTE.ObterStringXMLAutorizacao(cte, unitOfWork);
            }
        }

        #endregion
    }
}
