using Dominio.Excecoes.Embarcador;
using Infrastructure.Services.HttpClientFactory;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Servicos.Embarcador.Integracao.ATSSmartWeb
{
    public partial class IntegracaoATSSmartWeb
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly Repositorio.Embarcador.Configuracoes.IntegracaoATSSmartWeb _configuracaoIntegracaoRepositorio;

        private Dominio.Entidades.Embarcador.Configuracoes.IntegracaoATSSmartWeb _configuracaoIntegracaoATSSmartWeb;
        private Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado _auditado;

        #endregion Atributos Globais

        #region Construtores

        public IntegracaoATSSmartWeb(Repositorio.UnitOfWork unitOfWork)
        {
            _configuracaoIntegracaoRepositorio = new Repositorio.Embarcador.Configuracoes.IntegracaoATSSmartWeb(unitOfWork);
            _configuracaoIntegracaoATSSmartWeb = _configuracaoIntegracaoRepositorio.Buscar();

            _unitOfWork = unitOfWork;
            _auditado = new Dominio.ObjetosDeValor.Embarcador.Auditoria.Auditado()
            {
                OrigemAuditado = Dominio.ObjetosDeValor.Enumerador.OrigemAuditado.Sistema,
                TipoAuditado = Dominio.ObjetosDeValor.Enumerador.TipoAuditado.Sistema
            };
        }

        #endregion Construtores

        #region Métodos Privados

        #endregion

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.retornoWebService transmitir(string endPoint, object request, string metodo = "POST", string token = null)
        {
            var retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.retornoWebService();

            try
            {
                if (!(_configuracaoIntegracaoATSSmartWeb?.PossuiIntegracao ?? false))
                    throw new ServicoException("Não possui configuração para ATS Smart Web.");

                string url = $"{_configuracaoIntegracaoATSSmartWeb.URL}";

                if (_configuracaoIntegracaoATSSmartWeb.URL.EndsWith("/"))
                    url += endPoint;
                else
                    url += "/" + endPoint;

                HttpClient requisicao = criarRequisicao(url, token);
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
                    Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.errorResponse retornoWS = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.ATSSmartWeb.errorResponse>(retorno.jsonRetorno);

                    throw new ServicoException($"Falha ao conectar no WS ATSSmartWeb: {retornoRequisicao.StatusCode} - Retorno: {retornoWS.Retorno}");

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
                retorno.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a ATSSmartWeb: " + excecao.Message;
            }

            if (retorno?.ProblemaIntegracao.Length > 1000)
                retorno.ProblemaIntegracao = retorno.ProblemaIntegracao.Substring(0, 1000);

            return retorno;
        }
        private HttpClient criarRequisicao(string url, string token = null)
        {

            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoATSSmartWeb));
            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (token != null)
                requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token);
            else
            {
                token = gerarTokenJwt();
                requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token);

            }

            return requisicao;
        }

        private string gerarTokenJwt()
        {
            var secretKey = _configuracaoIntegracaoATSSmartWeb.SecretKEY;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var claims = new[]
            {
                new Claim("Email", _configuracaoIntegracaoATSSmartWeb.Usuario),
                new Claim("Senha", _configuracaoIntegracaoATSSmartWeb.Senha)
            };

            var token = new JwtSecurityToken(
                issuer: "ATS",  // ajuste conforme necessário
                audience: "ATS", // ajuste conforme necessário
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion
    }
}

