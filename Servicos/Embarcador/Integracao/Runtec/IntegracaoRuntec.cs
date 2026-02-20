using Dominio.Excecoes.Embarcador;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Runtec
{
    public partial class IntegracaoRuntec
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;
        private readonly AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware _tipoServicoMultisoftware;

        #endregion Atributos Globais

        #region Construtores


        public IntegracaoRuntec(Repositorio.UnitOfWork unitOfWork, AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            _unitOfWork = unitOfWork;
            _tipoServicoMultisoftware = tipoServicoMultisoftware;
        }
        public IntegracaoRuntec(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Privados

        private Dominio.ObjetosDeValor.Embarcador.Integracao.Runtec.retornoWebService Transmitir(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoRuntec configuracaoIntegracaoRuntec, string urlWebService, object request)
        {
            var retorno = new Dominio.ObjetosDeValor.Embarcador.Integracao.Runtec.retornoWebService();

            try
            {
                if (!(configuracaoIntegracaoRuntec?.PossuiIntegracao ?? false))
                    throw new ServicoException("Não possui configuração para Runtec.");

                string url = $"{urlWebService}";

                HttpClient requisicao = CriarRequisicao(url, configuracaoIntegracaoRuntec.Usuario, configuracaoIntegracaoRuntec.Senha);

                retorno.jsonRequisicao = JsonConvert.SerializeObject(request, Formatting.Indented);

                StringContent conteudoRequisicao = new StringContent(retorno.jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                retorno.jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK || retornoRequisicao.StatusCode == HttpStatusCode.Accepted)
                {
                    retorno.ProblemaIntegracao = "Registro integrado com sucesso";
                }
                else
                    throw new ServicoException($"Falha ao conectar no WS Runtec: {retornoRequisicao.StatusCode}");
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
                retorno.ProblemaIntegracao = "Ocorreu uma falha ao realizar a integração com a Runtec";
            }

            if (retorno?.ProblemaIntegracao.Length > 300)
                retorno.ProblemaIntegracao = retorno.ProblemaIntegracao.Substring(0, 300);

            return retorno;
        }

        private HttpClient CriarRequisicao(string url, string usuario, string senha)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoRuntec));
            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrEmpty(usuario) && !string.IsNullOrEmpty(usuario))
            {
                string credentials = String.Format("{0}:{1}", usuario, senha);
                byte[] bytes = Encoding.ASCII.GetBytes(credentials);
                string authorization = Convert.ToBase64String(bytes);
                requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authorization);
            }

            return requisicao;
        }

        #endregion
    }
}
