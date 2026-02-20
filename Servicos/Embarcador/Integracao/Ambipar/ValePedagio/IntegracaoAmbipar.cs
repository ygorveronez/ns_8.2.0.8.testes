using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Ambipar
{
    public partial class ValePedagio
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAmbiparValePedagio _integracaoAmbipar;
        private string urlWebService = null;
        private string token = null;

        #endregion

        #region Construtores

        public ValePedagio(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        #endregion

        #region Métodos Privados
        #endregion

        #region Métodos Privados - Configurações

        private bool ValidarConfiguracaoIntegracao(Dominio.Entidades.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio cargaValePedagio)
        {
            if (_integracaoAmbipar == null)
            {
                Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio repositorioCargaValePedagio = new Repositorio.Embarcador.Cargas.ValePedagio.CargaIntegracaoValePedagio(_unitOfWork);

                cargaValePedagio.ProblemaIntegracao = "Não possui configuração para Ambipar.";
                cargaValePedagio.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                cargaValePedagio.DataIntegracao = DateTime.Now;
                cargaValePedagio.NumeroTentativas++;
                repositorioCargaValePedagio.Atualizar(cargaValePedagio);

                return false;
            }
            else
            {
                if (!string.IsNullOrEmpty(this._integracaoAmbipar.URL))
                {
                    if (this._integracaoAmbipar.URL.EndsWith("/"))
                        this.urlWebService = this._integracaoAmbipar.URL;
                    else
                        this.urlWebService = this._integracaoAmbipar.URL + "/";
                }

                return true;
            }
        }

        private void ObterToken(out string mensagemErro)
        {
            mensagemErro = "";
            string jsonRequisicao = "";
            string jsonRetorno = "";

            try
            {
                if (!string.IsNullOrEmpty(this.token))
                    return;

                if (string.IsNullOrWhiteSpace(this.urlWebService))
                    throw new ServicoException("CIOTAmbipar não possui URL para integração.");

                Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.gerarTokenParam gerarTokenParam = ObterParametrosGetToken(this._integracaoAmbipar);
                string url = $"{this.urlWebService}mso-cargo-gestaousuario/api/Token";

                HttpClient requisicao = CriarRequisicao(url);
                jsonRequisicao = JsonConvert.SerializeObject(gerarTokenParam, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode != HttpStatusCode.OK)
                    throw new ServicoException($"Falha ao requisitar o token Ambipar: {retornoRequisicao.StatusCode}");

                Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.RetornoGerarTokenParam retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.RetornoGerarTokenParam>(jsonRetorno);
                if (!string.IsNullOrEmpty(retorno.access_token))
                    this.token = retorno.access_token;
            }
            catch (ServicoException ex)
            {
                mensagemErro = ex.Message;
                this.token = null;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);
                mensagemErro = "Ocorreu uma falha ao realizar a autenticação de viagem na Ambipar.";
                this.token = null;
            }
        }

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.gerarTokenParam ObterParametrosGetToken(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoAmbiparValePedagio config)
        {
            return new Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.gerarTokenParam()
            {
                login = config.Usuario,
                senha = config.Senha,
                perfil = "3"
            };

        }

        private HttpClient CriarRequisicao(string url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(ValePedagio));
            requisicao.DefaultRequestHeaders.Add("User-Agent", "NomeDoNavegador");
            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (!string.IsNullOrWhiteSpace(this.token))
                requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.token);
            return requisicao;
        }

        #endregion
    }
}