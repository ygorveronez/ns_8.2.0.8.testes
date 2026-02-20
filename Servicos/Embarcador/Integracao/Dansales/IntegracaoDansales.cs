using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Dansales
{
    public class IntegracaoDansales
    {

        #region Métodos Públicos
        public static Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta IntegrarNFes(List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais, Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao, Repositorio.UnitOfWork unitOfWork)
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
                Repositorio.Embarcador.Configuracoes.IntegracaoDansales repositorioConfiguracaoIntegracaoDansales = new Repositorio.Embarcador.Configuracoes.IntegracaoDansales(unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoDansales configuracaoIntegracaoDansales = repositorioConfiguracaoIntegracaoDansales.Buscar();

                if ((configuracaoIntegracaoDansales == null) || string.IsNullOrWhiteSpace(configuracaoIntegracaoDansales.URLIntegracao))
                {
                    httpRequisicaoResposta.mensagem = "Não existe configuração de integração disponível para a DANSALES.";
                }
                else
                {
                    string user = configuracaoIntegracaoDansales.Usuario;
                    string senha = configuracaoIntegracaoDansales.Senha;
                    string urlToken = configuracaoIntegracaoDansales.URLToken;
                    string endPoint = configuracaoIntegracaoDansales.URLIntegracao;

                    string userToken = configuracaoIntegracaoDansales.UsuarioToken;
                    string senhaToken = configuracaoIntegracaoDansales.SenhaToken;

                    string tokenID = ObterToken(urlToken, user, senha, userToken, senhaToken);

                    if (string.IsNullOrWhiteSpace(tokenID)) throw new ServicoException("DANSALES não retornou Token.");

                    string jsonRequest = string.Empty, jsonResponse = string.Empty;

                    try
                    {
                        HttpClient client = ObterClienteRequisicao(endPoint, tokenID, userToken, senhaToken);
                        jsonRequest = RetornarObjetoNotas(notasFiscais, integracao);

                        // Request
                        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                        var result = client.PostAsync(endPoint, content).Result;

                        // Response
                        jsonResponse = result.Content.ReadAsStringAsync().Result;

                        httpRequisicaoResposta.conteudoRequisicao = jsonRequest;
                        httpRequisicaoResposta.conteudoResposta = jsonResponse;
                        httpRequisicaoResposta.httpStatusCode = result.StatusCode;

                        if (result.IsSuccessStatusCode)
                        {
                            string retorno = result.Content.ReadAsStringAsync().Result;
                            if (!string.IsNullOrWhiteSpace(retorno))
                            {
                                dynamic retornoJSON = JsonConvert.DeserializeObject<dynamic>(retorno);
                                if (retornoJSON == null)
                                {
                                    httpRequisicaoResposta.mensagem = "Integração DANSALES não retornou JSON.";
                                }
                                else
                                {
                                    httpRequisicaoResposta.mensagem = "Integrado com sucesso.";
                                    httpRequisicaoResposta.sucesso = true;
                                }
                            }
                            else
                            {
                                httpRequisicaoResposta.mensagem = "Integração Dansales não teve retorno.";
                            }
                        }
                        else
                        {
                            httpRequisicaoResposta.mensagem = "Retorno integração Dansales: " + result.StatusCode.ToString();
                        }

                        if (!httpRequisicaoResposta.sucesso && string.IsNullOrWhiteSpace(httpRequisicaoResposta.mensagem))
                            httpRequisicaoResposta.mensagem = "Integração Dansales não retornou sucesso.";
                    }
                    catch (Exception excecao)
                    {
                        Log.TratarErro("Request: " + jsonRequest, "IntegracaoDansales");
                        Log.TratarErro("Response: " + jsonResponse, "IntegracaoDansales");
                        Log.TratarErro(excecao, "IntegracaoDansales");

                        httpRequisicaoResposta.mensagem = "Ocorreu uma falha ao comunicar com o Serviço da Dansales.";
                    }
                }
            }

            return httpRequisicaoResposta;
        }
        #endregion

        public static string ObterToken(string urlToken, string user, string senha, string userToken, string senhaToken)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string endPoint = urlToken;
            string mensagemErro = string.Empty;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoDansales));

            byte[] textoAsBytes = Encoding.ASCII.GetBytes(userToken + ":" + senhaToken);
            string senha64 = System.Convert.ToBase64String(textoAsBytes);

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", senha64);

            try
            {
                string jsonResponse = string.Empty;
                Dominio.ObjetosDeValor.Embarcador.Integracao.Dansales.RequestToken requestJson = new Dominio.ObjetosDeValor.Embarcador.Integracao.Dansales.RequestToken();

                requestJson.password = senha;
                requestJson.user = user;
                string jsonRequest = JsonConvert.SerializeObject(requestJson, Formatting.Indented);

                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                var result = client.PostAsync(endPoint, content).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                if (result.IsSuccessStatusCode)
                {
                    Servicos.Log.TratarErro("get-token URL: " + endPoint, "IntegracaoDansales");
                    Servicos.Log.TratarErro("get-token Request: " + jsonRequest, "IntegracaoDansales");
                    Servicos.Log.TratarErro("get-token Response: " + jsonResponse, "IntegracaoDansales");

                    var retorno = (string)(result.Content.ReadAsStringAsync().Result);
                    if (!string.IsNullOrWhiteSpace(retorno))
                    {
                        dynamic objetoRetorno = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(retorno);

                        string tokenRetorno = (string)objetoRetorno.token;
                        if (!string.IsNullOrWhiteSpace(tokenRetorno))
                        {
                            return tokenRetorno;
                        }
                        else
                        {
                            Servicos.Log.TratarErro("get-token: não teve retorno", "IntegracaoDansales");
                            return string.Empty;

                        }
                    }
                    else
                    {
                        Servicos.Log.TratarErro("get-token: não teve retorno", "IntegracaoDansales");
                        return string.Empty;
                    }
                }
                else
                {
                    mensagemErro = "get-token: " + result.StatusCode.ToString();
                    Servicos.Log.TratarErro("get-token: " + mensagemErro, "IntegracaoDansales");
                    return string.Empty;
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("ObterToken: " + excecao, "IntegracaoDansales");
                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoDansales");

                return string.Empty;
            }
        }

        public static HttpClient ObterClienteRequisicao(string url, string token, string userToken, string senhaToken)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            byte[] textoAsBytes = Encoding.ASCII.GetBytes(userToken + ":" + senhaToken);
            string senha64 = System.Convert.ToBase64String(textoAsBytes);

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoDansales));

            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", senha64);

            if (!string.IsNullOrWhiteSpace(token))
                client.DefaultRequestHeaders.Add("Token", token);

            return client;
        }

        #region Métodos Privados

        private static string RetornarObjetoNotas(List<Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal> notasFiscais, Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntregaIntegracao integracao)
        {
            string jsonRequest = string.Empty;
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Dansales.StatusNFe> lista = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Dansales.StatusNFe>();

            foreach (Dominio.Entidades.Embarcador.Pedidos.XMLNotaFiscal notaFiscal in notasFiscais)
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Dansales.StatusNFe statusNFe = new Dominio.ObjetosDeValor.Embarcador.Integracao.Dansales.StatusNFe();
                statusNFe.StatusId = (int)notaFiscal.SituacaoEntregaNotaFiscal == 0 ? 6 : (int)notaFiscal.SituacaoEntregaNotaFiscal;
                statusNFe.StatusDescription = (int)notaFiscal.SituacaoEntregaNotaFiscal == 0 ? "Aguardando Entrega" : notaFiscal.SituacaoEntregaNotaFiscal.ObterDescricao();
                statusNFe.NumeroCarga = integracao.PedidoOcorrenciaColetaEntrega.Pedido.CodigoCargaEmbarcador;
                statusNFe.Message = integracao.PedidoOcorrenciaColetaEntrega.TipoDeOcorrencia?.Descricao ?? "";
                statusNFe.EventDate = integracao.PedidoOcorrenciaColetaEntrega.DataOcorrencia.ToString("yyyy-MM-ddThh:mm:ss.fffZ");
                statusNFe.NumNF = notaFiscal.Numero.ToString();
                statusNFe.CnpjEmitente = notaFiscal.Emitente.CPF_CNPJ_SemFormato;
                statusNFe.Serie = Utilidades.Chave.ObterSerie(notaFiscal.Chave);

                lista.Add(statusNFe);
            }

            jsonRequest = JsonConvert.SerializeObject(lista, Formatting.Indented);

            return jsonRequest;
        }

        #endregion
    }
}
