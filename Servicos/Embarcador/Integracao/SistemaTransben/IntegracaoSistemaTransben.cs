using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.SistemaTransben;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;


namespace Servicos.Embarcador.Integracao.SistemaTransben
{
    public partial class IntegracaoSistemaTransben
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;
        private readonly Repositorio.Embarcador.Configuracoes.IntegracaoSistemaTransben _configuracaoIntegracaoRepositorio;

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoSistemaTransben _configuracaoIntegracao;
        string tokenAutenticacao = null;

        #endregion Atributos Globais

        #region Construtores
        public IntegracaoSistemaTransben(Repositorio.UnitOfWork unitOfWork)
        {

            _unitOfWork = unitOfWork;
            _configuracaoIntegracaoRepositorio = new Repositorio.Embarcador.Configuracoes.IntegracaoSistemaTransben(unitOfWork);
            _configuracaoIntegracao = _configuracaoIntegracaoRepositorio.Buscar();

        }

        #endregion Construtores

        #region Métodos Privados

        private string ObterToken(string metodo, string uri)
        {
            string jsonRequisicao = null;
            string jsonRetorno = null;

            if (string.IsNullOrWhiteSpace(metodo))
                return null;

            try
            {
                if (_configuracaoIntegracao == null)
                    throw new ServicoException("Processo Abortado! Integração não configurada.");

                if (string.IsNullOrEmpty(uri))
                    throw new ServicoException("Processo Abortado! URL não definida.");

                string url = null;
                if (uri.EndsWith("/"))
                    url = uri;
                else
                    url = uri + "/";
                url += metodo;

                HttpClient requisicao = CriarRequisicao(url, null);
                StringContent conteudoRequisicao = null;
                HttpResponseMessage retornoRequisicao = null;

                Hashtable request = new Hashtable
                {
                    { "usuario", _configuracaoIntegracao.Usuario},
                    { "senha", _configuracaoIntegracao.Senha },
                };

                jsonRequisicao = JsonConvert.SerializeObject(request, Formatting.Indented);
                conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");

                retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;


                if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.SistemaTransben.RetornoWebServiceToken retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.SistemaTransben.RetornoWebServiceToken>(jsonRetorno);

                    if (String.IsNullOrEmpty(retorno?.Token))
                        throw new ServicoException($"Não retornou um token para ser utilizado!");

                    return (string)retorno?.Token ?? "";
                }
                else
                    throw new ServicoException($"Falha ao conectar no WS Sistema Transben: {retornoRequisicao.StatusCode}");

            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

            }

            return tokenAutenticacao;
        }

        private HttpClient CriarRequisicao(string url, string accessToken)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoSistemaTransben));
            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (accessToken != null)
            {
                requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            return requisicao;
        }

        private RetornoWebService Transmitir(object objEnvio, string metodo, string token, string uri)
        {
            var retornoWS = new Dominio.ObjetosDeValor.Embarcador.Integracao.SistemaTransben.RetornoWebService();

            try
            {
                if (_configuracaoIntegracao == null)
                    throw new ServicoException("Processo Abortado! Integração não configurada.");

                if (string.IsNullOrEmpty(uri))
                    throw new ServicoException("Processo Abortado! URL não definida.");

                string url = null;
                if (uri.EndsWith("/"))
                    url = uri;
                else
                    url = uri + "/";
                url += metodo;

                HttpClient requisicao = CriarRequisicao(url, token);

                retornoWS.JsonRequisicao = JsonConvert.SerializeObject(objEnvio, Formatting.Indented);

                HttpResponseMessage retornoRequisicao;

                StringContent conteudoRequisicao = new StringContent(retornoWS.JsonRequisicao, Encoding.UTF8, "application/json");
                retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;

                retornoWS.JsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK || retornoRequisicao.StatusCode == HttpStatusCode.Created)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.SistemaTransben.RetornoWebService retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.SistemaTransben.RetornoWebService>(retornoWS.JsonRetorno);

                    retornoWS.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    retornoWS.ProblemaIntegracao = "Registro integrado com sucesso";
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.SistemaTransben.RetornoWebServiceFalha retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.SistemaTransben.RetornoWebServiceFalha>(retornoWS.JsonRetorno);

                    if (retorno?.Error != null)
                        throw new ServicoException(@$"Erro: {string.Join(", ", retorno.Error)}");
                    else
                        throw new ServicoException($@"Erro: {string.Join(", ", retorno.Mensagem)}");
                }
            }
            catch (ServicoException ex)
            {
                retornoWS.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                retornoWS.ProblemaIntegracao = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                retornoWS.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                retornoWS.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração da TransSat";
            }

            if (retornoWS?.ProblemaIntegracao.Length > 300)
                retornoWS.ProblemaIntegracao = retornoWS.ProblemaIntegracao.Substring(0, 300);

            return retornoWS;
        }

        #endregion Métodos Privados
    }
}
