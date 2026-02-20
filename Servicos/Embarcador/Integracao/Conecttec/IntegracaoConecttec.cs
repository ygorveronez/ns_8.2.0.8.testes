using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using Servicos.ServicoAX.OrdemVenda;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;


namespace Servicos.Embarcador.Integracao.Conecttec
{
    public partial class IntegracaoConecttec
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Repositorio.Embarcador.Configuracoes.IntegracaoConecttec _configuracaoIntegracaoRepositorio;

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoConecttec _configuracaoIntegracaoConecttec;
        private Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;

        #endregion Atributos Globais

        #region Construtores

        public IntegracaoConecttec(Repositorio.UnitOfWork unitOfWork)
        {
            _configuracaoIntegracaoRepositorio = new Repositorio.Embarcador.Configuracoes.IntegracaoConecttec(unitOfWork);
            _configuracaoIntegracaoConecttec = _configuracaoIntegracaoRepositorio.Buscar();

            _unitOfWork = unitOfWork;
            _auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema,
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema
            };
        }

        #endregion Construtores

        #region Métodos Privados
        public bool AtualizarURLRecebimentoCallback(string pProviderID, string pURL, ref string mensagemErro)
        {
            try
            {
                object request = new
                {
                    providerId = pProviderID,
                    entryPointProvider = pURL
                };

                Dominio.ObjetosDeValor.Embarcador.Integracao.Conecttec.retornoWebService retWS = Transmitir("Provider/Callback", request, "POST", null);

                mensagemErro = retWS.ProblemaIntegracao;

                if (retWS.SituacaoIntegracao == SituacaoIntegracao.Integrado)
                    return true;
                else
                    return false;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                return false;
            }
        }

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Conecttec.retornoWebService Transmitir(string endPoint, object request, string metodo = "POST", string token = null)
        {
            var retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Conecttec.retornoWebService();

            try
            {
                if (!(_configuracaoIntegracaoConecttec?.PossuiIntegracao ?? false))
                    throw new ServicoException("Não possui configuração para Conecttec.");

                string url = $"{_configuracaoIntegracaoConecttec.URL}" + (_configuracaoIntegracaoConecttec.BrokerPort > 0 ? ":" + _configuracaoIntegracaoConecttec.BrokerPort : "");

                if (_configuracaoIntegracaoConecttec.URL.EndsWith("/"))
                    url += endPoint;
                else
                    url += "/" + endPoint;

                HttpClient requisicao = CriarRequisicao(url, token);
                string jsonRetorno = "";
                HttpResponseMessage retornoRequisicao = null;

                if (metodo == "POST")
                {
                    retorno.jsonRequisicao = JsonConvert.SerializeObject(request, Formatting.Indented);
                    StringContent conteudoRequisicao = new StringContent(retorno.jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                    retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                }
                else
                {
                    retornoRequisicao = requisicao.GetAsync(url).Result;
                    jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
                }
                
                retorno.jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK || retornoRequisicao.StatusCode == HttpStatusCode.Accepted || retornoRequisicao.StatusCode == HttpStatusCode.Created)
                {
                    retorno.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    retorno.ProblemaIntegracao = "Registro integrado com sucesso";
                }
                else
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.Conecttec.ErrorResponse retornoWS = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.Conecttec.ErrorResponse>(retorno.jsonRetorno);

                    if (retornoWS != null && retornoWS.Errors != null)
                    {
                        string concatenatedErrors = string.Empty;

                        // Itera sobre os erros e concatena na variável
                        foreach (var errorItem in retornoWS.Errors)
                        {
                            foreach (var message in errorItem.Value)
                            {
                                concatenatedErrors += $"Campo: {errorItem.Key} - Mensagem: {message}\n";
                            }
                        }

                        throw new ServicoException($"Falha ao integrar: {retornoWS.Status} - {concatenatedErrors}");
                    }

                    if (retornoWS != null && !string.IsNullOrEmpty(retornoWS.Title))
                        throw new ServicoException($"Falha ao integrar: {retornoWS.Status} - {retornoWS.Title}");

                    throw new ServicoException($"Falha ao conectar no WS Conecttec: {retornoRequisicao.StatusCode}");

                }
            }
            catch (ServicoException ex)
            {
                retorno.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                retorno.ProblemaIntegracao = ex.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao);

                retorno.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                retorno.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Conecttec: " + excecao.Message;
            }

            if (retorno?.ProblemaIntegracao.Length > 300)
                retorno.ProblemaIntegracao = retorno.ProblemaIntegracao.Substring(0, 300);

            return retorno;
        }
        private HttpClient CriarRequisicao(string url, string token = null)
        {
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoConecttec));
            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if(token != null)
                requisicao.DefaultRequestHeaders.Add("SecretKeyAuthorization", token);
            else
                requisicao.DefaultRequestHeaders.Add("SecretKeyAuthorization", _configuracaoIntegracaoConecttec.SecretKEY);

            return requisicao;
        }
        #endregion
    }
}

