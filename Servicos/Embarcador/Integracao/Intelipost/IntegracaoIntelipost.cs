using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Intelipost
{
    public class IntegracaoIntelipost
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private const string URL = "https://api.intelipost.com.br/api/v1/tracking";
        private const string ADD_EVENTS_URI = "/add/events";

        #endregion

        #region Construtores

        public IntegracaoIntelipost(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public string ObterToken(int codigoEmpresa, int codigoCanalEntrega)
        {
            Repositorio.Global.EmpresaIntelipostIntegracao repEmpresaIntelipostIntegracao = new Repositorio.Global.EmpresaIntelipostIntegracao(_unitOfWork);
            Dominio.Entidades.EmpresaIntelipostIntegracao configuracaoToken;

            configuracaoToken = repEmpresaIntelipostIntegracao.BuscarPorEmpresaeCanalEntrega(codigoEmpresa, codigoCanalEntrega);
            if (configuracaoToken != null) return configuracaoToken.Token;

            configuracaoToken = repEmpresaIntelipostIntegracao.BuscarPorEmpresaSemCanalEntrega(codigoEmpresa);
            if (configuracaoToken != null) return configuracaoToken.Token;

            return ""; //c8f018a0-c7ab-84a4-7867-99544f15dd23
        }

        public Dominio.ObjetosDeValor.Embarcador.Integracao.HttpRequisicaoResposta EnviarEventoRastreamento(string token, Dominio.ObjetosDeValor.Embarcador.Integracao.Intelipost.EventoRastreamento eventoRastreamento)
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

            try
            {
                string url = URL + ADD_EVENTS_URI;

                // Headers
                HttpClient client = CriarRequisicao(url, token);

                // Body
                string jsonRequest = JsonConvert.SerializeObject(eventoRastreamento, Formatting.Indented);

                // Request
                StringContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                // Request
                HttpResponseMessage result = client.PostAsync(url, content).Result;
                string jsonResponse = result.Content.ReadAsStringAsync().Result;

                httpRequisicaoResposta.conteudoRequisicao = jsonRequest;
                httpRequisicaoResposta.conteudoResposta = jsonResponse;
                httpRequisicaoResposta.httpStatusCode = result.StatusCode;

                if (result.StatusCode == HttpStatusCode.OK || result.StatusCode == HttpStatusCode.BadRequest)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Intelipost.ResponseEventoRastreamento response = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Intelipost.ResponseEventoRastreamento>(jsonResponse);

                    httpRequisicaoResposta.sucesso = response.status == "OK";
                    httpRequisicaoResposta.mensagem = response.messages.FirstOrDefault().text;
                }
                else
                    httpRequisicaoResposta.mensagem = "O WS da Intelipost retornou " + ((int)result.StatusCode).ToString() + " na tentativa de conexão.";
            }
            catch (Exception ex)
            {
                Log.TratarErro("Falha Intelipost: " + ex);
                httpRequisicaoResposta.mensagem = "Ocorreu uma falha ao realizar a integração com a Intelipost.";
            }

            return httpRequisicaoResposta;
        }

        #endregion

        #region Métodos Privados

        private HttpClient CriarRequisicao(string url, string token)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoIntelipost));

            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Add("platform", "Multisoftware");
            requisicao.DefaultRequestHeaders.Add("logistic-provider-api-key", token);

            return requisicao;
        }

        #endregion
    }
}
