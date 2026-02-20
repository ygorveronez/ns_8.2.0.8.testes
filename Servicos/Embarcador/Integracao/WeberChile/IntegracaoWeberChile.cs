using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using Dominio.ObjetosDeValor.Embarcador.Integracao.WeberChile;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.WeberChile
{
    public class IntegracaoWeberChile
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoWeberChile _configuracaoIntegracao;

        #endregion

        #region Construtores

        public IntegracaoWeberChile(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Comunicação

        private HttpClient ObterHttpClient()
        {
            CarregarConfiguracaoIntegracao();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoWeberChile));

            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string token = ObterToken();
            requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            requisicao.DefaultRequestHeaders.Add("APIKey", _configuracaoIntegracao.ApiKey);

            return requisicao;
        }

        public string ObterToken()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            RestClient client = new RestClient(_configuracaoIntegracao.URLAutenticacao);
            client.Timeout = -1;
            RestRequest request = new RestRequest(Method.POST);

            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("client_id", _configuracaoIntegracao.ClientID);
            request.AddParameter("client_secret", _configuracaoIntegracao.ClientSecret);
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("scope", "openid");

            IRestResponse response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new ServicoException("Não foi possível obter o Token");

            Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.RetornoToken retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Pedido.ConsultarAndamentoPedido.RetornoToken>(response.Content);

            return retorno.access_token;
        }

        private HttpRequisicaoResposta ExecutarRequisicao<T>(T dadosRequisicao, string endpointAcao)
        {
            HttpClient client = ObterHttpClient();

            string jsonRequest = JsonConvert.SerializeObject(dadosRequisicao, Formatting.Indented);

            StringContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            HttpResponseMessage result = client.PostAsync(string.Concat(_configuracaoIntegracao.URLIntegracao.TrimEnd('/'), endpointAcao), content).Result;
            HttpRequisicaoResposta httpRequisicaoResposta = ObterHttRequisicaoResposta(jsonRequest, result);

            return httpRequisicaoResposta;
        }

        private static HttpRequisicaoResposta ObterHttRequisicaoResposta(string jsonRequest, HttpResponseMessage result)
        {
            Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta httpRequisicaoResposta = new Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta()
            {
                conteudoRequisicao = jsonRequest,
                extensaoRequisicao = "json",
                conteudoResposta = result.Content.ReadAsStringAsync().Result,
                extensaoResposta = "json",
                sucesso = false,
                mensagem = string.Empty,
                httpStatusCode = result.StatusCode
            };

            return httpRequisicaoResposta;
        }

        #endregion

        #region Métodos Públicos       

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Integracao.IntegracaoEnvioProgramado integracaoEnvioProgramado)
        {
            Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado repositorioIntegracaoEnvioProgramado = new Repositorio.Embarcador.Integracao.IntegracaoEnvioProgramado(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            integracaoEnvioProgramado.DataIntegracao = DateTime.Now;
            integracaoEnvioProgramado.NumeroTentativas++;
            HttpRequisicaoResposta respostaHttp = new HttpRequisicaoResposta();

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.WeberChile.Carga carga = ObterCarga(integracaoEnvioProgramado.Carga);

                respostaHttp = ExecutarRequisicao(carga, "/v1/tms_crea_doc_gasto?sap-client=550");

                if (respostaHttp.httpStatusCode == HttpStatusCode.OK)
                {
                    DadosRetornoWeberChile retornoWeberChile = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.WeberChile.DadosRetornoWeberChile>(respostaHttp.conteudoResposta);

                    if (retornoWeberChile.Status == "E")
                        throw new ServicoException($"Resposta API WeberChile: {retornoWeberChile.Descricao}");
                }
                else
                    throw new ServicoException("Problema ao obter a resposta da API WeberChile.");

                integracaoEnvioProgramado.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                integracaoEnvioProgramado.ProblemaIntegracao = "Integrado com sucesso.";
            }
            catch (ServicoException ex)
            {
                integracaoEnvioProgramado.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoEnvioProgramado.ProblemaIntegracao = ex.Message;
            }
            catch (Exception ex)
            {
                Log.TratarErro(ex);
                integracaoEnvioProgramado.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoEnvioProgramado.ProblemaIntegracao = "Problema ao integrar com API WeberChile";
            }

            servicoArquivoTransacao.Adicionar(integracaoEnvioProgramado, respostaHttp.conteudoRequisicao, respostaHttp.conteudoResposta, "json");
            repositorioIntegracaoEnvioProgramado.Atualizar(integracaoEnvioProgramado);
        }


        #endregion

        #region Métodos Privados

        private void CarregarConfiguracaoIntegracao()
        {
            if (_configuracaoIntegracao != null)
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoWeberChile repositorioConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoWeberChile(_unitOfWork);
            _configuracaoIntegracao = repositorioConfiguracaoIntegracao.BuscarPrimeiroRegistro();

            if (string.IsNullOrWhiteSpace(_configuracaoIntegracao.URLAutenticacao) || string.IsNullOrWhiteSpace(_configuracaoIntegracao.URLAutenticacao) || string.IsNullOrEmpty(_configuracaoIntegracao.ApiKey))
                throw new ServicoException("Não existe configuração de integração disponível para a Weber Chile.");

            if (string.IsNullOrWhiteSpace(_configuracaoIntegracao.ClientID) || string.IsNullOrWhiteSpace(_configuracaoIntegracao.ClientSecret))
                throw new ServicoException("Não existe configuração de autenticação para a Weber Chile.");
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.WeberChile.Carga ObterCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Ocorrencias.CargaOcorrencia repositorioCargaOcorrencia = new Repositorio.Embarcador.Ocorrencias.CargaOcorrencia(_unitOfWork);

            List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias = repositorioCargaOcorrencia.BuscarPorCargaESituacao(carga.Codigo, SituacaoOcorrencia.Finalizada);

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.WeberChile.Carga
            {
                NumeroCarga = carga.CodigoCargaEmbarcador,
                NumeroPedidoEmbarcador = string.Join(",", carga.Pedidos.Where(obj => obj.Pedido?.NumeroPedidoEmbarcador != null).Select(obj => obj.Pedido.NumeroPedidoEmbarcador)),
                ValorTotal = Math.Round(carga.ValorFreteAPagar + ocorrencias.Sum(obj => obj.ValorOcorrencia), 2, MidpointRounding.AwayFromZero),
                Ocorrencias = ObterCargaOcorrencias(ocorrencias),
            };

        }

        private List<Dominio.ObjetosDeValor.Embarcador.Integracao.WeberChile.Ocorrencia> ObterCargaOcorrencias(List<Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia> ocorrencias)
        {
            List<Dominio.ObjetosDeValor.Embarcador.Integracao.WeberChile.Ocorrencia> objetoOcorrencias = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.WeberChile.Ocorrencia>();

            foreach (Dominio.Entidades.Embarcador.Ocorrencias.CargaOcorrencia cargaOcorrencia in ocorrencias)
                objetoOcorrencias.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.WeberChile.Ocorrencia
                {
                    Descricao = cargaOcorrencia.Observacao,
                    Valor = cargaOcorrencia.ValorOcorrencia
                });

            return objetoOcorrencias;
        }

        #endregion
    }
}
