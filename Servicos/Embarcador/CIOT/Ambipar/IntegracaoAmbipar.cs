using Dominio.Excecoes.Embarcador;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.CIOT
{
    public partial class Ambipar
    {
        #region Propriedades Privadas

        private Repositorio.UnitOfWork _unitOfWork;
        private Dominio.Entidades.Embarcador.CIOT.CIOTAmbipar configuracaoAmbipar = null;
        private string urlWebService = null;
        private string token = null;

        #endregion

        #region Construtores

        public Ambipar(Repositorio.UnitOfWork unitOfWork, string urlWebService = null, string token = null)
        {
            _unitOfWork = unitOfWork;

            if (string.IsNullOrEmpty(urlWebService))
                this.urlWebService = urlWebService;

            if (string.IsNullOrEmpty(token))
                this.token = token;
        }

        #endregion

        #region Métodos Globais

        #endregion

        #region Métodos Privados

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

                Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.gerarTokenParam gerarTokenParam = ObterParametrosGetToken(this.configuracaoAmbipar);
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

        private Dominio.ObjetosDeValor.Embarcador.CIOT.Ambipar.gerarTokenParam ObterParametrosGetToken(Dominio.Entidades.Embarcador.CIOT.CIOTAmbipar config)
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
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(Ambipar));
            requisicao.DefaultRequestHeaders.Add("User-Agent", "NomeDoNavegador");
            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (!string.IsNullOrWhiteSpace(this.token))
                requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.token);
            return requisicao;
        }

        private void ObterConfiguracaoAmbipar(Dominio.Entidades.Embarcador.CIOT.ConfiguracaoCIOT configuracaoCIOT)
        {
            Repositorio.Embarcador.CIOT.CIOTAmbipar rep = new Repositorio.Embarcador.CIOT.CIOTAmbipar(_unitOfWork);

            if (this.configuracaoAmbipar == null)
            {
                this.configuracaoAmbipar = rep.BuscarPorConfiguracaoCIOT(configuracaoCIOT.Codigo);

                if (!string.IsNullOrEmpty(this.configuracaoAmbipar.URL))
                {
                    if (this.configuracaoAmbipar.URL.EndsWith("/"))
                        this.urlWebService = this.configuracaoAmbipar.URL;
                    else
                        this.urlWebService = this.configuracaoAmbipar.URL + "/";
                }
            }
        }

        private void GravarArquivoIntegracao(Dominio.Entidades.Embarcador.Documentos.CIOT ciot, string request, string response, string extensaoArquivo)
        {
            Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo repCIOTIntegracaoArquivo = new Repositorio.Embarcador.Documentos.CIOTIntegracaoArquivo(_unitOfWork);
            Repositorio.Embarcador.Documentos.CIOT repCIOT = new Repositorio.Embarcador.Documentos.CIOT(_unitOfWork);

            Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo ciotIntegracaoArquivo = new Dominio.Entidades.Embarcador.Documentos.CIOTIntegracaoArquivo();

            if (!string.IsNullOrWhiteSpace(request))
                ciotIntegracaoArquivo.ArquivoRequisicao = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(request, extensaoArquivo, _unitOfWork);

            if (!string.IsNullOrWhiteSpace(response))
                ciotIntegracaoArquivo.ArquivoResposta = Servicos.Embarcador.Integracao.ArquivoIntegracao.SalvarArquivoIntegracao(response, extensaoArquivo, _unitOfWork);

            ciotIntegracaoArquivo.Data = DateTime.Now;
            ciotIntegracaoArquivo.Tipo = Dominio.ObjetosDeValor.Embarcador.Enumeradores.TipoArquivoIntegracaoCTeCarga.EnvioParaProcessamento;
            ciotIntegracaoArquivo.Mensagem = ciot.Mensagem;

            if (ciotIntegracaoArquivo.ArquivoResposta != null && ciotIntegracaoArquivo.ArquivoResposta != null)
            {
                repCIOTIntegracaoArquivo.Inserir(ciotIntegracaoArquivo);
                ciot.ArquivosTransacao.Add(ciotIntegracaoArquivo);
            }
            if (ciot.Codigo > 0)
                repCIOT.Atualizar(ciot);
            else
                repCIOT.Inserir(ciot);
        }

        #endregion
    }
}