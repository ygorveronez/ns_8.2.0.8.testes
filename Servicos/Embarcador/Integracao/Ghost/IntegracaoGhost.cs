using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Servicos.Embarcador.Integracao.Ghost
{
    public class IntegracaoGhost
    {
        #region Atributos
        private readonly Repositorio.UnitOfWork _unitOfWork;
        #endregion

        #region Construtores
        public IntegracaoGhost(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Métodos Públicos

        public void ProcessarIntegracao(Dominio.Entidades.Embarcador.Integracao.IntegracaoGhost integracao)
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoIntegracaoGhost repConfiguracaoIntegracao = new Repositorio.Embarcador.Configuracoes.ConfiguracaoIntegracaoGhost(_unitOfWork);
            Repositorio.Embarcador.Integracao.IntegracaoGhost repIntegracao = new Repositorio.Embarcador.Integracao.IntegracaoGhost(_unitOfWork);

            try
            {
                bool mule = integracao.TipoDestino == TipoDestinoGhost.MuleSoft;
                var configuracao = repConfiguracaoIntegracao.BuscarPrimeiroRegistro();

                integracao.NumeroTentativas += 1;
                integracao.DataIntegracao = DateTime.Now;

                string endPoint = mule ? configuracao.URLMule : configuracao.URLFilaH;

                HttpClient client;

                if (mule)
                    client = ObterClientMule(configuracao);
                else
                    client = ObterClientFilaH(configuracao);

                string jsonRequest = ObterRequisicao(integracao);
                var content = new StringContent(jsonRequest.ToString(), Encoding.UTF8, "application/json");
                var result = client.PostAsync(endPoint, content).Result;
                string jsonResponse = result.Content.ReadAsStringAsync().Result;

                if (mule)
                    ProcessarRetornoMule(integracao, result, jsonRequest, jsonResponse);
                else
                    ProcessarRetornoFilaH(integracao, result, jsonRequest, jsonResponse);

                repIntegracao.Atualizar(integracao);
            }
            catch (ServicoException excecao)
            {
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = excecao.Message;
                repIntegracao.Atualizar(integracao);
            }
            catch (Exception excecao)
            {
                Servicos.Log.TratarErro(excecao, "IntegracaoGhost");

                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = "Falha ao processar a integração";
                repIntegracao.Atualizar(integracao);
            }
        }

        public void GerarIntegracaoGhost(string requisicao, TipoDestinoGhost tipoDestino)
        {
            try
            {
                Repositorio.Embarcador.Integracao.IntegracaoGhost repIntegracaoGhost = new Repositorio.Embarcador.Integracao.IntegracaoGhost(_unitOfWork);
                Repositorio.Embarcador.Cargas.TipoIntegracao repTipoIntegracao = new Repositorio.Embarcador.Cargas.TipoIntegracao(_unitOfWork);
                Dominio.Entidades.Embarcador.Integracao.IntegracaoGhost integracao = new Dominio.Entidades.Embarcador.Integracao.IntegracaoGhost();

                Dominio.Entidades.Embarcador.Cargas.TipoIntegracao tipoIntegracao = repTipoIntegracao.BuscarPorTipo(TipoIntegracao.Ghost);

                if (tipoIntegracao == null)
                    return;

                string guid = Guid.NewGuid().ToString().Replace("-", "");

                integracao.SituacaoIntegracao = SituacaoIntegracao.AgIntegracao;
                integracao.DataIntegracao = DateTime.Now;
                integracao.ProblemaIntegracao = "";
                integracao.Guid = guid;
                integracao.TipoIntegracao = tipoIntegracao;
                integracao.TipoDestino = tipoDestino;

                SalvarArquivo(requisicao, guid);
                SalvarChaveRequisicao(integracao, requisicao, tipoDestino == TipoDestinoGhost.MuleSoft);

                repIntegracaoGhost.Inserir(integracao);
            }
            catch (Exception ex)
            {
                Servicos.Log.TratarErro($"Erro ao gerar o arquivo -> {ex.Message}", "IntegracaoGhost");
            }
        }

        public string ObterCaminhoPasta()
        {
            Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo repConfiguracaoArquivo = new Repositorio.Embarcador.Configuracoes.ConfiguracaoArquivo(_unitOfWork);
            Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoArquivo configuracaoArquivo = repConfiguracaoArquivo.BuscarPrimeiroRegistro();
            string caminhoGhost = Utilidades.IO.FileStorageService.Storage.Combine(configuracaoArquivo.CaminhoArquivosIntegracao, "Ghost");

#if DEBUG
            caminhoGhost = Servicos.FS.GetPath("C:\\Arquivos\\Ghost");
#endif
            
            return caminhoGhost;
        }

        #endregion

        #region Métodos Privados

        private void ProcessarRetornoMule(Dominio.Entidades.Embarcador.Integracao.IntegracaoGhost integracao, HttpResponseMessage result, string jsonRequest, string jsonResponse)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> svcArquivo = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            if (result.IsSuccessStatusCode)
            {
                integracao.ProblemaIntegracao = "Integrado com sucesso";
                integracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;

                svcArquivo.Adicionar(jsonRequest, jsonResponse, ".json", integracao);
            }
            else
            {
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = "Falha na integração";

                svcArquivo.Adicionar(jsonRequest, jsonResponse, ".json", integracao);
            }
        }

        private void ProcessarRetornoFilaH(Dominio.Entidades.Embarcador.Integracao.IntegracaoGhost integracao, HttpResponseMessage result, string jsonRequest, string jsonResponse)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> svcArquivo = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);
            dynamic retorno = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

            if (result.IsSuccessStatusCode)
            {
                HttpStatusCode httpStatusCode = (HttpStatusCode)retorno?.codigoMensagem;
                bool isSuccessStatusCode = ((int)httpStatusCode >= 200 && (int)httpStatusCode <= 299);

                if (isSuccessStatusCode)
                {
                    integracao.ProblemaIntegracao = "Integrado com sucesso";
                    integracao.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                }
                else
                {
                    integracao.ProblemaIntegracao = $"Retorno: {retorno?.mensagem}";
                    integracao.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                }
            }
            else
            {
                integracao.SituacaoIntegracao = Dominio.ObjetosDeValor.Embarcador.Enumeradores.SituacaoIntegracao.ProblemaIntegracao;
                integracao.ProblemaIntegracao = "Falha na integração";

            }
            svcArquivo.Adicionar(jsonRequest, jsonResponse, ".json", integracao);
        }

        private void SalvarArquivo(string requisicao, string guid)
        {
            string caminhoPasta = ObterCaminhoPasta();
            string caminhoArquivo = Utilidades.IO.FileStorageService.Storage.Combine(caminhoPasta, $"{guid}.json");

            Utilidades.IO.FileStorageService.Storage.WriteAllText(caminhoArquivo, requisicao);
            Servicos.Log.TratarErro($"Arquivo Gerado: {guid}", "IntegracaoGhost");
        }

        private HttpClient ObterClientMule(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoIntegracaoGhost configuracao)
        {
            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoGhost));

            client.BaseAddress = new Uri(configuracao.URLMule);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("client_id", configuracao.ClientIDMule);
            client.DefaultRequestHeaders.Add("client_secret", configuracao.ClientSecretMule);

            return client;
        }

        private HttpClient ObterClientFilaH(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoIntegracaoGhost configuracao)
        {

            string token = ObterTokenFilaH(configuracao);

            HttpClient client = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoGhost));

            client.BaseAddress = new Uri(configuracao.URLFilaH);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", token);

            return client;
        }

        private string ObterRequisicao(Dominio.Entidades.Embarcador.Integracao.IntegracaoGhost integracao)
        {
            string caminho = ObterCaminhoPasta();
            string caminhoCompleto = Utilidades.IO.FileStorageService.Storage.Combine(caminho, $"{integracao.Guid}.json");

            if (!Utilidades.IO.FileStorageService.Storage.Exists(caminhoCompleto))
                throw new ServicoException("Arquivo não encontrado");

            return Utilidades.IO.FileStorageService.Storage.ReadAllText(caminhoCompleto);
        }

        private void SalvarChaveRequisicao(Dominio.Entidades.Embarcador.Integracao.IntegracaoGhost integracao, string objEnvio, bool mule)
        {
            dynamic obj = JsonConvert.DeserializeObject(objEnvio);
            if (mule)
                integracao.ChaveRequisicao = (string)obj?.driverTicket ?? string.Empty;
            else
                integracao.ChaveRequisicao = ((string)obj?.DRVID).ToString() ?? string.Empty;
        }

        public string ObterTokenFilaH(Dominio.Entidades.Embarcador.Configuracoes.ConfiguracaoIntegracaoGhost configuracao)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var client = new RestClient(configuracao.URLAuthFilaH);
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);

            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("login", configuracao.LoginFilaH);
            request.AddParameter("password", configuracao.PasswordFilaH);
            request.AddParameter("grant_type", "client_credentials");

            IRestResponse response = client.Execute(request);

            if (!response.IsSuccessful)
                throw new ServicoException("Não foi possível obter o Token");

            dynamic retorno = JsonConvert.DeserializeObject<dynamic>(response.Content);

            return retorno.token;
        }

        #endregion
    }
}
