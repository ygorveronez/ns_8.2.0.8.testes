using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.GSW
{
    public class IntegracaoGSW
    {
        #region Atributos Globais

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion

        #region Construtores

        public IntegracaoGSW(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Métodos Públicos

        public bool IsPossuiIntegracaoGSW()
        {
            Repositorio.Embarcador.Cargas.TipoIntegracao repositorioTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);

            return repositorioTipoIntegracao.ExistePorTipo(TipoIntegracao.GSW);
        }

        public void ConsultarXMLCTes(AdminMultisoftware.Dominio.Enumeradores.TipoServicoMultisoftware tipoServicoMultisoftware)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Repositorio.Embarcador.Configuracoes.IntegracaoGSW repIntegracaoGSW = new Repositorio.Embarcador.Configuracoes.IntegracaoGSW(_unitOfWork);

            Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGSW integracaoGSW = repIntegracaoGSW.Buscar();

            if (integracaoGSW == null || string.IsNullOrWhiteSpace(integracaoGSW.URL) || string.IsNullOrWhiteSpace(integracaoGSW.Usuario) || string.IsNullOrWhiteSpace(integracaoGSW.Senha) || integracaoGSW.CodigoInicialConsultaXMLCTe == 0)
                return;

            try
            {
                string authToken = ObterToken(integracaoGSW);
                if (string.IsNullOrWhiteSpace(authToken))
                {
                    Log.TratarErro("Erro na autenticação GSW", "IntegracaoGSW");
                    return;
                }

                Dominio.ObjetosDeValor.Embarcador.Integracao.GSW.ConsultaXML objetoConsultaXML = new Dominio.ObjetosDeValor.Embarcador.Integracao.GSW.ConsultaXML()
                {
                    TipoXML = 1,
                    CodigoInicial = integracaoGSW.CodigoInicialConsultaXMLCTe
                };

                string url = integracaoGSW.URL + "/RequestFiles/GetXmls";
                HttpClient requisicao = CriarRequisicao(url, authToken);

                string jsonRequisicao = JsonConvert.SerializeObject(objetoConsultaXML, Formatting.Indented);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");
                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
                string jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
                {
                    Dominio.ObjetosDeValor.Embarcador.Integracao.GSW.RetornoConsultaXML retorno = JsonConvert.DeserializeObject<Dominio.ObjetosDeValor.Embarcador.Integracao.GSW.RetornoConsultaXML>(jsonRetorno);

                    if (retorno.StatusCode == 0)
                    {
                        long ultimoCodigoLeitura = integracaoGSW.CodigoInicialConsultaXMLCTe;
                        _unitOfWork.FlushAndClear();

                        foreach (Dominio.ObjetosDeValor.Embarcador.Integracao.GSW.RetornoConsultaXMLLista objeto in retorno.ListaXML)
                        {
                            byte[] byteArray = Convert.FromBase64String(objeto.XML);
                            MemoryStream arquivo = new MemoryStream(byteArray);

                            Servicos.Embarcador.CTe.CTe.ProcessarXMLCTe(arquivo, _unitOfWork, _unitOfWork.StringConexao, tipoServicoMultisoftware, "");

                            ultimoCodigoLeitura = objeto.Codigo;
                        }

                        integracaoGSW.CodigoInicialConsultaXMLCTe = ultimoCodigoLeitura;
                        repIntegracaoGSW.Atualizar(integracaoGSW);
                    }
                    else
                        Log.TratarErro($"Retorno GSW: {retorno.StatusCode} - {retorno.Mensagem}", "IntegracaoGSW");
                }
                else if (retornoRequisicao.StatusCode != HttpStatusCode.NoContent)
                    Log.TratarErro($"Falha ao conectar no WS GSW: {retornoRequisicao.StatusCode}", "IntegracaoGSW");
            }
            catch (Exception ex)
            {
                Log.TratarErro("Falha GSW: " + ex, "IntegracaoGSW");
            }
        }

        #endregion

        #region Métodos Privados

        private HttpClient CriarRequisicao(string url, string accessToken = null)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoGSW));

            requisicao.BaseAddress = new Uri(url);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrWhiteSpace(accessToken))
                requisicao.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            return requisicao;
        }

        private string ObterToken(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoGSW integracaoGSW)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string url = integracaoGSW.URL + "/Account/Login";
            HttpClient requisicao = CriarRequisicao(url);

            Dominio.ObjetosDeValor.Embarcador.Integracao.GSW.Autenticacao autenticacao = new Dominio.ObjetosDeValor.Embarcador.Integracao.GSW.Autenticacao()
            {
                Usuario = integracaoGSW.Usuario,
                Senha = integracaoGSW.Senha
            };

            string jsonRequisicao = JsonConvert.SerializeObject(autenticacao, Formatting.Indented);
            StringContent conteudoRequisicao = new StringContent(jsonRequisicao.ToString(), Encoding.UTF8, "application/json");

            HttpResponseMessage retornoRequisicao = requisicao.PostAsync(url, conteudoRequisicao).Result;
            string jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;

            if (retornoRequisicao.StatusCode == HttpStatusCode.OK)
            {
                dynamic obj = JsonConvert.DeserializeObject<dynamic>(jsonRetorno);
                return (string)obj.token;
            }
            else
                return null;
        }

        #endregion
    }
}
