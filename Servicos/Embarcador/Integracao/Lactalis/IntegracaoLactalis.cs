using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Lactalis;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Lactalis
{
    public class IntegracaoLactalis
    {
        #region Atributos Privados

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLactalis _configuracaoIntegracaoLactalis;

        #endregion

        #region Construtores

        public IntegracaoLactalis(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaDadosTransporteIntegracao cargaDadosTransporteIntegracao)
        {
            HttpRequisicaoResposta respostaHttp = new HttpRequisicaoResposta();
            Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao repositorioCargaDadosTransporteIntegracao = new Repositorio.Embarcador.Cargas.CargaDadosTransporteIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            cargaDadosTransporteIntegracao.NumeroTentativas++;
            cargaDadosTransporteIntegracao.DataIntegracao = DateTime.Now;

            try
            {
                Dominio.ObjetosDeValor.Embarcador.Integracao.Lactalis.Carga requisicao = ObterDadosCarga(cargaDadosTransporteIntegracao.Carga);

                respostaHttp = ExecutarRequisicao(requisicao, "/preshipment");

                if (respostaHttp.httpStatusCode == HttpStatusCode.InternalServerError)
                    throw new ServicoException("Ocorreu um erro interno no servidor requisitado da Lactalis.");

                RetornoIntegracao retorno = JsonConvert.DeserializeObject<RetornoIntegracao>(respostaHttp.conteudoResposta);

                if (!respostaHttp.sucesso || respostaHttp.httpStatusCode != HttpStatusCode.OK)
                    throw new ServicoException(retorno.Mensagem ?? "Ocorreu um erro ao realizar a integração com a Lactalis");

                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Integração realizada com sucesso.";
            }
            catch (ServicoException exception)
            {
                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = exception.Message;
            }
            catch (Exception exception)
            {
                Log.TratarErro(exception, "IntegracaoLactalis");

                cargaDadosTransporteIntegracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                cargaDadosTransporteIntegracao.ProblemaIntegracao = "Ocorreu um erro ao realizar a integração com a Lactalis.";
            }

            servicoArquivoTransacao.Adicionar(cargaDadosTransporteIntegracao, respostaHttp.conteudoRequisicao, respostaHttp.conteudoResposta, "json");
            repositorioCargaDadosTransporteIntegracao.Atualizar(cargaDadosTransporteIntegracao);
        }

        private void ObterConfiguracaoIntegracaoLactalis()
        {
            if (_configuracaoIntegracaoLactalis != null)
                return;

            Repositorio.Embarcador.Configuracoes.IntegracaoLactalis repositorioIntegracaoLactalis = new Repositorio.Embarcador.Configuracoes.IntegracaoLactalis(_unitOfWork);
            _configuracaoIntegracaoLactalis = repositorioIntegracaoLactalis.BuscarPrimeiroRegistro();

            if (_configuracaoIntegracaoLactalis == null || !_configuracaoIntegracaoLactalis.PossuiIntegracao)
                throw new ServicoException("Não existe configuração de integração disponível para a Lactalis.");

            if (string.IsNullOrWhiteSpace(_configuracaoIntegracaoLactalis.URLAutenticacao) || string.IsNullOrWhiteSpace(_configuracaoIntegracaoLactalis.URLIntegracao))
                throw new ServicoException("URL Autenticação e URL Integração devem estar preenchidos na configuração de integração da Lactalis.");
        }

        private string ObterToken()
        {
            ObterConfiguracaoIntegracaoLactalis();

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoLactalis));

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            FormUrlEncodedContent content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", _configuracaoIntegracaoLactalis.Usuario},
                { "client_secret", _configuracaoIntegracaoLactalis.Senha },
                { "grant_type", "client_credentials" }
            });

            HttpResponseMessage result = client.PostAsync(_configuracaoIntegracaoLactalis.URLAutenticacao, content).Result;

            if (!result.IsSuccessStatusCode)
                throw new ServicoException("Não foi possível obter o Token");

            string jsonResponse = result.Content.ReadAsStringAsync().Result;

            RetornoToken retorno = JsonConvert.DeserializeObject<RetornoToken>(jsonResponse);

            return retorno.Token;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Lactalis.Carga ObterDadosCarga(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Repositorio.Embarcador.Cargas.CargaPedido repositorioCargaPedido = new Repositorio.Embarcador.Cargas.CargaPedido(_unitOfWork);
            List<int> protocolosIntegracaoPedido = repositorioCargaPedido.BuscarProtocoloPedidoPorCarga(carga.Codigo);


            if (protocolosIntegracaoPedido.Count == 0)
                throw new ServicoException("Não existem pedidos integrados para a carga.");

            List<Dominio.ObjetosDeValor.Embarcador.Integracao.Lactalis.Pedido> pedidos = new List<Dominio.ObjetosDeValor.Embarcador.Integracao.Lactalis.Pedido>();

            foreach (int protocolo in protocolosIntegracaoPedido)
            {
                pedidos.Add(new Dominio.ObjetosDeValor.Embarcador.Integracao.Lactalis.Pedido
                {
                    ProtocoloIntegracaoPedido = protocolo.ToString()
                });
            }

            return new Dominio.ObjetosDeValor.Embarcador.Integracao.Lactalis.Carga
            {
                CodigoCargaEmbarcador = carga.CodigoCargaEmbarcador,
                CodigoIntegracaoCarga = carga.Protocolo,
                TipoOperacao = carga.TipoOperacao?.Descricao ?? string.Empty,
                Pedidos = pedidos
            };
        }

        private HttpRequisicaoResposta ExecutarRequisicao<T>(T dadosRequisicao, string urlIntegracao)
        {
            string token = ObterToken();
            HttpClient client = CriarRequisicao(token);

            string jsonRequest = JsonConvert.SerializeObject(dadosRequisicao, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            StringContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            HttpResponseMessage result = client.PostAsync(_configuracaoIntegracaoLactalis.URLIntegracao + urlIntegracao, content).Result;
            HttpRequisicaoResposta httpRequisicaoResposta = ObterHttRequisicaoResposta(jsonRequest, result);

            return httpRequisicaoResposta;
        }

        private HttpRequisicaoResposta ObterHttRequisicaoResposta(string jsonRequest, HttpResponseMessage result)
        {
            HttpRequisicaoResposta httpRequisicaoResposta = new HttpRequisicaoResposta()
            {
                conteudoRequisicao = jsonRequest,
                extensaoRequisicao = "json",
                conteudoResposta = result.Content.ReadAsStringAsync().Result,
                extensaoResposta = "json",
                sucesso = result.IsSuccessStatusCode,
                mensagem = string.Empty,
                httpStatusCode = result.StatusCode
            };

            return httpRequisicaoResposta;
        }

        private HttpClient CriarRequisicao(string accessToken = null)
        {
            ObterConfiguracaoIntegracaoLactalis();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoLactalis));

            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrWhiteSpace(accessToken))
                requisicao.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

            return requisicao;
        }
    }
}