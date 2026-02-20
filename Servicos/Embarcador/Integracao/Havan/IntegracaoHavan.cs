using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Havan
{
    public class IntegracaoHavan
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public IntegracaoHavan(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public HttpRequisicaoResposta EnviarOcorrencia(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega pedidoOcorrenciaColetaEntrega)
        {
            HttpRequisicaoResposta httpRequisicaoResposta = new HttpRequisicaoResposta()
            {
                conteudoRequisicao = string.Empty,
                extensaoRequisicao = "json",
                conteudoResposta = string.Empty,
                extensaoResposta = "json",
                sucesso = false,
                mensagem = string.Empty
            };

            Repositorio.Embarcador.Configuracoes.IntegracaoHavan repositorioIntegracaoHavan = new Repositorio.Embarcador.Configuracoes.IntegracaoHavan(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoHavan configuracaoIntegracaoHavan = repositorioIntegracaoHavan.Buscar();

            try
            {
                if (string.IsNullOrWhiteSpace(configuracaoIntegracaoHavan?.URLAutenticacao) || string.IsNullOrWhiteSpace(configuracaoIntegracaoHavan?.URLEnvioOcorrencia))
                    throw new ServicoException("A integração com a Havan não está configurada.");

                string authToken = ObterToken(configuracaoIntegracaoHavan);

                string url = configuracaoIntegracaoHavan.URLEnvioOcorrencia;
                HttpClient requisicao = CriarRequisicao(url, authToken);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Havan.EnvioOcorrencia evento = ObterDadosEnvioOcorrencia(pedidoOcorrenciaColetaEntrega);
                string jsonRequisicao = JsonConvert.SerializeObject(evento, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                string jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                httpRequisicaoResposta.conteudoRequisicao = jsonRequisicao;
                httpRequisicaoResposta.conteudoResposta = jsonRetorno;

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                    httpRequisicaoResposta.sucesso = true;

                dynamic retorno = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonRetorno);
                httpRequisicaoResposta.mensagem = (string)retorno;
            }
            catch (ServicoException excecao)
            {
                httpRequisicaoResposta.mensagem = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                httpRequisicaoResposta.mensagem = "Ocorreu uma falha ao realizar a integração com a Havan.";
            }

            return httpRequisicaoResposta;
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Havan.EnvioOcorrencia ObterDadosEnvioOcorrencia(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega pedidoOcorrenciaColetaEntrega)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Havan.EnvioOcorrencia()
            {
                Data = pedidoOcorrenciaColetaEntrega.DataOcorrencia.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                ProtocoloPedido = pedidoOcorrenciaColetaEntrega.Pedido.Protocolo,
                CodigoIntegracao = pedidoOcorrenciaColetaEntrega.TipoDeOcorrencia.CodigoIntegracao,
                Descricao = pedidoOcorrenciaColetaEntrega.TipoDeOcorrencia.Descricao
            };
        }

        private HttpClient CriarRequisicao(string url, string accessToken = null)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoHavan));

            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrWhiteSpace(accessToken))
                requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return requisicao;
        }

        private string ObterToken(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoHavan integracaoHavan)
        {
            if (string.IsNullOrWhiteSpace(integracaoHavan.URLAutenticacao))
                throw new ServicoException("URL de autenticação não configurada.");

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoHavan));

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            FormUrlEncodedContent content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "username", integracaoHavan.Usuario },
                { "password", integracaoHavan.Senha },
                { "grant_type", "password" }
            });

            HttpResponseMessage result = client.PostAsync(integracaoHavan.URLAutenticacao, content).Result;
            string jsonResponse = result.Content.ReadAsStringAsync().Result;

            if (!result.IsSuccessStatusCode)
                throw new ServicoException("Não retornou o token de integração.");

            dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);

            return (string)obj.access_token;
        }

        #endregion
    }
}
