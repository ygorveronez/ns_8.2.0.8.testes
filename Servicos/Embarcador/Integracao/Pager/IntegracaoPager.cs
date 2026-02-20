using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;


namespace Servicos.Embarcador.Integracao.Pager
{
    public class IntegracaoPager
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos Globais

        #region Construtores

        public IntegracaoPager(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public bool ValidarEtapaIntegracao(EtapaFluxoGestaoPatio etapa, Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio)
        {
            if (etapa != EtapaFluxoGestaoPatio.SolicitacaoVeiculo)
                return false;

            GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa servicoFluxoGestaoPatioConfiguracaoEtapa = new GestaoPatio.FluxoGestaoPatioConfiguracaoEtapa(_unitOfWork);
            Dominio.Entidades.Embarcador.Filiais.SequenciaGestaoPatio sequenciaGestaoPatio = servicoFluxoGestaoPatioConfiguracaoEtapa.ObterSequenciaGestaoPatio(fluxoGestaoPatio);

            return sequenciaGestaoPatio?.SolicitacaoVeiculoHabilitarIntegracaoPager ?? false;
        }

        public void Integrar(Dominio.Entidades.Embarcador.GestaoPatio.FluxoPatioIntegracao fluxoPatioIntegrao)
        {
            Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao repositorioFluxoPatioIntegracao = new Repositorio.Embarcador.GestaoPatio.FluxoPatioIntegracao(_unitOfWork);
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            string jsonRequest = string.Empty;
            string jsonResponse = string.Empty;

            try
            {
                fluxoPatioIntegrao.NumeroTentativas++;
                fluxoPatioIntegrao.DataIntegracao = DateTime.Now;

                jsonRequest = ObterRequestIntegracao(fluxoPatioIntegrao.Carga);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPager configuracaoPager = ObterConfiguracaoPager();
                string token = ObterToken(configuracaoPager);
                HttpClient cliente = CriarRequisicao(token);
                StringContent content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                HttpResponseMessage result = cliente.PostAsync(configuracaoPager.URLIntegracao + "/AOMS145/ChamadoDePager", content).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                if (!result.IsSuccessStatusCode)
                {
                    dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                    string message = $"Falha ao integrar - Status ({retorno.status}) - {retorno.message}";

                    fluxoPatioIntegrao.ProblemaIntegracao = message;
                    fluxoPatioIntegrao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Pager.RetornoPager retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Pager.RetornoPager>(jsonResponse);

                    if (retorno.Status != 200)
                    {
                        string message = $"Falha ao integrar - Status ({retorno.Status}) - {retorno.Mensagem}";

                        fluxoPatioIntegrao.ProblemaIntegracao = message;
                        fluxoPatioIntegrao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                    }
                    else
                    {
                        fluxoPatioIntegrao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                        fluxoPatioIntegrao.ProblemaIntegracao = "Integração realizada com Sucesso";
                    }
                }

                servicoArquivoTransacao.Adicionar(fluxoPatioIntegrao, jsonRequest, jsonResponse, "json");
            }
            catch (ServicoException ex)
            {
                fluxoPatioIntegrao.ProblemaIntegracao = ex.Message;
                fluxoPatioIntegrao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro(ex);

                fluxoPatioIntegrao.ProblemaIntegracao = "Problema ao tentar integrar com Pager";
                fluxoPatioIntegrao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                servicoArquivoTransacao.Adicionar(fluxoPatioIntegrao, jsonRequest, jsonResponse, "json");
            }

            repositorioFluxoPatioIntegracao.Atualizar(fluxoPatioIntegrao);
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private string ObterToken(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPager configuracaoPager)
        {
            HttpClient requisicao = CriarRequisicao();

            requisicao.DefaultRequestHeaders.Add("grant_type", "password");
            requisicao.DefaultRequestHeaders.Add("username", configuracaoPager.Usuario);
            requisicao.DefaultRequestHeaders.Add("password", configuracaoPager.Senha);

            HttpResponseMessage retornoRequisicao = requisicao.PostAsync($"{configuracaoPager.URLIntegracao}/api/oauth2/v1/token", null).Result;
            string jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

            if (retornoRequisicao.StatusCode != HttpStatusCode.Created)
                throw new ServicoException("Não foi possível obter o Token");

            var obj = JsonConvert.DeserializeObject<dynamic>(jsonRetorno);
            return obj?.access_token;
        }

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPager ObterConfiguracaoPager()
        {
            Repositorio.Embarcador.Configuracoes.IntegracaoPager repositorioIntegracaoPager = new Repositorio.Embarcador.Configuracoes.IntegracaoPager(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoPager configuracaoIntegracaoPager = repositorioIntegracaoPager.BuscarPrimeiroRegistro();

            if (configuracaoIntegracaoPager == null || !configuracaoIntegracaoPager.PossuiIntegracao)
                throw new ServicoException("Não existe configuração de integração disponível para Pager.");

            return configuracaoIntegracaoPager;
        }

        private string ObterRequestIntegracao(Dominio.Entidades.Embarcador.Cargas.Carga carga)
        {
            Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio servicoFluxoGestaoPatio = new Servicos.Embarcador.GestaoPatio.FluxoGestaoPatio(_unitOfWork);

            Dominio.Entidades.Embarcador.GestaoPatio.FluxoGestaoPatio fluxoGestaoPatio = servicoFluxoGestaoPatio.ObterFluxoGestaoPatio(carga);

            string numeroPager = fluxoGestaoPatio.Equipamento?.Numero ?? string.Empty;

            if (string.IsNullOrWhiteSpace(numeroPager))
                throw new ServicoException("Equipamento não informado ou não possui número de Pager cadastrado.");

            Dominio.ObjetosDeValor.Embarcador.Integracao.Pager.RequisicaoPager corpoRequisicaoPager = new Dominio.ObjetosDeValor.Embarcador.Integracao.Pager.RequisicaoPager()
            {
                CodigoIntegracao = carga.Filial?.CodigoFilialEmbarcador,
                NumeroPager = numeroPager,
            };

            return JsonConvert.SerializeObject(corpoRequisicaoPager, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
        }

        private HttpClient CriarRequisicao(string accessToken = null)
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)12288;

            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoPager));

            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requisicao.Timeout = TimeSpan.FromMinutes(3);

            if (!string.IsNullOrWhiteSpace(accessToken))
                requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return requisicao;
        }

        #endregion Métodos Privados
    }
}