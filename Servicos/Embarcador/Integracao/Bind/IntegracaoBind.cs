using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Servicos.Embarcador.Integracao.Bind
{
    public class IntegracaoBind
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos Globais

        #region Construtores

        public IntegracaoBind(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void Integrar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao fluxoPatioIntegracao)
        {
            Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao repositorioFluxoPatio = new Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {
                fluxoPatioIntegracao.NumeroTentativas++;
                fluxoPatioIntegracao.DataIntegracao = DateTime.Now;

                if (string.IsNullOrWhiteSpace(fluxoPatioIntegracao.Carga.NumeroDoca))
                    throw new ServicoException("Número da Doca não informado");

                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBind configIntegracaoBind = ObterConfiguracaBind();
                Dominio.ObjetosDeValor.Embarcador.Integracao.Bind.RequisicaoBind dadosRequisicao = ObterObjetoRequest(fluxoPatioIntegracao);

                string format = "yyyy-MM-dd'T'HH:mm:ss.fffzzz";
                jsonRequest = JsonConvert.SerializeObject(dadosRequisicao, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, DateFormatString = format });

                MultipartFormDataContent content = new MultipartFormDataContent
                {
                    { new StringContent(dadosRequisicao.NumeroCarga), "id_carga" },
                    { new StringContent(dadosRequisicao.Etapa), "etapa" },
                    { new StringContent(((int)dadosRequisicao.Prioridade).ToString()), "prioridade" }
                };

                if (dadosRequisicao.Inicio.HasValue)
                    content.Add(new StringContent(new DateTimeOffset(dadosRequisicao.Inicio.Value).ToString(format)), "inicio");
                if (dadosRequisicao.Fim.HasValue)
                    content.Add(new StringContent(new DateTimeOffset(dadosRequisicao.Fim.Value).ToString(format)), "fim");

                string requestUrl = configIntegracaoBind.URLIntegracao + $"/{fluxoPatioIntegracao.Carga.NumeroDoca}";
                HttpClient cliente = ObterHttpClient(configIntegracaoBind);
                HttpResponseMessage result = cliente.PostAsync(requestUrl, content).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                if (!result.IsSuccessStatusCode)
                {
                    dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                    string message = $"Falha ao integrar - Status ({retorno.status}) - {retorno?.fault?.faultstring ?? retorno?.message}";
                    fluxoPatioIntegracao.ProblemaIntegracao = message;
                    fluxoPatioIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }
                else
                {
                    fluxoPatioIntegracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    fluxoPatioIntegracao.ProblemaIntegracao = "Integração realizada com Sucesso";
                }

                servicoArquivoTransacao.Adicionar(fluxoPatioIntegracao, jsonRequest, jsonResponse, "json");
            }
            catch (ServicoException ex)
            {
                fluxoPatioIntegracao.ProblemaIntegracao = ex.Message;
                fluxoPatioIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                fluxoPatioIntegracao.ProblemaIntegracao = "Problema ao tentar integrar com Bind";
                fluxoPatioIntegracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;

                servicoArquivoTransacao.Adicionar(fluxoPatioIntegracao, jsonRequest, jsonResponse, "json");
            }

            repositorioFluxoPatio.Atualizar(fluxoPatioIntegracao);
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private HttpClient ObterHttpClient(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBind configuracaoIntegracao)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoBind));

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", configuracaoIntegracao.APIKeyIntegracao);

            return client;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBind ObterConfiguracaBind()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoBind repositorioIntegracaoBind = new Repositorio.Embarcador.Configuracoes.IntegracaoBind(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoBind configuracaoIntegracaoBind = repositorioIntegracaoBind.BuscarPrimeiroRegistro();

            return configuracaoIntegracaoBind;
        }

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Bind.RequisicaoBind ObterObjetoRequest(Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao fluxoPatioIntegracao)
        {

            Dominio.ObjetosDeValor.Embarcador.Integracao.Bind.RequisicaoBind dadosRequisicao = new Dominio.ObjetosDeValor.Embarcador.Integracao.Bind.RequisicaoBind()
            {
                NumeroCarga = fluxoPatioIntegracao.Carga.CodigoCargaEmbarcador,
                Etapa = fluxoPatioIntegracao.Pedido?.Destinatario?.Descricao,
                Inicio = fluxoPatioIntegracao.Pedido?.DataInicialColeta,
                Fim = new Repositorio.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork).BuscarDataDocaInformadaPorCarga(fluxoPatioIntegracao.Carga.Codigo),
                Veiculo = fluxoPatioIntegracao.Carga.Veiculo != null ? "S" : "N",
                Prioridade = fluxoPatioIntegracao.Pedido?.Destinatario?.GrupoPessoas?.Prioridade ?? PrioridadeGrupoPessoas.Outros
            };

            return dadosRequisicao;
        }

        #endregion Métodos Privados
    }
}