using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Magalu
{
    public class IntegracaoMagalu
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public IntegracaoMagalu(Repositorio.UnitOfWork unitOfWork)
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

            Repositorio.Embarcador.Configuracoes.IntegracaoMagalu repositorioIntegracaoMagalu = new Repositorio.Embarcador.Configuracoes.IntegracaoMagalu(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoMagalu configuracaoIntegracaoMagalu = repositorioIntegracaoMagalu.Buscar();

            if (string.IsNullOrWhiteSpace(configuracaoIntegracaoMagalu?.URL) || string.IsNullOrWhiteSpace(configuracaoIntegracaoMagalu?.Token))
            {
                httpRequisicaoResposta.mensagem = "A integração com a Magalu não está configurada.";
                return httpRequisicaoResposta;
            }

            try
            {
                if (string.IsNullOrWhiteSpace(pedidoOcorrenciaColetaEntrega.Pedido.NumeroPedidoEmbarcador))
                    throw new ServicoException("Número do pedido não informado nele, necessário para a integração.");

                string url = $"{configuracaoIntegracaoMagalu.URL}?package.id={pedidoOcorrenciaColetaEntrega.Pedido.NumeroPedidoEmbarcador}";

                HttpClient requisicao = CriarRequisicao(url, configuracaoIntegracaoMagalu.Token);

                Dominio.ObjetosDeValor.Embarcador.Integracao.Magalu.Evento evento = ObterDadosEvento(pedidoOcorrenciaColetaEntrega);
                string jsonRequisicao = JsonConvert.SerializeObject(evento, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                string jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                httpRequisicaoResposta.conteudoRequisicao = jsonRequisicao;
                httpRequisicaoResposta.conteudoResposta = jsonRetorno;

                if (retornoRequisicao.StatusCode == HttpStatusCode.Accepted)
                {
                    httpRequisicaoResposta.mensagem = "Integrado com sucesso.";
                    httpRequisicaoResposta.sucesso = true;
                }
                else if (string.IsNullOrWhiteSpace(jsonRetorno))
                    httpRequisicaoResposta.mensagem = "Retorno integração: " + retornoRequisicao.StatusCode;
                else
                {
                    jsonRetorno = jsonRetorno.Replace("[", "").Replace("]", "");
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Magalu.EventoRetornoErro retornoErro = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Magalu.EventoRetornoErro>(jsonRetorno);
                    httpRequisicaoResposta.mensagem = retornoErro.CodigoErro + " - " + retornoErro.Mensagem;
                }
            }
            catch (ServicoException excecao)
            {
                httpRequisicaoResposta.mensagem = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                httpRequisicaoResposta.mensagem = "Ocorreu uma falha ao realizar a integração com a Magalu.";
            }

            return httpRequisicaoResposta;
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Magalu.Evento ObterDadosEvento(Dominio.Entidades.Embarcador.Pedidos.PedidoOcorrenciaColetaEntrega pedidoOcorrenciaColetaEntrega)
        {
            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Magalu.Evento()
            {
                Data = pedidoOcorrenciaColetaEntrega.DataOcorrencia.ToString("yyyy-MM-ddTHH:mm:ss.fff-03:00"),
                Status = new Dominio.ObjetosDeValor.Embarcador.Integracao.Magalu.EventoCodigo
                {
                    Codigo = pedidoOcorrenciaColetaEntrega.TipoDeOcorrencia.CodigoIntegracao
                }
            };
        }

        private HttpClient CriarRequisicao(string url, string token)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoMagalu));

            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return requisicao;
        }

        #endregion
    }
}
