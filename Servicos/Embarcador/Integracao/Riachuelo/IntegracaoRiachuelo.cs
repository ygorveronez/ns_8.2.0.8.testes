using Dominio.Excecoes.Embarcador;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Riachuelo
{
    public class IntegracaoRiachuelo
    {
        #region Métodos Públicos

        public static void IntegrarFinalizacaoCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao cargaIntegracao, Repositorio.UnitOfWork unitOfWork)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.Empresa repEmpresa = new Repositorio.Empresa(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCargaIntegracao repositorioCargaIntegracao = new Repositorio.Embarcador.Cargas.CargaCargaIntegracao(unitOfWork);
            Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo repCargaCTeIntegracaoArquivo = new Repositorio.Embarcador.Cargas.CargaCTeIntegracaoArquivo(unitOfWork);
            Repositorio.Embarcador.Configuracao.ConfiguracaoTMS repConfiguracaoTMS = new Repositorio.Embarcador.Configuracao.ConfiguracaoTMS(unitOfWork);
            Repositorio.Embarcador.Cargas.Carga repCarga = new Repositorio.Embarcador.Cargas.Carga(unitOfWork);
            Repositorio.Embarcador.Cargas.CargaPedido repCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
            Dominio.Entidades.Embarcador.Configuracao.ConfiguracaoTMS configuracaoTMS = repConfiguracaoTMS.BuscarConfiguracaoPadrao();

            cargaIntegracao.NumeroTentativas += 1;
            cargaIntegracao.DataIntegracao = DateTime.Now;

            if ((configuracaoIntegracao == null) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLIntegracaoRiachuelo))
            {
                cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaIntegracao.ProblemaIntegracao = "Não existe configuração de integração disponível para a Riachuelo.";
            }
            else
            {
                string apiKey = configuracaoIntegracao.ApiKeyRiachuelo; //"SggGaK3k7LgfS33tDDpzw6MoGriWu9Y5L2a512uI3ROLBisuilJFbSVgMGFUVs9e";
                string applicationID = configuracaoIntegracao.AppIDRiachuelo; // "5930d6cd-d1cf-4337-9dde-774f993714e2";                
                string urlToken = configuracaoIntegracao.URLTokenRiachuelo;// "https://9hyxh9dsj1.execute-api.us-east-1.amazonaws.com/v1/";//

                ////string userName = "ay7LV1vVctLI2dEeac3DgsQeory411BU";
                //Para obter o LoginCrypt deve-se utilizar o Postman e verificar pelo console a string gerada a partir do Login
                ////string password = "1iNy~mw7iE|!S2{n";
                ////string pubkey = "LS0tLS1CRUdJTiBQVUJMSUMgS0VZLS0tLS0KTUlJQ0lqQU5CZ2txaGtpRzl3MEJBUUVGQUFPQ0FnOEFNSUlDQ2dLQ0FnRUF6b2VFVWYwbW10WTRqOGRXVGk2RAovM2NiRC9hWFM4dDJ5UStsK2NUZVU3Q3lxNTBVUTJVcnEwaWtpZnZ6NzlIaDhSRXdtcHRHQVlRSFRlbm5jb0pXCklTeXpidkc5SmtCZ1lTSVMyZmR2dHh3N01vQm82WmF3cE1KRUFTc1pTV1c4Wk1MRTVWalI5NE1QWkQwbWlMSzgKanhiL0RvYzlxN2JJOUxmQ0w0MzhsaUNaVU81S0tQbFFEZmptNkl4RFJHek5IWHl5S0VGMlZEckFDM0cxWnd3NQozVzVUZnlJMWl3VXQ0R0x3T2U2clBFZWNsemxVVFlFZTFKMHdTSWVkd1ZlOW1laHlyNGhXU1NpbE1GQTRrTVlVCmlGa211eW5EVi9SVW04VG5QYzVGbG9ueDNlTWFENDBPdklIWnpWeVFEVnBOODUxY1lwM3BpdUk3TDF6WE10akYKTzU0SHBRME92dmZvSjlwbXF4WEFmVENCUlA3TlJQWWJ6WElFUjZWMDZ3TjRDVERmUGhkTkhPOUN3bERmYmppRwpOd0RkbmF5Z2EwU3Z6YnV5M2dEcjFlTTBtSFpqcVMrNlhEMHBMVHJhS3ZpVEw2Qmw1dGVOYmo0dGExcUhLNnJxCllXdnFXbTRRelBldC90bXduRnZOK2FEaDc1ak1JMi9OSXZmamhWcjlhYnNOTi9FU1p5aFgydCtoWlhiOWF6OUIKYUl0dHlCYVlrb01YUTQ0SmU2S2wraHFuUnJ4b1Z4Uyt0eVFtVlcveWJQNnE2Z1NNU3dLQVphNDJIUHUzQU56eAprcWNyVVhmb2xNUHlGYnBSdUR4bHlxb0FCOFM4TE5mdEV4R0k5WW50cjZCRHplTElFWUhTOTRKUElzMWt0RjFvCmRSanFNbmlmM0hBL1JNVGcxMUJoUWdVQ0F3RUFBUT09Ci0tLS0tRU5EIFBVQkxJQyBLRVktLS0tLQo=";
                string loginCript = configuracaoIntegracao.LoginCriptRiachuelo;// "QrfrH1fls3Aft+MP4vg9i7EVnuRXF7yY480R5vlOaYkd1n6rnMVwPAp1MQtHXE74s87QVQHPQYupXrcZgSYmNqUN8BpTpVPPLU4gpka72D96w9SjRd/J3u5OyUVmijAx+bGNvS4j2+eV9M4DbpKXlIWumzHtmmkjE6doOp5nTxiLdbJkyc6T+9DOa8QKwmPm5xg6fgK0ypxV9cnPZEINFefjSflcXOQPS32ookJ+5VWbiubtojx98Xq4t4Mi9hvKOrHDvIJcF6VgQVbWwKS1/7a9nGtJ1yG+iGPq+QP1JmrcNtlExJX7b4n/gKresCfbObH3j/85m3op+E/41cbIYj8EAA/wGcYHh3KDKXjbKqwU8Za4VzYZBAifXtDqNawS+WG6ypwidnxf2jzKxRX3NsZOSyhpiwmVWveqvOQ1AO/svgxrWs9c5Fvo71N/sYBuCncKZsoyajNaw/QFjmJ8TXIabAbHVEw68gMHaIBcSWs1REBci0NPShGosW7JKo7moRLRd27a6r6MfSVJUg+m5TIohqW5SUCGBynbBUDlh8CBvjDPR0YnBpDqdqF9E91AP5Z4lB0EiHvBvw211wCZd88pVeaKi+7gD3UeTn3x4gCA3drMuJnSgRG8Y/80xayKKRm6FDyL78NirP2qBHMQspddMdxhimFmAd9hozEo3Ik="

                string tokenID = ObterToken(urlToken, apiKey, applicationID, loginCript);

                if (!string.IsNullOrWhiteSpace(tokenID))
                {
                    string endPoint = configuracaoIntegracao.URLIntegracaoRiachuelo;
                    string mensagemErro = string.Empty;

                    HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoRiachuelo));

                    client.BaseAddress = new Uri(endPoint);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("x-api-key", apiKey);
                    client.DefaultRequestHeaders.Add("x-app-token", tokenID);
                    client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");

                    string jsonRequest = string.Empty;
                    string jsonResponse = string.Empty;
                    bool situacaoIntegracao = false;

                    try
                    {
                        List<int> listaCarga = repCargaPedido.BuscarCodigoCargasOriginais(cargaIntegracao.Carga.Codigo);

                        foreach (int codigoCarga in listaCarga)
                        {
                            Dominio.Entidades.Embarcador.Cargas.Carga carga = repCarga.BuscarPorCodigo(codigoCarga);

                            if (carga != null)
                            {
                                jsonRequest = carga.Protocolo.ToString();

                                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                                var result = client.PostAsync(endPoint, content).Result;
                                jsonResponse = result.Content.ReadAsStringAsync().Result;

                                if (result.IsSuccessStatusCode)
                                {
                                    Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoRiachuelo");
                                    Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoRiachuelo");
                                    Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoRiachuelo");


                                    var retorno = (string)(result.Content.ReadAsStringAsync().Result);
                                    if (!string.IsNullOrWhiteSpace(retorno))
                                    {
                                        dynamic retornoJSON = JsonConvert.DeserializeObject<dynamic>(retorno);

                                        if (retornoJSON == null)
                                            throw new ServicoException("Integração Riachuelo não retornou JSON.");

                                        if ((bool)retornoJSON.result)
                                        {
                                            situacaoIntegracao = true;
                                            mensagemErro = string.Empty;

                                            Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                                            arquivoIntegracao.Data = cargaIntegracao.DataIntegracao;
                                            arquivoIntegracao.Mensagem = mensagemErro;
                                            arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;

                                            arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                                            arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                                            repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                                            cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);
                                        }
                                        else
                                        {
                                            mensagemErro = "Retorno Riachuelo: " + (string)retornoJSON.message;
                                            throw new ServicoException(!string.IsNullOrWhiteSpace(mensagemErro) ? mensagemErro : "Integração Riachuelo retornou falha.");
                                        }
                                    }
                                    else
                                        throw new ServicoException("Integração Riachuelo não teve retorno.");

                                }
                                else
                                    throw new ServicoException("Retorno integração riachuelo: " + result.StatusCode.ToString());
                            }
                            else
                                throw new ServicoException("Carga não localizada para retorno.");
                        }

                    }
                    catch (Exception excecao)
                    {
                        Servicos.Log.TratarErro("IntegrarSituacaoCTe: " + excecao.Message, "IntegracaoRiachuelo");

                        Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoRiachuelo");
                        Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoRiachuelo");
                        Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoRiachuelo");

                        mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da Riachuelo.";
                        situacaoIntegracao = false;

                        Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo();
                        arquivoIntegracao.Data = cargaIntegracao.DataIntegracao;
                        arquivoIntegracao.Mensagem = excecao.Message.Length > 400 ? excecao.Message.Substring(0, 400) : excecao.Message;
                        arquivoIntegracao.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;


                        arquivoIntegracao.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequest, "json", unitOfWork);
                        arquivoIntegracao.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(jsonResponse, "json", unitOfWork);

                        repCargaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

                        cargaIntegracao.ArquivosTransacao.Add(arquivoIntegracao);

                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        cargaIntegracao.ProblemaIntegracao = mensagemErro;
                        repositorioCargaIntegracao.Atualizar(cargaIntegracao);
                        return;
                    }

                    if (!situacaoIntegracao)
                    {
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                        cargaIntegracao.ProblemaIntegracao = mensagemErro;
                    }
                    else
                    {
                        cargaIntegracao.ProblemaIntegracao = string.Empty;
                        cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    }
                }
                else
                {
                    cargaIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                    cargaIntegracao.ProblemaIntegracao = "Não foi possível obter token.";
                }
            }
            repositorioCargaIntegracao.Atualizar(cargaIntegracao);
        }

        public static void IntegrarNFesEntregues(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao integracao, Repositorio.UnitOfWork unitOfWork)
        {
            Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe repCargaPedidoXMLNotaFiscalCTe = new Repositorio.Embarcador.Cargas.CargaPedidoXMLNotaFiscalCTe(unitOfWork);
            List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> listaDocumentosCTe = repCargaPedidoXMLNotaFiscalCTe.BuscarXMLNotasFiscaisPorCTe(integracao.CargaCTe.Codigo);

            Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = IntegrarNFesEntregues(listaDocumentosCTe, unitOfWork, integracao);

            integracao.NumeroTentativas += 1;
            integracao.DataIntegracao = DateTime.Now;
            integracao.ProblemaIntegracao = httpRequisicaoResposta.mensagem;
            integracao.SituacaoIntegracao = (httpRequisicaoResposta.sucesso) ? Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado : Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;

            SalvarArquivoIntegracaoOcorrencia(integracao, httpRequisicaoResposta.conteudoRequisicao, httpRequisicaoResposta.conteudoResposta, httpRequisicaoResposta.mensagem, unitOfWork);

            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao repositorioOcorrenciaCTeIntegracao = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao(unitOfWork);
            repositorioOcorrenciaCTeIntegracao.Atualizar(integracao);
        }

        public static Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta IntegrarNFesEntregues(List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais, Repositorio.UnitOfWork unitOfWork, Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao integracao = null)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = new Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta()
            {
                conteudoRequisicao = string.Empty,
                extensaoRequisicao = "json",
                conteudoResposta = string.Empty,
                extensaoResposta = "json",
                sucesso = false,
                mensagem = string.Empty,
            };

            if (notasFiscais == null)
            {
                httpRequisicaoResposta.mensagem = "Nenhuma NFe localizada.";
            }
            else
            {
                Repositorio.Embarcador.Configuracoes.Integracao repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.Integracao(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega repOcorrenciaColetaEntrega = new Repositorio.Embarcador.Cargas.ControleEntrega.OcorrenciaColetaEntrega(unitOfWork);
                Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal repositorioCargaEntregaNotaFiscal = new Repositorio.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal(unitOfWork);

                Dominio.Entidades.Embarcador.Configuracoes.Integracao configuracaoIntegracao = repositorioConfiguracaoIntegracao.Buscar();
                List<Dominio.Entidades.Embarcador.Cargas.ControleEntrega.CargaEntregaNotaFiscal> cargasEntrega = repositorioCargaEntregaNotaFiscal.BuscarTodasCargaEntregaPorXMLNotasFiscais(notasFiscais.Select(obj => obj.Codigo).ToList());

                if ((configuracaoIntegracao == null) || string.IsNullOrWhiteSpace(configuracaoIntegracao.URLIntegracaoEntregaNFeRiachuelo))
                {
                    httpRequisicaoResposta.mensagem = "Não existe configuração de integração disponível para a Riachuelo.";
                }
                else
                {
                    string apiKey = configuracaoIntegracao.ApiKeyRiachuelo;
                    string applicationID = configuracaoIntegracao.AppIDRiachuelo;
                    string urlToken = configuracaoIntegracao.URLTokenRiachuelo;
                    string loginCript = configuracaoIntegracao.LoginCriptRiachuelo;
                    string endPoint = configuracaoIntegracao.URLIntegracaoEntregaNFeRiachuelo;
                    string tokenID = ObterToken(urlToken, apiKey, applicationID, loginCript);

                    if (string.IsNullOrWhiteSpace(tokenID))
                        throw new ServicoException("Riachuelo não retornou Token.");

                    bool ocorrenciaEmTransito = integracao?.CargaOcorrencia?.TipoOcorrencia?.CodigoIntegracao == "099";
                    bool ocorrenciaEntregaRealizada = integracao?.CargaOcorrencia?.TipoOcorrencia?.CodigoIntegracao == "001";

                    string jsonRequest = string.Empty, jsonResponse = string.Empty;
                    try
                    {
                        HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoRiachuelo));
                        client.BaseAddress = new Uri(endPoint);

                        // Headers
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("x-api-key", apiKey);
                        client.DefaultRequestHeaders.Add("x-app-token", tokenID);
                        client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");

                        Servicos.Log.TratarErro(" ------- Gerando - Integração ------- ", "IntegracaoRiachuelo");

                        Dominio.ObjetosDeValor.Embarcador.Integracao.Riachuelo.NFesEntregues nfesEntregues = new Dominio.ObjetosDeValor.Embarcador.Integracao.Riachuelo.NFesEntregues();
                        nfesEntregues.invoices = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Riachuelo.NFe>();
                        foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal in notasFiscais)
                        {
                            Dominio.ObjetosDeValor.Embarcador.Integracao.Riachuelo.NFe invoiceNotaFiscal = new Dominio.ObjetosDeValor.Embarcador.Integracao.Riachuelo.NFe();
                            invoiceNotaFiscal.branch = notaFiscal.Destinatario.CodigoIntegracao;
                            invoiceNotaFiscal.bukrs = notaFiscal.Emitente.CodigoIntegracao;
                            invoiceNotaFiscal.message = string.Empty;
                            invoiceNotaFiscal.nfeKey = notaFiscal.Chave;
                            invoiceNotaFiscal.nfeYear = Utilidades.Chave.ObterAno(notaFiscal.Chave);
                            invoiceNotaFiscal.number = notaFiscal.Numero.ToString();
                            invoiceNotaFiscal.serie = Utilidades.Chave.ObterSerie(notaFiscal.Chave);
                            invoiceNotaFiscal.status = "SUCCESS";
                            if (configuracaoIntegracao?.HabilitarDataSaidaCDLoja ?? false)
                            {
                                invoiceNotaFiscal.carga = integracao?.CargaCTe.Carga.CodigoCargaEmbarcador;
                                invoiceNotaFiscal.cargaSaidaCD = ocorrenciaEntregaRealizada ? "false" : "true";
                                if (!ocorrenciaEntregaRealizada)
                                    invoiceNotaFiscal.dtCargaSaidaCD = integracao?.CargaCTe.Carga.DataInicioViagem?.ToString("dd/MM/yyyy HH:mm") ?? null;
                                invoiceNotaFiscal.cargaSaidaLoja = ocorrenciaEmTransito ? "false" : "true";
                                if (!ocorrenciaEmTransito)
                                    invoiceNotaFiscal.dtCargaSaidaLoja = integracao?.CargaOcorrencia.DataOcorrencia.ToString("dd/MM/yyyy HH:mm") ?? null;
                            }

                            Servicos.Log.TratarErro($"Gerando Nota: {notaFiscal.Codigo} ocorrenciaColetaEntrega: {integracao?.CargaOcorrencia?.Codigo ?? 0} ", "IntegracaoRiachuelo");

                            nfesEntregues.invoices.Add(invoiceNotaFiscal);
                        }

                        Servicos.Log.TratarErro(" ------- Gerado - Integração ------- ", "IntegracaoRiachuelo");

                        jsonRequest = JsonConvert.SerializeObject(nfesEntregues, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                        // Request
                        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                        var result = client.PostAsync(endPoint, content).Result;

                        // Response
                        jsonResponse = result.Content.ReadAsStringAsync().Result;

                        httpRequisicaoResposta.conteudoRequisicao = jsonRequest;
                        httpRequisicaoResposta.conteudoResposta = jsonResponse;
                        httpRequisicaoResposta.httpStatusCode = result.StatusCode;

                        Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoRiachuelo");
                        Servicos.Log.TratarErro("Request: " + jsonRequest, "IntegracaoRiachuelo");
                        Servicos.Log.TratarErro("Response: " + jsonResponse, "IntegracaoRiachuelo");

                        if (result.IsSuccessStatusCode)
                        {
                            string retorno = result.Content.ReadAsStringAsync().Result;
                            if (!string.IsNullOrWhiteSpace(retorno))
                            {
                                dynamic retornoJSON = JsonConvert.DeserializeObject<dynamic>(retorno);
                                if (retornoJSON == null)
                                {
                                    httpRequisicaoResposta.mensagem = "Integração Riachuelo não retornou JSON.";
                                }
                                else
                                {
                                    httpRequisicaoResposta.mensagem = (string)retornoJSON.message ?? string.Empty;

                                    if (retornoJSON.status == "SUCCESS")
                                    {
                                        httpRequisicaoResposta.sucesso = true;
                                        DateTime dataIntegracao = DateTime.Now;
                                        Repositorio.Embarcador.Canhotos.Canhoto repCanhoto = new Repositorio.Embarcador.Canhotos.Canhoto(unitOfWork);
                                        foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal in notasFiscais)
                                        {
                                            Dominio.Entidades.Embarcador.Canhotos.Canhoto canhoto = notaFiscal.Canhoto;
                                            if (canhoto != null)
                                            {
                                                canhoto.DataIntegracaoEntrega = dataIntegracao;
                                                repCanhoto.Atualizar(canhoto);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        httpRequisicaoResposta.mensagem = "Retorno Riachuelo: " + (string)retornoJSON.message ?? string.Empty;
                                    }
                                }
                            }
                            else
                            {
                                httpRequisicaoResposta.mensagem = "Integração Riachuelo não teve retorno.";
                            }
                        }
                        else
                        {
                            httpRequisicaoResposta.mensagem = "Retorno integração riachuelo: " + result.StatusCode.ToString();
                        }

                        if (!httpRequisicaoResposta.sucesso && string.IsNullOrWhiteSpace(httpRequisicaoResposta.mensagem))
                            httpRequisicaoResposta.mensagem = "Integração riachuelo não retornou sucesso.";
                    }
                    catch (Exception excecao)
                    {
                        Log.TratarErro("Request: " + jsonRequest, "IntegracaoRiachuelo");
                        Log.TratarErro("Response: " + jsonResponse, "IntegracaoRiachuelo");
                        Log.TratarErro(excecao, "IntegracaoRiachuelo");
                        httpRequisicaoResposta.mensagem = "Ocorreu uma falha ao comunicar com o Serviço da Riachuelo.";
                    }
                }
            }

            return httpRequisicaoResposta;
        }

        #endregion

        #region Métodos Privados

        private static string ObterToken(string urlToken, string apiKey, string applicationID, string loginCript)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string endPoint = urlToken + "/" + applicationID + "/get-token";
            string mensagemErro = string.Empty;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoRiachuelo));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("x-api-key", apiKey);

            try
            {
                string jsonResponse = string.Empty;
                Dominio.ObjetosDeValor.Embarcador.Integracao.Riachuelo.RequestToken requestJson = new Dominio.ObjetosDeValor.Embarcador.Integracao.Riachuelo.RequestToken();

                requestJson.value = loginCript;
                string jsonRequest = JsonConvert.SerializeObject(requestJson, Formatting.Indented);

                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                var result = client.PostAsync(endPoint, content).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                if (result.IsSuccessStatusCode)
                {
                    Servicos.Log.TratarErro("get-token URL: " + endPoint, "IntegracaoRiachuelo");
                    Servicos.Log.TratarErro("get-token Request: " + jsonRequest, "IntegracaoRiachuelo");
                    Servicos.Log.TratarErro("get-token Response: " + jsonResponse, "IntegracaoRiachuelo");

                    var retorno = (string)(result.Content.ReadAsStringAsync().Result);
                    if (!string.IsNullOrWhiteSpace(retorno))
                    {
                        dynamic objetoRetorno = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(retorno);

                        string tokenRetorno = (string)objetoRetorno.IdToken;
                        if (string.IsNullOrWhiteSpace(tokenRetorno))
                        {
                            if ((string)objetoRetorno.ChallengeName == "NEW_PASSWORD_REQUIRED")
                            {
                                string session = (string)objetoRetorno.Session;
                                if (!string.IsNullOrWhiteSpace(session))
                                {
                                    string endPointChallengResponse = urlToken + "/" + applicationID + "/challenge-response";

                                    Servicos.Log.TratarErro("challenge-response URL: " + endPoint, "IntegracaoRiachuelo");

                                    Dominio.ObjetosDeValor.Embarcador.Integracao.Riachuelo.ChallengeResponse challengeResponse = new Dominio.ObjetosDeValor.Embarcador.Integracao.Riachuelo.ChallengeResponse();
                                    challengeResponse.ChallengeName = "NEW_PASSWORD_REQUIRED";
                                    challengeResponse.Session = session;
                                    challengeResponse.ChallengeResponses = loginCript;

                                    jsonRequest = JsonConvert.SerializeObject(challengeResponse, Formatting.Indented);
                                    Servicos.Log.TratarErro("challenge-response Request: " + jsonRequest, "IntegracaoRiachuelo");

                                    content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                                    result = client.PostAsync(endPointChallengResponse, content).Result;
                                    jsonResponse = result.Content.ReadAsStringAsync().Result;
                                    Servicos.Log.TratarErro("challenge-response Response: " + jsonResponse, "IntegracaoRiachuelo");

                                    var retornoChallengResponse = (string)(result.Content.ReadAsStringAsync().Result);
                                    if (result.IsSuccessStatusCode)
                                    {
                                        //dynamic objetoRetornoChallengResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(retornoChallengResponse);

                                        mensagemErro = "ChallengResponse: sem retorno";
                                        Servicos.Log.TratarErro("ChallengResponse: sem retorno", "IntegracaoRiachuelo");
                                        return string.Empty;
                                    }
                                    else
                                    {
                                        mensagemErro = "Retorno ChallengResponse: " + result.StatusCode.ToString();
                                        Servicos.Log.TratarErro("ChallengResponse: " + result.StatusCode.ToString(), "IntegracaoRiachuelo");
                                        return string.Empty;
                                    }
                                }
                                else
                                {
                                    mensagemErro = "get-token: não teve retorno session para ChallengResponse";
                                    Servicos.Log.TratarErro("get-token: não teve retorno session para ChallengResponse", "IntegracaoRiachuelo");
                                    return string.Empty;
                                }
                            }
                        }

                        return tokenRetorno;
                    }
                    else
                    {
                        mensagemErro = "get-token: não teve retorno";
                        Servicos.Log.TratarErro("get-token: não teve retorno", "IntegracaoRiachuelo");
                        return string.Empty;
                    }
                }
                else
                {
                    mensagemErro = "get-token: " + result.StatusCode.ToString();
                    Servicos.Log.TratarErro("get-token: " + mensagemErro, "IntegracaoRiachuelo");
                    return string.Empty;
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("ObterToken: " + excecao, "IntegracaoRiachuelo");
                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoRiachuelo");

                mensagemErro = "Ocorreu uma falha ao comunicar com o Serviço da Riachuelo.";

                return string.Empty;
            }
        }

        private static void SalvarArquivoIntegracaoOcorrencia(Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracao integracao, string jsonRequisicao, string jsonResposta, string mensagemErro, Repositorio.UnitOfWork unitOfWork)
        {
            if (string.IsNullOrWhiteSpace(jsonRequisicao) && string.IsNullOrWhiteSpace(jsonResposta))
                return;

            Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo repositorioOcorrenciaCTeIntegracaoArquivo = new Repositorio.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo(unitOfWork);
            Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo arquivoIntegracao = new Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo()
            {
                Data = integracao.DataIntegracao,
                Mensagem = mensagemErro,
                Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento,
                ArquivoRequisicao = ArquivoIntegracao.SalvarArquivoIntegracao(jsonRequisicao, "json", unitOfWork),
                ArquivoResposta = ArquivoIntegracao.SalvarArquivoIntegracao(jsonResposta, "json", unitOfWork)
            };

            repositorioOcorrenciaCTeIntegracaoArquivo.Inserir(arquivoIntegracao);

            if (integracao.ArquivosTransacao == null)
                integracao.ArquivosTransacao = new List<Dominio.Entidades.Embarcador.Ocorrencias.OcorrenciaCTeIntegracaoArquivo>();

            integracao.ArquivosTransacao.Add(arquivoIntegracao);
        }

        #endregion
    }
}
