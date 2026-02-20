using Dominio.Excecoes.Embarcador;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.ConfirmaFacil
{
    public partial class IntegracaoConfirmaFacil
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        #endregion Atributos Globais

        #region Construtores


        public IntegracaoConfirmaFacil(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }
        public IntegracaoConfirmaFacil(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmaFacil.retornoWebService Transmitir(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoConfirmaFacil configuracaoIntegracaoConfirmaFacil, string urlWebService, object request)
        {
            var retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.ConfirmaFacil.retornoWebService();

            try
            {
                if (!(configuracaoIntegracaoConfirmaFacil?.PossuiIntegracao ?? false))
                    throw new ServicoException("Não possui configuração para Confirma Fácil.");

                string url = $"{urlWebService}";

                if (urlWebService.EndsWith("/"))
                    url += "business/v2/embarque";
                else
                    url += "/business/v2/embarque";

                string token = ObterToken(urlWebService, configuracaoIntegracaoConfirmaFacil.Email, configuracaoIntegracaoConfirmaFacil.Senha, configuracaoIntegracaoConfirmaFacil.IDCliente, configuracaoIntegracaoConfirmaFacil.IDProduto);
                HttpClient requisicao = CriarRequisicao(url, token);

                retorno.jsonRequisicao = JsonConvert.SerializeObject(request, Formatting.Indented);

                //remover tag fotos caso vier null
                retorno.jsonRequisicao = retorno.jsonRequisicao.Replace(",\r\n      \"fotos\": null", "");

                StringContent conteudoRequisicao = new StringContent(retorno.jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                retorno.jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK || retornoRequisicao.StatusCode == HttpStatusCode.Accepted)
                {
                    retorno.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.Integrado;
                    retorno.ProblemaIntegracao = "Registro integrado com sucesso";
                }
                else
                    throw new ServicoException($"Falha ao conectar no WS Confirma Fácil: {retornoRequisicao.StatusCode}");
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
                retorno.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Confirma Fácil";
            }

            if (retorno?.ProblemaIntegracao.Length > 300)
                retorno.ProblemaIntegracao = retorno.ProblemaIntegracao.Substring(0, 300);

            return retorno;
        }

        private HttpClient CriarRequisicao(string url, string token)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoConfirmaFacil));
            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrEmpty(token))
            {
                requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token);
            }

            return requisicao;
        }

        private string ObterToken(string url, string email, string senha, string idcliente, string idproduto)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string endPoint = "";
            if (url.EndsWith("/"))
                endPoint = url + "login/login";
            else
                endPoint = url + "/login/login";

            string mensagemErro = string.Empty;

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoConfirmaFacil));

            client.BaseAddress = new Uri(endPoint);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                string jsonResponse = string.Empty;
                dynamic requestJson = new ExpandoObject();
                requestJson.email = email;
                requestJson.senha = senha;
                if (!string.IsNullOrEmpty(idcliente))
                    requestJson.idcliente = idcliente.ToInt();
                if (!string.IsNullOrEmpty(idproduto))
                    requestJson.idproduto = idproduto.ToInt();                

                string jsonRequest = JsonConvert.SerializeObject(requestJson, Formatting.Indented);

                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                var result = client.PostAsync(endPoint, content).Result;
                jsonResponse = result.Content.ReadAsStringAsync().Result;

                if (result.IsSuccessStatusCode)
                {

                    var retorno = (string)(result.Content.ReadAsStringAsync().Result);
                    if (!string.IsNullOrWhiteSpace(retorno))
                    {
                        dynamic objetoRetorno = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(retorno);

                        string tokenRetorno = (string)objetoRetorno.resposta.token;
                        if (!string.IsNullOrWhiteSpace(tokenRetorno))
                        {
                            return tokenRetorno;
                        }
                        else
                        {
                            Servicos.Log.TratarErro("token: não teve retorno", "IntegracaoConfirmaFacil");
                            return string.Empty;
                        }
                    }
                    else
                    {
                        Servicos.Log.TratarErro("token: não teve retorno", "IntegracaoConfirmaFacil");
                        return string.Empty;
                    }
                }
                else
                {
                    mensagemErro = "token: " + result.StatusCode.ToString();
                    Servicos.Log.TratarErro("token: " + mensagemErro, "IntegracaoConfirmaFacil");
                    return string.Empty;
                }
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro("ObterToken: " + excecao, "IntegracaoConfirmaFacil");
                Servicos.Log.TratarErro("URL: " + endPoint, "IntegracaoConfirmaFacil");

                return string.Empty;
            }
        }

        #endregion
    }
}
