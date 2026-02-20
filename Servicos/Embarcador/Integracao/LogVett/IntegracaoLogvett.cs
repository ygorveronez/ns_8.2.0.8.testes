using Dominio.Entidades.Embarcador.Cargas;
using Dominio.Excecoes.Embarcador;
using Dominio.ObjetosDeValor.Embarcador.Enumeradores;
using Dominio.ObjetosDeValor.Embarcador.Integracao.Logvett;
using Infrastructure.Services.HttpClientFactory;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;


namespace Servicos.Embarcador.Integracao.LogVett
{
    public sealed class IntegracaoLogvett
    {
        #region Atributos

        private readonly Repositorio.UnitOfWork _unitOfWork;

        #endregion Atributos

        #region Construtores

        public IntegracaoLogvett(Repositorio.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion Construtores

        #region Métodos Públicos

        public void IntegrarCarga(Dominio.Entidades.Embarcador.Cargas.CargaCargaIntegracao integracaoPendente)
        {
            ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo> servicoArquivoTransacao = new ArquivoTransacao<Dominio.Entidades.Embarcador.Cargas.CargaCTeIntegracaoArquivo>(_unitOfWork);

            integracaoPendente.NumeroTentativas++;
            integracaoPendente.DataIntegracao = DateTime.Now;

            string jsonRequisicao = string.Empty;
            string jsonRetorno = string.Empty;

            try
            {
                Repositorio.Embarcador.Configuracoes.IntegracaoLogvett repIntegracao = new Repositorio.Embarcador.Configuracoes.IntegracaoLogvett(_unitOfWork);
                Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLogvett configuracaoIntegracao = repIntegracao.Buscar();

                HttpClient requisicao = CriarRequisicao(configuracaoIntegracao);
                Dominio.ObjetosDeValor.Embarcador.Integracao.Logvett.RequisicaoTituloPagar corpoRequisicao = PreencherCorpoRequisicaoTituloPagar(integracaoPendente);

                jsonRequisicao = JsonConvert.SerializeObject(corpoRequisicao);
                StringContent conteudoRequisicao = new StringContent(jsonRequisicao, Encoding.UTF8, "application/json");

                HttpResponseMessage retornoRequisicao = requisicao.PostAsync(configuracaoIntegracao.URLTituloPagar, conteudoRequisicao).Result;
                jsonRetorno = retornoRequisicao.Content.ReadAsStringAsync().Result;
                dynamic retornoIntegracao = JsonConvert.DeserializeObject<dynamic>(jsonRetorno);

                if (retornoRequisicao.StatusCode == HttpStatusCode.OK || retornoRequisicao.StatusCode == HttpStatusCode.Created
                    || (retornoRequisicao.StatusCode == HttpStatusCode.BadRequest && !string.IsNullOrWhiteSpace((string)retornoIntegracao.msgRetorno)))
                {
                    integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.Integrado;
                    integracaoPendente.ProblemaIntegracao = (string)retornoIntegracao.msgRetorno;
                }
                else if (retornoRequisicao.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw new ServicoException("Houve um erro interno no servidor requisitado.");
                }
            }
            catch (ServicoException excecao)
            {
                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = excecao.Message;
            }
            catch (Exception excecao)
            {
                Log.TratarErro(excecao, "Integração Logvett");

                integracaoPendente.SituacaoIntegracao = SituacaoIntegracao.ProblemaIntegracao;
                integracaoPendente.ProblemaIntegracao = "Problema ao tentar integrar com Logvett.";
            }
            servicoArquivoTransacao.Adicionar(integracaoPendente, jsonRequisicao, jsonRetorno, "json");
        }

        #endregion Métodos Públicos

        #region Métodos Privados

        private HttpClient CriarRequisicao(Dominio.Entidades.Embarcador.Configuracoes.IntegracaoLogvett configuracaoIntegracao)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpClient requisicao = HttpClientFactoryWrapper.GetClient(nameof(IntegracaoLogvett));

            requisicao.BaseAddress = new Uri(configuracaoIntegracao.URLTituloPagar);
            requisicao.DefaultRequestHeaders.Accept.Clear();
            requisicao.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(configuracaoIntegracao.Usuario, configuracaoIntegracao.Senha);

            return requisicao;
        }

        private RequisicaoTituloPagar PreencherCorpoRequisicaoTituloPagar(CargaCargaIntegracao integracaoPendente)
        {
            //integracaoPendente.Codigo;
            //Repositorio.Embarcador.CIOT.CIOTEFrete
            return new RequisicaoTituloPagar
            {
                CnpjAutonomo = "12345678901234",
                CnpjIpaLog = "53113791000122",
                PrefixoTitulo = "1",
                NumeroTitulo = "987654321",
                TipoTitulo = "PA",
                ParcelaTitulo = "1",
                DataEmissaoTitulo = "2023-05-26",
                DataVencimentoTitulo = "2023-05-26",
                Historico = "1",
                ValorTitulo = 200,
            };
        }

        #endregion Métodos Privados
    }
}
